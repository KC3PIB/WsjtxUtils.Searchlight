using WsjtxUtils.Searchlight.Common.PskReporter;

namespace WsjtxUtils.Searchlight.Common
{
    /// <summary>
    /// Recpetion report state
    /// </summary>
    public class ReceptionReportState
    {
        /// <summary>
        /// Reception report state
        /// </summary>
        public ReceptionReportState() : this(DateTime.UtcNow)
        {
        }

        /// <summary>
        /// Reception report state
        /// </summary>
        /// <param name="timestamp"></param>
        public ReceptionReportState(DateTime timestamp) : this(timestamp, null)
        {
        }

        /// <summary>
        /// Reception report state
        /// </summary>
        /// <param name="receptionReports"></param>
        public ReceptionReportState(PskReceptionReports? receptionReports) : this(DateTime.UtcNow, receptionReports)
        {
        }

        /// <summary>
        /// Reception report state
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="receptionReports"></param>
        public ReceptionReportState(DateTime timestamp, PskReceptionReports? receptionReports)
        {
            ReportTimestamp = timestamp;
            ReceptionReports = receptionReports;
            Retry = 0;
            IsDirty = true;
        }

        /// <summary>
        /// Last report timestamp
        /// </summary>
        public DateTime ReportTimestamp { get; set; }

        /// <summary>
        /// Last sucessful reception report
        /// </summary>
        public PskReceptionReports? ReceptionReports { get; set; }

        /// <summary>
        /// Number of retry when requesting a report
        /// </summary>
        public int Retry { get; set; }

        /// <summary>
        /// Has the report been updated before a refresh?
        /// </summary>
        public bool IsDirty { get; set; }
    }
}
