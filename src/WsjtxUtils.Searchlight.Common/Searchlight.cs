using Serilog;
using System.Collections.Concurrent;
using System.Drawing;
using System.Net;
using System.Text.Json;
using WsjtxUtils.Searchlight.Common.Settings;
using WsjtxUtils.WsjtxMessages.Messages;
using WsjtxUtils.WsjtxUdpServer;

namespace WsjtxUtils.Searchlight.Common
{
    public class Searchlight : WsjtxUdpServerBaseAsyncMessageHandler
    {

        /// <summary>
        /// Various settings for use with searchlight
        /// </summary>
        private readonly SearchlightSettings _settings;

        /// <summary>
        /// State for all connected clients
        /// </summary>
        private readonly ConcurrentDictionary<string, SearchlightClientState> _searchlightClients = new ConcurrentDictionary<string, SearchlightClientState>();

        /// <summary>
        /// Reception reports
        /// </summary>
        private readonly ReceptionReporter _receptionReporter;

        /// <summary>
        /// WSJT-X UDP Server
        /// </summary>
        private readonly WsjtxUdpServer.WsjtxUdpServer _server;

        /// <summary>
        /// QSO Logger
        /// </summary>
        private readonly LoggedQsoManager _loggedQsos;

        /// <summary>
        /// Create a searchlight
        /// </summary>
        /// <param name="settings"></param>
        public Searchlight(SearchlightSettings settings)
        {
            _settings = settings;
            _receptionReporter = new ReceptionReporter(settings.PskReporter);
            _server = new WsjtxUdpServer.WsjtxUdpServer(this, IPAddress.Parse(_settings.Server.Address), _settings.Server.Port);
            _loggedQsos = new LoggedQsoManager(_settings.LoggedQsos);

            // load previouslt logged Qsos
            Task.Run(async () => await _loggedQsos.ReadAllQsosFromLogFileAsync());

            ClientConnectedCallback = (client) => { Log.Information("Client {server} connected", client.ClientId); return Task.CompletedTask; };
            ClientClosedCallback = (client) => { Log.Information("Client {server} disconnected", client.ClientId); return Task.CompletedTask; };
            ClientExpiredCallback = (client) => { Log.Information("Client {server} expired", client.ClientId); return Task.CompletedTask; };
        }

        /// <summary>
        /// Run the searchlight application loop
        /// </summary>
        /// <param name="cancellationTokenSource"></param>
        /// <returns></returns>
        public async Task RunAsync(CancellationTokenSource cancellationTokenSource)
        {
            Log.Information("Starting server at {server}:{port}", _settings.Server.Address, _settings.Server.Port);
            _server.Start();

            try
            {
                Task.WaitAll(_receptionReporter.PeriodicallyGatherReceptionReportsAsync(cancellationTokenSource.Token),
                    HighlightBasedOnReceptionReportAsync(_server, cancellationTokenSource.Token));
            }
            catch (AggregateException aggregateException)
            {
                aggregateException.Handle((innerException) =>
                {
                    if (innerException is not TaskCanceledException)
                    {
                        Log.Error(innerException, "An exception occured.");
                        return false;
                    }
                    return true;
                });
            }
            finally
            {
                // clear highlights on connected clients
                foreach (var client in ConnectedClients.Keys)
                    await ClearAllHighlightedCallsignsForClientAsync(client);

                Log.Information("Stopping server at {server}:{port}", _settings.Server.Address, _settings.Server.Port);
                _server.Stop();
            }
        }

        #region IWsjtxUdpMessageHandler
        /// <summary>
        /// Handle a QSO logged message
        /// </summary>
        /// <param name="server"></param>
        /// <param name="message"></param>
        /// <param name="endPoint"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task HandleQsoLoggedMessageAsync(WsjtxUdpServer.WsjtxUdpServer server, QsoLogged message, EndPoint endPoint, CancellationToken cancellationToken = default)
        {
            await base.HandleQsoLoggedMessageAsync(server, message, endPoint, cancellationToken);

            // log the QSO
            await _loggedQsos.LogQsoAsync(message);

            // send highlight message for the DX callsign
            await HighlightCallsignAsync(message.Id, message.DXCall, _settings.Palette.ContactedBackgroundColor, _settings.Palette.ContactedForegroundColor);
        }

