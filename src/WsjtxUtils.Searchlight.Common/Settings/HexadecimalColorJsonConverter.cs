using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WsjtxUtils.Searchlight.Common.Settings
{
    /// <summary>
    /// JSON Converter for hexidecimal color string
    /// </summary>
    public class HexadecimalColorJsonConverter : JsonConverter<Color>
    {
        /// <summary>
        /// Read a hexidecimal color value and convert to a <see cref="System.Drawing.Color"/>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var hexColor = reader.GetString();
            if (string.IsNullOrEmpty(hexColor))
                return Color.Empty;

            return ColorTranslator.FromHtml(hexColor);
        }


        /// <summary>
        /// Write a <see cref="System.Drawing.Color"/> to a hexidecimal color
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(ColorTranslator.ToHtml(value));
        }
    }
}
