using Newtonsoft.Json;
using System.Xml.Serialization;
using System;
using OpenIZ.Core.Model.Attributes;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents information related to the clinical protocol to which an act is a member of
    /// </summary>
    [XmlType(nameof(ActProtocol), Namespace = "http://openiz.org/model"), JsonObject(nameof(ActProtocol))]
    public class ActProtocol : VersionedAssociation<Act>
    {

        /// <summary>
        /// Gets or sets the protocol  to which this act belongs
        /// </summary>
        [XmlElement("protocol"), JsonProperty("protocol")]
        public Guid ProtocolKey { get; set; }

        /// <summary>
        /// Gets or sets the protocol data related to the protocol
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReferenceAttribute(nameof(ProtocolKey))]
        public Protocol Protocol { get; set; }

        /// <summary>
        /// Represents any state data related to the act / protocol link
        /// </summary>
        [XmlElement("state"), JsonProperty("state")]
        public String StateData { get; set; }
    }
}