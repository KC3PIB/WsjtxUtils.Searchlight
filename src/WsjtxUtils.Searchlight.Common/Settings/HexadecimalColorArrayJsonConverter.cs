using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WsjtxUtils.Searchlight.Common.Settings
{
    /// <summary>
    /// JSON Converter for hexidecimal color arrays
    /// </summary>
    public class HexadecimalColorArrayJsonConverter : JsonConverter<List<Color>>
    {
        /// <summary>
        /// Read a comma delimited list of hexidecimal color values and convert to an array of <see cref="System.Drawing.Color"/>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override List<Color>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new List<Color>();
            switch (reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                            break;
                        var hexColorList = reader.GetString();
                        if (!string.IsNullOrEmpty(hexColorList))
                            result.Add(ColorTranslator.FromHtml(hexColorList));
                    }
                    break;
                default:
                    var hexColor = reader.GetString();
                    if (!string.IsNullOrEmpty(hexColor))
                        result.Add(ColorTranslator.FromHtml(hexColor));
                    break;
            }
            return result;
        }

        /// <summary>
        /// Write an array of <see cref="System.Drawing.Color"/> to a delimited list of hexidecimal color values
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, List<Color> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteStringValue(ColorTranslator.ToHtml(item));
            writer.WriteEndArray();
        }
    }
}
