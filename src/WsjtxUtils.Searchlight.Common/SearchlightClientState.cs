using System.Collections.Concurrent;
using WsjtxUtils.Searchlight.Common.Wsjtx;
using WsjtxUtils.WsjtxMessages.Messages;

namespace WsjtxUtils.Searchlight.Common
{
    /// <summary>
    /// Searchligh client state
    /// </summary>
    public class SearchlightClientState
    {
        public SearchlightClientState(Status? status) : this(status, new ConcurrentDictionary<string, WsjtxQso>(), new ConcurrentDictionary<string, HighlightCallsign>())
        {
        }

        public SearchlightClientState(Status? status, ConcurrentDictionary<string, WsjtxQso> decodedStations, ConcurrentDictionary<string, HighlightCallsign> highlightedCallsigns)
        {
            Status = status;
            DecodedStations = decodedStations;
            HighlightedCallsigns = highlightedCallsigns;
        }

        /// <summary>
        /// Status for the client
        /// </summary>
        public Status? Status { get; set; }

        /// <summary>
        /// Dictionary of decoded stations keyed by remote callsign
        /// </summary>
        public ConcurrentDictionary<string, WsjtxQso> DecodedStations { get; private set; }

        /// <summary>
        /// Dictionary of highlighted stations keyed by callsign
        /// </summary>
        public ConcurrentDictionary<string, HighlightCallsign> HighlightedCallsigns { get; private set; }

        /// <summary>
        /// Add a callsign to the highlighted list
        /// </summary>
        /// <param name="message"></param>
        public void AddHighlightedCallsign(HighlightCallsign message)
        {
            HighlightedCallsigns.AddOrUpdate(message.Callsign, m => message, (k, v) => message);
        }

        /// <summary>
        /// Remove a callsign from the highlighted list
        /// </summary>
        /// <param name="message"></param>
        public void RemoveHighlightedCallsign(HighlightCallsign message)
        {
            if (HighlightedCallsigns.ContainsKey(message.Callsign))
                HighlightedCallsigns.TryRemove(message.Callsign, out _);
        }

        /// <summary>
        /// Add a station that was decoded to the list
        /// </summary>
        /// <param name="HighlightCallsign"></param>
        /// <param name="expiryInSeconds"></param>
        public void AddDecodedStation(Decode HighlightCallsign, int expiryInSeconds = 1800)
        {
            var qso = WsjtxQsoParser.ParseDecode(HighlightCallsign);

            DecodedStations.AddOrUpdate(qso.DECallsign, (deCallsign) =>
            {
                return qso;
            },
            (deCallsign, wsjtxQso) =>
            {
                return qso;
            });

            var expiredClients = DecodedStations.Values
                   .Where(target => (DateTime.UtcNow - target.Time).TotalSeconds > expiryInSeconds)
                   .Select(target => target.DECallsign);

            foreach (var id in expiredClients)
                DecodedStations.TryRemove(id, out WsjtxQso? target);
        }
    }
}
