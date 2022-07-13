using System.Collections.Concurrent;
using WsjtxUtils.WsjtxMessages.Messages;
using WsjtxUtils.WsjtxMessages.QsoParsing;

namespace WsjtxUtils.Searchlight.Common
{
    /// <summary>
    /// Searchligh client state
    /// </summary>
    public class SearchlightClientState
    {
        /// <summary>
        /// Construct a searchlight client state
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public SearchlightClientState(string id, Status? status = null) : this(id, status, new ConcurrentDictionary<string, WsjtxQso>(), new ConcurrentDictionary<string, HighlightCallsign>())
        {
        }

        /// <summary>
        /// Construct a searchlight client state
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="decodedStations"></param>
        /// <param name="highlightedCallsigns"></param>
        public SearchlightClientState(string id, Status? status, ConcurrentDictionary<string, WsjtxQso> decodedStations, ConcurrentDictionary<string, HighlightCallsign> highlightedCallsigns)
        {
            Id = id;
            Status = status;
            DecodedStations = decodedStations;
            HighlightedCallsigns = highlightedCallsigns;
        }

        /// <summary>
        /// WSJT-X client id
        /// </summary>
        public string Id { get; set; }

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
        /// <param name="decode"></param>
        /// <param name="expiryInSeconds"></param>
        public void AddDecodedStation(Decode decode, int expiryInSeconds = 1800)
        {
            var qso = WsjtxQsoParser.ParseDecode(decode);

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
