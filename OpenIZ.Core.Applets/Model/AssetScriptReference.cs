using Newtonsoft.Json;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
    /// <summary>
    /// Represents an asset script reference
    /// </summary>
    [XmlType(nameof(AssetScriptReference), Namespace = "http://openiz.org/applet")]
    [JsonObject(nameof(AssetScriptReference))]
    public class AssetScriptReference
    {
        /// <summary>
        /// True if the reference is static
        /// </summary>
        [XmlAttribute("static")]
        public bool IsStatic { get; set; }

        /// <summary>
        /// Reference itself
        /// </summary>
        [XmlText]
        [JsonProperty("href")]
        public string Reference { get; set; }

    }
}