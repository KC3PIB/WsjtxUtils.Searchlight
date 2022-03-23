using System.Xml.Serialization;

namespace WsjtxUtils.Searchlight.Common.PskReporter
{
    /// <summary>
    /// PSK Reporter XML element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PskElementWithValue<T>
    {
        /// <summary>
        /// The value of the XML element
        /// </summary>
        [XmlAttribute("value")]
        public T? Value { get; set; }
    }
}