        /// <summary>
        /// Handle a decoded message
        /// </summary>
        /// <param name="server"></param>
        /// <param name="message"></param>
        /// <param name="endPoint"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task HandleDecodeMessageAsync(WsjtxUdpServer.WsjtxUdpServer server, Decode message, EndPoint endPoint, CancellationToken cancellationToken = default)
        {
            await base.HandleDecodeMessageAsync(server, message, endPoint, cancellationToken);

            // add the decoded station to the client's state
            ClientStateFor(message.Id).AddDecodedStation(message, Math.Abs(_settings.PskReporter.ReportWindowSeconds) * 2);
        }

        /// <summary>
        /// Handle status messages
        /// </summary>
        /// <param name="server"></param>
        /// <param name="message"></param>
        /// <param name="endPoint"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task HandleStatusMessageAsync(WsjtxUdpServer.WsjtxUdpServer server, Status message, EndPoint endPoint, CancellationToken cancellationToken = default)
        {
            await base.HandleStatusMessageAsync(server, message, endPoint, cancellationToken);

            // set the status for the specified WSJT-X client
            ClientStateFor(message.Id).Status = message;

            // add the WSJT-X client DE callsign to the reception reporter
            _receptionReporter.TryAddCallsign(message.DECall);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Fetch the current state for a given WSJT-X client
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        private SearchlightClientState ClientStateFor(string clientId)
        {
            // get the correct list for this client
            return _searchlightClients.AddOrUpdate(clientId, (wsjtxClient) =>
            {
                return new SearchlightClientState(clientId);
            },
            (wsjtxClient, searchlightClient) =>
            {
                return searchlightClient;
            });
        }

        /// <summary>
        /// Highlight callsigns based on reception reports
        /// </summary>
        /// <param name="server"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task HighlightBasedOnReceptionReportAsync(WsjtxUdpServer.WsjtxUdpServer server, CancellationToken cancellationToken = default)
        {
            using (var timer = new PeriodicTimer(TimeSpan.FromSeconds(_settings.Palette.HighlightCallsignsPeriodSeconds)))
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    // get all clients that have a status
                    foreach (var kvp in ConnectedClients.WhereValidStatus())
                    {
                        // determine the callsign being used for this client and check if there are reception
                        // report for this callsign, current band, and mode
                        string client = kvp.Key;
                        string callsign = kvp.Value.Status!.DECall;
                        string mode = kvp.Value.Status.Mode;
                        ulong band = Utils.ApproximateBandFromFrequency(kvp.Value.Status.DialFrequencyInHz);

                        // check for any logged QSO's that are being decoded but haven't been highlighted
                        await CheckAndHighlightLoggedQsosAsync(ClientStateFor(client), cancellationToken);

                        Log.Verbose("Checking for reception reports for {callsign} with WSJT-X client '{client}' on {band} {mode}", callsign, client, kvp.Value.Status.DialFrequencyInHz, mode);
                        if (!_receptionReporter.TryGetReceptionReportState(callsign, out ReceptionReportState? receptionReportState) ||
                            receptionReportState == null ||
                            receptionReportState.ReceptionReports == null ||
                            receptionReportState.ReceptionReports.Reports == null ||
                            !receptionReportState.ReceptionReports.Reports.WhereBand(band).WhereMode(mode).Any())
                        {
                            Log.Verbose("No reception reports for {callsign} with WSJT-X client '{client}' on {band} {mode}", callsign, client, kvp.Value.Status.DialFrequencyInHz, mode);
                            continue;
                        }

                        var reportsForBandAndMode = receptionReportState.ReceptionReports.Reports.WhereBand(band).WhereMode(mode);
                        var reportSnrMin = reportsForBandAndMode.Min(r => r.SNR);
                        var reportSnrMax = reportsForBandAndMode.Max(r => r.SNR);

                        // if this is a new report, cleanup previous highlights
                        if (receptionReportState.IsDirty)
                        {
                            receptionReportState.IsDirty = false;

                            // clear all previous highlights
                            await ClearAllHighlightedCallsignsForClientAsync(client, cancellationToken);

                            // highlight logged qsos
                            await CheckAndHighlightLoggedQsosAsync(ClientStateFor(client), cancellationToken);
                        }

                        Log.Verbose("Checking {count} reports for {callsign} on '{client}' {band} {mode}", reportsForBandAndMode.Count(), callsign, client, kvp.Value.Status.DialFrequencyInHz, mode);

                        // find all heard stations that haven't already been contacted/higlighted and pull the reception reports for that station if it exists.
                        foreach (var heardStation in ClientStateFor(client).DecodedStations
                            .WhereNotPreviouslyContacted(_loggedQsos.GetQsosLogged(band, callsign, mode))
                            .WhereNotPreviouslyHiglighted(ClientStateFor(client))
                            .Select(s => s.Key))
                        {
                            // check for a reception report for the heard station
                            var receptionReport = receptionReportState.ReceptionReports.Reports
                                .WhereReceiverCallsign(heardStation)
                                .WhereBand(band)
                                .WhereMode(mode)
                                .FirstOrDefault();

                            if (receptionReport == null)
                                continue;

                            // colorize based on the signal report
                            int colorIndex = Utils.Scale(receptionReport.SNR, 1, _settings.Palette.ReceptionReportBackgroundColors.Count(), reportSnrMin, reportSnrMax) - 1;
                            Log.Information("Highlighting {station} {snr} with color {index} on WSJT-X client '{client}' - {mode}", heardStation, receptionReport.SNR, colorIndex, client, mode);
                            await HighlightCallsignAsync(client, heardStation, _settings.Palette.ReceptionReportBackgroundColors[colorIndex], _settings.Palette.ReceptionReportForegroundColor, cancellationToken);
                        }
                    }
                }
        }

