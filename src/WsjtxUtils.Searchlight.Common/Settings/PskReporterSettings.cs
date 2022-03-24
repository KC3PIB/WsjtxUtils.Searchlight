using Serilog;

namespace WsjtxUtils.Searchlight.Common.Settings
{
    /// <summary>
    /// PSKReporter settings
    /// </summary>
    public class PskReporterSettings
    {
        private int _reportRetrievalPeriodSeconds;

        /// <summary>
        /// A negative number representing the number of seconds of reports
        /// </summary>
        public int ReportWindowSeconds { get; set; }

        /// <summary>
        /// How ofter to request the report in seconds
        /// </summary>
        /// <remarks>
        /// PSK Reporter can and will block request that exceed a rate of more than one every five minutes
        /// </remarks>
        public int ReportRetrievalPeriodSeconds
        {
            get => _reportRetrievalPeriodSeconds; set
            {
                if (value < 300)
                    Log.Warning("The current PskReporterSettings.ReportRetrievalPeriodSeconds value of {value} is less than 300 seconds, is not recommended, and will likely result in the requests being rejected for abuse. Phil, of PSK Reporter, has created a service of enormous importance to the amateur radio community, don't be a lid", value);
                _reportRetrievalPeriodSeconds = value;
            }
        }
    }
}
