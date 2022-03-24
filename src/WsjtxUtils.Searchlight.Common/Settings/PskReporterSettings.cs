using Serilog;

namespace WsjtxUtils.Searchlight.Common.Settings
{
    /// <summary>
    /// PSKReporter settings
    /// </summary>
    public class PskReporterSettings
    {
        const int RetrievalPeriodWarnLimit = 300;

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
                if (value < RetrievalPeriodWarnLimit)
                    Log.Warning("The current PskReporterSettings.ReportRetrievalPeriodSeconds value of {value} is less than {limit} seconds, is not recommended, and will likely result in the requests being rejected for abuse. Philip does a considerable service to the ham radio community with this data, don't be a lid", value, RetrievalPeriodWarnLimit);
                _reportRetrievalPeriodSeconds = value;
            }
        }
    }
}
