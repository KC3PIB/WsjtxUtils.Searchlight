using System.Xml.Serialization;

namespace WsjtxUtils.Searchlight.Common.PskReporter
{
    /// <summary>
    /// PSK Recepetion report
    /// </summary>
    public class PskReceptionReport
    {
        /// <summary>
        /// The callsign of the receiver of the transmission
        /// </summary>
        [XmlAttribute("receiverCallsign")]
        public string? ReceiverCallsign { get; set; }

        /// <summary>
        /// The locator of the receiver of the transmission
        /// </summary>
        [XmlAttribute("receiverLocator")]
        public string? ReceiverLocator { get; set; }

        /// <summary>
        /// The callsign of the sender of the transmission
        /// </summary>
        [XmlAttribute("senderCallsign")]
        public string? SenderCallsign { get; set; }

        /// <summary>
        /// The locator of the sender of the transmission
        /// </summary>
        [XmlAttribute("senderLocator")]
        public string? SenderLocator { get; set; }

        /// <summary>
        /// The frequency of the transmission in Hertz
        /// </summary>
        [XmlAttribute("frequency")]
        public long Frequency { get; set; }

        /// <summary>
        /// Negative number representing the report timefram
        /// </summary>
        [XmlAttribute("flowStartSeconds")]
        public long FlowStartSeconds { get; set; }

        /// <summary>
        /// The mode of the communication
        /// </summary>
        [XmlAttribute("mode")]
        public string? Mode { get; set; }

        [XmlAttribute("isSender")]
        public bool IsSender { get; set; }

        /// <summary>
        /// Receiver DXCC
        /// </summary>
        [XmlAttribute("receiverDXCC")]
        public string? ReceiverDXCC { get; set; }

        /// <summary>
        /// Receiver DXCC code
        /// </summary>
        [XmlAttribute("receiverDXCCCode")]
        public string? ReceiverDXCCCode { get; set; }

        /// <summary>
        /// Sender last LOTW update
        /// </summary>
        [XmlAttribute("senderLotwUpload")]
        public DateTime SenderLotwUpload { get; set; }

        /// <summary>
        /// The signal to noise ration of the transmission
        /// </summary>
        [XmlAttribute("sNR")]
        public int SNR { get; set; }
    }
}
