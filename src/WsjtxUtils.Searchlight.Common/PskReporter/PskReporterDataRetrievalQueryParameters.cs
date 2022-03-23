namespace WsjtxUtils.Searchlight.Common.PskReporter
{
    public class PskReporterDataRetrievalQueryParameters
    {
        /// <summary>
        /// Specifies the sending callsign of interest.
        /// </summary>
        public string? SenderCallsign { get; set; }

        /// <summary>
        /// Specifies the receiving callsign of interest.
        /// </summary>
        public string? ReceiverCallsign { get; set; }

        /// <summary>
        /// Specifies the callsign of interest. Specify only one of these three parameters.
        /// </summary>
        public string? Callsign { get; set; }

        /// <summary>
        /// A negative number of seconds to indicate how much data to retreive.
        /// This cannot be more than 24 hours.
        /// </summary>
        public int? FlowStartSeconds { get; set; }

        /// <summary>
        /// The mode of the signal that was detected.
        /// </summary>
        public string? Mode { get; set; }

        /// <summary>
        /// Limit the number of records returned.
        /// </summary>
        public int? RptLimit { get; set; }

        /// <summary>
        /// Only return the reception report records if non zero
        /// </summary>
        public bool? RrOnly { get; set; }

        /// <summary>
        /// Limit the number of records returned.
        /// </summary>
        public bool? NoActive { get; set; }

        /// <summary>
        /// A lower and upper limit of frequency. E.g. 14000000-14100000
        /// </summary>
        public string? FRange { get; set; }

        /// <summary>
        /// If non zero, then include reception reports that do not include a locator.
        /// </summary>
        public bool? NoLocator { get; set; }

        /// <summary>
        /// Causes the returned document to be javascript, and it will invoke the function named in the callback.
        /// </summary>
        public string? Callback { get; set; }

        /// <summary>
        /// Includes some statistical information
        /// </summary>
        public bool? Statistics { get; set; }

        /// <summary>
        /// If this has the value 'grid' then the callsign are interpreted as grid squares
        /// </summary>
        public string? Modify { get; set; }

        /// <summary>
        /// Limits search to records with a sequence number greater than or equal to this parameter. The last sequence number in the database is returned on each response.
        /// </summary>
        public int? LastSeqno { get; set; }

        /// <summary>
        /// PskReporter reserves right to block or rate limit anybody who imposes a significant load on their system. If you want to be notified, then please add an
        /// additional query parameter of 'appcontact=myemailaddress' so that they can contact you.
        /// </summary>
        public string? AppContact { get; set; }


        /// <summary>
        /// Convert the object to a dictionary with the keys being the correct query parameter for psk reporter data retrieval
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ToDictionary()
        {
            var dictionary = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(SenderCallsign))
                dictionary.Add("senderCallsign", SenderCallsign);

            if (!string.IsNullOrEmpty(ReceiverCallsign))
                dictionary.Add("receiverCallsign", ReceiverCallsign);

            if (!string.IsNullOrEmpty(Callsign))
                dictionary.Add("callsign", Callsign);

            if (FlowStartSeconds != null)
                dictionary.Add("flowStartSeconds", $"{FlowStartSeconds}");

            if (!string.IsNullOrEmpty(Mode))
                dictionary.Add("mode", Mode);

            if (RptLimit != null)
                dictionary.Add("rptlimit", $"{RptLimit}");

            if (RrOnly != null)
                dictionary.Add("rronly", $"{Convert.ToInt32(RrOnly)}");

            if (NoActive != null)
                dictionary.Add("noactive", $"{Convert.ToInt32(NoActive)}");

            if (!string.IsNullOrEmpty(FRange))
                dictionary.Add("frange", FRange);

            if (NoLocator != null)
                dictionary.Add("nolocator", $"{Convert.ToInt32(NoLocator)}");

            if (!string.IsNullOrEmpty(Callback))
                dictionary.Add("callback", $"{Callback}");

            if (Statistics != null)
                dictionary.Add("statistics", $"{Convert.ToInt32(Statistics)}");

            if (!string.IsNullOrEmpty(Modify))
                dictionary.Add("modify", $"{Modify}");

            if (LastSeqno != null)
                dictionary.Add("lastseqno", $"{LastSeqno}");

            if (!string.IsNullOrEmpty(AppContact))
                dictionary.Add("appcontact", $"{AppContact}");

            return dictionary;
        }
    }
}
