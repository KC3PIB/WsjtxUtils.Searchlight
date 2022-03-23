using System.Drawing;
using System.Text.Json.Serialization;

namespace WsjtxUtils.Searchlight.Common.Settings
{
    /// <summary>
    /// Searchlight color settings
    /// </summary>
    public class ColorSettings
    {
        /// <summary>
        /// Searchlight color settings
        /// </summary>
        public ColorSettings() : this(new List<Color>(), Color.Empty, Color.Empty, Color.Empty)
        {
        }

        /// <summary>
        /// Searchlight color settings
        /// </summary>
        /// <param name="receptionReportBackgroundColors"></param>
        /// <param name="receptionReportForegroundColor"></param>
        /// <param name="contactedBackgroundColor"></param>
        /// <param name="contactedForegroundColor"></param>
        public ColorSettings(List<Color> receptionReportBackgroundColors, Color receptionReportForegroundColor, Color contactedBackgroundColor, Color contactedForegroundColor)
        {
            ReceptionReportBackgroundColors = receptionReportBackgroundColors;
            ReceptionReportForegroundColor = receptionReportForegroundColor;
            ContactedBackgroundColor = contactedBackgroundColor;
            ContactedForegroundColor = contactedForegroundColor;
        }

        /// <summary>
        /// The background highlight colors used for reception reports
        /// </summary>
        [JsonConverter(typeof(HexadecimalColorArrayJsonConverter))]
        public List<Color> ReceptionReportBackgroundColors { get; set; }

        /// <summary>
        /// The foreground highlight colors used for reception reports
        /// </summary>
        [JsonConverter(typeof(HexadecimalColorJsonConverter))]
        public Color ReceptionReportForegroundColor { get; set; }

        /// <summary>
        /// The background highlight colors used for contacted stations
        /// </summary>
        [JsonConverter(typeof(HexadecimalColorJsonConverter))]
        public Color ContactedBackgroundColor { get; set; }

        /// <summary>
        /// The foreground highlight colors used for contacted stations
        /// </summary>
        [JsonConverter(typeof(HexadecimalColorJsonConverter))]
        public Color ContactedForegroundColor { get; set; }
    }
}
