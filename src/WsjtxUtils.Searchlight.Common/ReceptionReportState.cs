using WsjtxUtils.Searchlight.Common.PskReporter;

namespace WsjtxUtils.Searchlight.Common
{
    /// <summary>
    /// Reception report state
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
        /// Time in seconds for exponential backoff
        /// </summary>
        public double Backoff { get; set; }

        /// <summary>
        /// Has the report been updated before a refresh?
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Handle exponential backoff on retry
        /// </summary>
        /// <param name="seconds"></param>
        public void RetryWithExponentialBackoff(double seconds = 30)
        {
            Retry++;
            Backoff = seconds * Math.Pow(2, Retry + Random.Shared.NextDouble()); // simple backoff with jitter
        }
    }
}
