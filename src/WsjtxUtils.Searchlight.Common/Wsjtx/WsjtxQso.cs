using WsjtxUtils.WsjtxMessages.Messages;

namespace WsjtxUtils.Searchlight.Common.Wsjtx
{
    /// <summary>
    /// A decoded WSJT-X QSO
    /// </summary>
    public class WsjtxQso
    {
        /// <summary>
        /// Decoded WSJT-X QSO
        /// </summary>
        /// <param name="decode"></param>
        public WsjtxQso(Decode decode) : this(decode, string.Empty, string.Empty, string.Empty, string.Empty)
        {

        }

        /// <summary>
        /// Decoded WSJT-X QSO
        /// </summary>
        /// <param name="decode"></param>
        /// <param name="callingModifier"></param>
        /// <param name="dxCallsign"></param>
        /// <param name="deCallsign"></param>
        /// <param name="gridSquare"></param>
        public WsjtxQso(Decode decode, string callingModifier, string dxCallsign, string deCallsign, string gridSquare)
        {
            RawMessage = decode.Message;
            Time = DateTime.UtcNow.Date.AddSeconds(decode.Time / 1000);
            QsoState = QsoState.Unknown;
            Mode = decode.Mode;
            Snr = decode.Snr;
            OffsetTimeSeconds = decode.OffsetTimeSeconds;
            OffsetFrequencyHz = decode.OffsetFrequencyHz;
            LowConfidence = decode.LowConfidence;
            CallingModifier = callingModifier;
            DXCallsign = dxCallsign;
            DECallsign = deCallsign;
            GridSquare = gridSquare;
        }

        /// <summary>
        /// The date and time the message was received by the local station
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The WSJT-X mode
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// The signal to noise from the remote to local station
        /// </summary>
        public int Snr { get; set; }

        /// <summary>
        /// The time difference from the remote to local station
        /// </summary>
        public float OffsetTimeSeconds { get; set; }

        /// <summary>
        /// The frequency delta of the remote station
        /// </summary>
        public uint OffsetFrequencyHz { get; set; }

        /// <summary>
        /// Low confidence decodes are flagged in protocols where the decoder
        /// has knows that a decode has a higher than normal probability
        /// of being false, they should not be reported on publicly
        /// accessible services without some attached warning or further validation.
        /// </summary>
        public bool LowConfidence { get; set; }

        /// <summary>
        /// State of the current QSO
        /// </summary>
        public QsoState QsoState { get; set; }

        /// <summary>
        /// Was priori information used to complete this call
        /// </summary>
        /// <remarks>https://www.physics.princeton.edu/pulsar/K1JT/wsjtx-doc/wsjtx-main-2.5.4.html#_ap_decoding</remarks>
        public bool UsedPriori { get; set; }

        /// <summary>
        /// Is the station calling CQ
        /// </summary>
        public bool IsCallingCQ { get => QsoState == QsoState.CallingCq; }

        /// <summary>
        /// The CQ calling modifier
        /// </summary>
        public string CallingModifier { get; set; }

        /// <summary>
        /// The gridsquare of the calling station
        /// </summary>
        public string GridSquare { get; set; }

        /// <summary>
        /// The local callsign
        /// </summary>
        public string DECallsign { get; set; }

        /// <summary>
        /// The remote callsign
        /// </summary>
        public string DXCallsign { get; set; }

        /// <summary>
        /// The raw decode message
        /// </summary>
        public string RawMessage { get; set; }
    }
}
