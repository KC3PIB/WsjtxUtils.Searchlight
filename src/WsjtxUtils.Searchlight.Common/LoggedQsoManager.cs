using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using WsjtxUtils.Searchlight.Common.Settings;
using WsjtxUtils.WsjtxMessages.Messages;
using WsjtxUtils.WsjtxMessages.QsoParsing;

namespace WsjtxUtils.Searchlight.Common
{
    /// <summary>
    /// Logged QSO manager behavior
    /// </summary>
    public enum QsoManagerBehavior
    {
        OncePerBand,
        OncePerBandAndMode,
    }

    public class LoggedQsoManager
    {
        /// <summary>
        /// Semaphore to manage log access
        /// </summary>
        private readonly SemaphoreSlim _qsoLogSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Settings that alter <see cref="nameof(LoggedQsoManager)"/> behavior
        /// </summary>
        private readonly LoggedQsoManagerSettings _settings;

        /// <summary>
        /// Platform specific newline characters
        /// </summary>
        private readonly ReadOnlyMemory<byte> _newlineBuffer;

        /// <summary>
        /// Logged QSO's key by band
        /// </summary>
        private readonly ConcurrentDictionary<ulong, ConcurrentBag<QsoLogged>> _loggedQso = new ConcurrentDictionary<ulong, ConcurrentBag<QsoLogged>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggedQsoManagerSettings"></param>
        public LoggedQsoManager(LoggedQsoManagerSettings loggedQsoManagerSettings)
        {
            _settings = loggedQsoManagerSettings;
            _newlineBuffer = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(Environment.NewLine));
        }

        /// <summary>
        /// Provides a list of previsouly contacted stations that are not highlighted already
        /// </summary>
        /// <param name="clientState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<WsjtxQso> GetPreviouslyContactedStationsNotHiglighted(SearchlightClientState clientState)
        {
            if (clientState.Status == null)
                throw new ArgumentNullException(nameof(clientState.Status));

            string client = clientState.Id;
            string callsign = clientState.Status.DECall;
            string mode = clientState.Status.Mode;
            ulong band = Utils.ApproximateBandFromFrequency(clientState.Status.DialFrequencyInHz);

            return clientState.DecodedStations
                .WherePreviouslyContacted(GetQsosLogged(band, callsign, mode))
                .WhereNotPreviouslyHiglighted(clientState)
                .Select(s => s.Value);
        }

        /// <summary>
        /// Get all logged QSOs for the band and mode and specified callsign
        /// </summary>
        /// <param name="band"></param>
        /// <param name="callsign"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public IEnumerable<QsoLogged> GetQsosLogged(ulong band, string callsign, string mode)
        {
            if (!_loggedQso.ContainsKey(band))
                return new List<QsoLogged>();

            var qsosForCallsign = _loggedQso[band].WhereDECallsign(callsign);

            if (_settings.QsoManagerBehavior == QsoManagerBehavior.OncePerBand)
                return qsosForCallsign;

            return qsosForCallsign.WhereMode(mode);
        }

        /// <summary>
        /// Log a <see cref="QsoLogged"/> message
        /// </summary>
        /// <param name="qsoLogged"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task LogQsoAsync(QsoLogged qsoLogged, CancellationToken cancellationToken = default)
        {
            // log the message to the local cache
            QsosLoggedForBand(qsoLogged.TXFrequencyInHz).Add(qsoLogged);

            // check for a log file
            if (string.IsNullOrEmpty(_settings.LogFilePath))
                return;

            // log the message to disk
            await _qsoLogSemaphore.WaitAsync();
            try
            {
                using(var stream = File.Open(_settings.LogFilePath, FileMode.Append))
                {
                    // write the object
                    await JsonSerializer.SerializeAsync<QsoLogged>(stream,
                        qsoLogged,
                        new JsonSerializerOptions { WriteIndented = false },
                        cancellationToken);

                    // write a newline
                    await stream.WriteAsync(_newlineBuffer, cancellationToken);
                }
            }
            finally
            {
                _qsoLogSemaphore.Release();
            }
        }

        /// <summary>
        /// Load all logged QSOs from the log file
        /// </summary>
        /// <param name="func"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ReadAllQsosFromLogFileAsync(CancellationToken cancellationToken = default)
        {
            await _qsoLogSemaphore.WaitAsync();
            try
            {
                using (TextReader r = new StreamReader(_settings.LogFilePath))
                {
                    string? line;
                    while ((line = await r.ReadLineAsync()) != null)
                    {
                        QsoLogged ? qso = JsonSerializer.Deserialize<QsoLogged>(line);
                        if (qso == null)
                            continue;

                        QsosLoggedForBand(qso.TXFrequencyInHz).Add(qso);
                    }
                }
            }
            finally
            {
                _qsoLogSemaphore.Release();
            }
        }

        /// <summary>
        /// Fetch the list of qso's for a given band
        /// </summary>
        /// <param name="frequencyInHertz"></param>
        /// <returns></returns>
        private ConcurrentBag<QsoLogged> QsosLoggedForBand(ulong frequencyInHertz)
        {
            var band = Utils.ApproximateBandFromFrequency(frequencyInHertz);

            return _loggedQso.AddOrUpdate(band, (qsoBag) =>
            {
                return new ConcurrentBag<QsoLogged>();
            },
           (band, qsoBag) =>
           {
               return qsoBag;
           });
        }
    }
}
