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
        [JsonProperty("static")]
        public string IsStaticString
        {
            get
            {
                return (this.IsStatic ?? true).ToString();
            }
            set
            {
                if (value == null)
                    this.IsStatic = null;
                else
                    this.IsStatic = bool.Parse(value);
            }
        }

        /// <summary>
        /// Static
        /// </summary>
        [XmlIgnore]
        public bool? IsStatic { get; set; }

        /// <summary>
        /// Reference itself
        /// </summary>
        [XmlText]
        [JsonProperty("href")]
        public string Reference { get; set; }

    }
}