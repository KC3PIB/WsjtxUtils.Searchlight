using Serilog;
using System.Collections.Concurrent;
using System.Net;
using WsjtxUtils.Searchlight.Common.PskReporter;
using WsjtxUtils.Searchlight.Common.Settings;

namespace WsjtxUtils.Searchlight.Common
{
    public class ReceptionReporter
    {
        private const int ExponentialBackoffSeconds = 30;
        private const int MaxRetries = 3;

        private readonly SemaphoreSlim _reportRequestSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Global dictionary for reception reports keyed by DE callsign
        /// </summary>
        private readonly ConcurrentDictionary<string, ReceptionReportState> _receptionReports = new ConcurrentDictionary<string, ReceptionReportState>();

        /// <summary>
        /// PSK Reporter settings
        /// </summary>
        private readonly PskReporterSettings _pskReporterSettings;

        /// <summary>
        /// Construct a PSK Reporter object
        /// </summary>
        /// <param name="pskReporterSettings"></param>
        public ReceptionReporter(PskReporterSettings pskReporterSettings)
        {
            _pskReporterSettings = pskReporterSettings;
        }

        /// <summary>
        /// Try to get the reception report state for a callsign
        /// </summary>
        /// <param name="callsign"></param>
        /// <param name="receptionReportState"></param>
        /// <returns></returns>
        public bool TryGetReceptionReportState(string callsign, out ReceptionReportState? receptionReportState)
        {
            if (_receptionReports.ContainsKey(callsign))
            {
                receptionReportState = _receptionReports[callsign];
                return true;
            }
            receptionReportState = null;
            return false;
        }

        /// <summary>
        /// Try to add a callsign to monitor for reception reports
        /// </summary>
        /// <param name="callsign"></param>
        /// <returns></returns>
        public bool TryAddCallsign(string callsign)
        {
            if (!_receptionReports.ContainsKey(callsign))
                return _receptionReports.TryAdd(callsign, new ReceptionReportState(DateTime.UtcNow));

            return false;
        }

        /// <summary>
        /// Try to remove a callsign from monitor for reception reports
        /// </summary>
        /// <param name="callsign"></param>
        /// <returns></returns>
        public bool TryRemoveCallsign(string callsign)
        {
            if (_receptionReports.ContainsKey(callsign))
                return _receptionReports.TryRemove(callsign, out ReceptionReportState? receptionReportState);

            return false;
        }

        /// <summary>
        /// Gather reception reports for all monitored callsigns
        /// </summary>
        /// <returns></returns>
        public async Task PeriodicallyGatherReceptionReportsAsync(CancellationToken cancellationToken = default)
        {
            using (var timer = new PeriodicTimer(TimeSpan.FromSeconds(1)))
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    foreach (var callsign in _receptionReports.Keys)
                    {
                        var receptionReportsForCallsignState = _receptionReports[callsign];

                        // check if a new report needs to be queried
                        if (!ShouldQueryReport(callsign, receptionReportsForCallsignState) || _reportRequestSemaphore.CurrentCount < 1)
                            continue;

                        await _reportRequestSemaphore.WaitAsync();
                        try
                        {
                            await GatherReceptionReportsForCallsignAsync(callsign, cancellationToken);
                        }
                        catch (HttpRequestException htttpRequestException)
                        {
                            Log.Warning("Report for {callsign} failed with {code} {message}, retry {retry}", callsign, htttpRequestException.StatusCode, htttpRequestException.Message, receptionReportsForCallsignState.Retry);

                            if (htttpRequestException.StatusCode != HttpStatusCode.ServiceUnavailable && receptionReportsForCallsignState.Retry > MaxRetries)
                                throw;

                            receptionReportsForCallsignState.RetryWithExponentialBackoff(ExponentialBackoffSeconds);
                        }
                        catch(Exception exception)
                        {
                            Log.Error(exception, "An error ocurred.");
                        }
                        finally
                        {
                            _reportRequestSemaphore.Release();
                        }
                    }
                }
        }

        /// <summary>
        /// Checks if the report should be queried
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool ShouldQueryReport(string callsign, ReceptionReportState state)
        {
            var secondsElasped = (DateTime.UtcNow - state.ReportTimestamp).TotalSeconds;
            var intervalInSeconds = _pskReporterSettings.ReportRetrievalPeriodSeconds + state.Backoff;

            if (secondsElasped > intervalInSeconds)
            {
                Log.Information("Querying reception report for {callsign}", callsign);
                return true;
            }

            Log.Verbose("Reception report update for {callsign} in {seconds} seconds", callsign, Convert.ToInt32(intervalInSeconds - secondsElasped));
            return false;
        }

        /// <summary>
        /// Fetch the PSK reporter reception reports
        /// </summary>
        /// <param name="callsign"></param>
        /// <param name="reportWindowSeconds"></param>
        /// <returns></returns>
        private async Task GatherReceptionReportsForCallsignAsync(string callsign, CancellationToken cancellationToken = default)
        {

            // fetch the PSK Reporter reception report
            var report = await PskReporterUtils.GetPskReporterData(new PskReporterDataRetrievalQueryParameters()
            {
                SenderCallsign = callsign,
                RrOnly = true,
                FlowStartSeconds = _pskReporterSettings.ReportWindowSeconds
            }, cancellationToken);


            if (report == null || report.Reports == null)
            {
                report = new PskReceptionReports() { Reports = new List<PskReceptionReport>() };
            }
            else if (report.Reports.Any())
            {
                // clean up the report to prevent records that do not have callsigns or a frequency
                report.Reports = report.Reports.Where(r => !string.IsNullOrEmpty(r.ReceiverCallsign) && r.Frequency != 0).ToList();
            }

            _receptionReports.AddOrUpdate(callsign,
                (callsign) => new ReceptionReportState(report),
                (callsign, reportState) =>
                {
                    reportState.ReceptionReports = report;
                    reportState.ReportTimestamp = DateTime.UtcNow;
                    reportState.Retry = 0;
                    reportState.Backoff = 0;
                    reportState.IsDirty = true;
                    return reportState;
                });

            Log.Information("Updated reception report for {callsign} with {count} receiving stations", callsign, report.Reports.Count());
        }
    }
}
