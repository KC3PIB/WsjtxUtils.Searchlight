namespace WsjtxUtils.Searchlight.Common.Settings
{
    /// <summary>
    /// Searchlight settings
    /// </summary>
    public class SearchlightSettings
    {
        public SearchlightSettings() : this(new WsjtxServer(), new ColorSettings(), new PskReporterSettings())
        {

        }

        public SearchlightSettings(WsjtxServer server, ColorSettings palette, PskReporterSettings pskReporter)
        {
            Server = server;
            Palette = palette;
            PskReporter = pskReporter;
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
    }
}
