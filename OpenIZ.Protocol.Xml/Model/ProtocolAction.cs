using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Represents an action to take
    /// </summary>
    [XmlType(nameof(ProtocolAction), Namespace = "http://openiz.org/protocol")]
    public class ProtocolAction
    {

        /// <summary>
        /// Identifies the order in which the action should occur
        /// </summary>
        [XmlAttribute("sequence")]
        public int Sequence { get; set; }

        /// <summary>
        /// Represents pre-conditions or controls 
        /// </summary>
        [XmlElement("after")]
        public List<ProtocolCondition> PreCondition { get; set; }

        [XmlElement("until")]
        public List<ProtocolCondition> Until { get; set; }

        /// <summary>
        /// Indicates the number of times that the action should repeat
        /// </summary>
        [XmlElement("repeat")]
        public int Repeat { get; set; }

        /// <summary>
        /// Gets or sets the proposals made
        /// </summary>
        [XmlElement("model")]
        public List<ProtocolModel> Proposal { get; set; }
    }
}