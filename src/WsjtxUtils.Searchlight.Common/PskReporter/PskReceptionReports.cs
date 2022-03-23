using System.Xml.Serialization;

namespace WsjtxUtils.Searchlight.Common.PskReporter
{
    [XmlRoot("receptionReports")]
    public class PskReceptionReports
    {
        /// <summary>
        /// The last sequence number in the database is returned on each response
        /// </summary>
        [XmlElement("lastSequenceNumber")]
        public PskElementWithValue<long>? LastSequenceNumber { get; set; }

        [XmlElement("maxFlowStartSeconds")]
        public PskElementWithValue<long>? MaxFlowStartSeconds { get; set; }

        /// <summary>
        /// Reception reports
        /// </summary>
        [XmlElement("receptionReport")]
        public List<PskReceptionReport>? Reports { get; set; }
    }
}
