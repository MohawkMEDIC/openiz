using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Represents a single rule definition
    /// </summary>
    [XmlType(nameof(ProtocolRuleDefinition), Namespace = "http://openiz.org/cdss")]
    public class ProtocolRuleDefinition : DecisionSupportBaseElement
    {

        /// <summary>
        /// Represents a WHEN condition
        /// </summary>
        [XmlElement("when")]
        public ProtocolWhenClauseCollection When { get; set; }

        /// <summary>
        /// Represents the THEN conditions
        /// </summary>
        [XmlElement("then")]
        public ProtocolThenClauseCollection Then { get; set; }
    }
}