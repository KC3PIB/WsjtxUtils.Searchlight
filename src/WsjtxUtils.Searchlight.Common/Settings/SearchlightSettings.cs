namespace WsjtxUtils.Searchlight.Common.Settings
{
    /// <summary>
    /// Searchlight settings
    /// </summary>
    public class SearchlightSettings
    {
        /// <summary>
        /// Constructs a searchlight settings object
        /// </summary>
        public SearchlightSettings() : this(new WsjtxServer(), new ColorSettings(), new PskReporterSettings(), new LoggedQsoManagerSettings())
        {
        }

        /// <summary>
        /// Constructs a searchlight settings object
        /// </summary>
        /// <param name="server"></param>
        /// <param name="palette"></param>
        /// <param name="pskReporter"></param>
        /// <param name="loggedQsos"></param>
        public SearchlightSettings(WsjtxServer server, ColorSettings palette, PskReporterSettings pskReporter, LoggedQsoManagerSettings loggedQsos)
        {
            Server = server;
            Palette = palette;
            PskReporter = pskReporter;
            LoggedQsos = loggedQsos;
        }

        /// <summary>
        /// WSJT-X Server settings
        /// </summary>
        public WsjtxServer Server { get; set; }

        /// <summary>
        /// Searchlight color settings
        /// </summary>
        public ColorSettings Palette { get; set; }

        /// <summary>
        /// PSKReporter settings
        /// </summary>
        public PskReporterSettings PskReporter { get; set; }

        /// <summary>
        /// Logged QSO settings
        /// </summary>
        public LoggedQsoManagerSettings LoggedQsos { get; set; }
    }
}
