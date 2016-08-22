using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Protocol definition file
    /// </summary>
    [XmlType(nameof(ProtocolDefinition), Namespace = "http://openiz.org/cdss")]
    [XmlRoot(nameof(ProtocolDefinition), Namespace = "http://openiz.org/cdss")]
    public class ProtocolDefinition : DecisionSupportBaseElement
    {

        /// <summary>
        /// When clause for the entire protocol
        /// </summary>
        [XmlElement("when")]
        public ProtocolWhenClauseCollection When { get; set; }

        /// <summary>
        /// Gets or sets the rules
        /// </summary>
        [XmlElement("rule")]
        public List<ProtocolRuleDefinition> Rules { get; set; }

        /// <summary>
        /// Save the protocol definition to the specified stream
        /// </summary>
        public void Save(Stream ms)
        {
            XmlSerializer xsz = new XmlSerializer(typeof(ProtocolDefinition));
            xsz.Serialize(ms, this);
        }

        /// <summary>
        /// Load the protocol from the stream
        /// </summary>
        public static ProtocolDefinition Load(Stream ms)
        {
            XmlSerializer xsz = new XmlSerializer(typeof(ProtocolDefinition));
            return xsz.Deserialize(ms) as ProtocolDefinition;
        }
    }
}