using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Represents an identified protocol element
    /// </summary>
    [XmlType(nameof(IdentifiedElement), Namespace = "http://openiz.org/protocol")]
    public class IdentifiedElement
    {

        /// <summary>
        /// Gets the unique identifier
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}