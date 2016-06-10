using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
    /// <summary>
    /// Applet strings
    /// </summary>
    [XmlType(nameof(AppletStrings), Namespace = "http://openiz.org/applet")]
    [JsonObject]
    public class AppletStrings
    {

        /// <summary>
        /// Language of the strings
        /// </summary>
        [XmlAttribute("lang"), JsonProperty("lang")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the string
        /// </summary>
        [XmlElement("string"), JsonProperty("string")]
        public List<AppletStringData> String { get; set; }

    }

    /// <summary>
    /// Applet string data
    /// </summary>
    [XmlType(nameof(AppletStringData), Namespace = "http://openiz.org/applet")]
    [JsonObject]
    public class AppletStringData
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [XmlAttribute("key"), JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value of the property
        /// </summary>
        [XmlText, JsonProperty("value")]
        public string Value { get; set; }

    }
}