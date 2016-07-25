using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Represents a clinical protocol
    /// </summary>
    [XmlType(nameof(Protocol), Namespace = "http://openiz.org/protocol")]
    public class Protocol : IdentifiedElement
    {
        /// <summary>
        /// Gets or sets the name of the protocol
        /// </summary>
        [XmlAttribute("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the trigger events
        /// </summary>
        [XmlElement("trigger")]
        public List<ProtocolTrigger> Trigger { get; set; }

        /// <summary>
        /// Gets or sets the action
        /// </summary>
        [XmlElement("action")]
        public List<ProtocolAction> Action { get; set; }

    }
}