        /// <summary>
        /// Check and highlight any logged QSO's
        /// </summary>
        /// <param name="server"></param>
        /// <param name="clientState"></param>
        /// <param name="qsoLogged"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task CheckAndHighlightLoggedQsosAsync(SearchlightClientState clientState, CancellationToken cancellationToken)
        {
            foreach (var decode in _loggedQsos.GetPreviouslyContactedStationsNotHiglighted(clientState))
            {
                Log.Information("Highlight logged QSO with {callsign} on WSJT-X client '{client}'", decode.DECallsign, clientState.Id);
                await HighlightCallsignAsync(clientState.Id, decode.DECallsign, _settings.Palette.ContactedBackgroundColor, _settings.Palette.ContactedForegroundColor, cancellationToken);
            }
        }

        /// <summary>
        /// Clear all highlighted callsigns for a given client
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task ClearAllHighlightedCallsignsForClientAsync(string client, CancellationToken cancellationToken = default)
        {
            foreach (var callsign in ClientStateFor(client).HighlightedCallsigns.Keys)
                await ClearHighlightedCallsignAsync(client, callsign, cancellationToken);
        }

        /// <summary>
        /// Clear a callsign from being highlighted
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="callsign"></param>
        /// <returns></returns>
        private async Task ClearHighlightedCallsignAsync(string client, string callsign, CancellationToken cancellationToken = default)
        {
            await HighlightCallsignAsync(client, callsign, Color.Empty, Color.Empty, cancellationToken);
        }

        /// <summary>
        /// Send a highlight callsign message to the specified client
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="callsign"></param>
        /// <param name="background"></param>
        /// <param name="foreground"></param>
        /// <returns></returns>
        private async Task HighlightCallsignAsync(string client, string callsign, Color background, Color foreground, CancellationToken cancellationToken = default)
        {
            HighlightCallsign message = new HighlightCallsign(client, callsign, background, foreground);

            if (background == Color.Empty && foreground == Color.Empty)
                ClientStateFor(client).RemoveHighlightedCallsign(message);
            else
                ClientStateFor(client).AddHighlightedCallsign(message);

            await _server.SendMessageToAsync(ConnectedClients[client].Endpoint, message, cancellationToken);
        }
        #endregion
    }
}
