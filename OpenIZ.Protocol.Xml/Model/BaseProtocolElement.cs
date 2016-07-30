using System;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Represents a base protocol element
    /// </summary>
    [XmlType(nameof(BaseProtocolElement), Namespace = "http://openiz.org/protocol")]
    public abstract class BaseProtocolElement
    {

        /// <summary>
        /// Gets or sets the identifier of the object
        /// </summary>
        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }

        /// <summary>
        /// Identifies the object within the protocol
        /// </summary>
        [XmlAttribute("id")]
        public String Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the object
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version of the object
        /// </summary>
        [XmlAttribute("version")]
        public string Version { get; set; }

    }
}