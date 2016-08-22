using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{

    /// <summary>
    /// When the rule should be fired
    /// </summary>
    [XmlType(nameof(RuleSetTriggerEvent), Namespace = "http://openiz.org/cdss")]
    public enum RuleSetTriggerEvent
    {
        Inserting,
        Inserted,
        Updating,
        Updated,
        Obsoleting,
        Obsoleted,
        Queried
    }

    /// <summary>
    /// Represents an independent when/then condition which may or may not be executed 
    /// when trigger events occur. Rules are different than protocols in that the ruleset is
    /// fired from a trigger or called manually
    /// </summary>
    [XmlType(nameof(RuleSetDefinition), Namespace = "http://openiz.org/cdss")]
    [XmlRoot(nameof(RuleSetDefinition), Namespace = "http://openiz.org/cdss")]
    public class RuleSetDefinition : DecisionSupportBaseElement
    {

        /// <summary>
        /// Triggers for the ruleset
        /// </summary>
        [XmlElement("trigger")]
        public List<RuleSetTrigger> Triggers { get; set; }

        /// <summary>
        /// When clause for the entire ruleset
        /// </summary>
        [XmlElement("when")]
        public ProtocolWhenClauseCollection When { get; set; }

        /// <summary>
        /// Gets or sets the rules
        /// </summary>
        [XmlElement("rule")]
        public List<ProtocolRuleDefinition> Rules { get; set; }

        /// <summary>
        /// Save the rules definition to the specified stream
        /// </summary>
        public void Save(Stream ms)
        {
            XmlSerializer xsz = new XmlSerializer(typeof(RuleSetDefinition));
            xsz.Serialize(ms, this);
        }

        /// <summary>
        /// Load the rules from the stream
        /// </summary>
        public static RuleSetDefinition Load(Stream ms)
        {
            XmlSerializer xsz = new XmlSerializer(typeof(RuleSetDefinition));
            return xsz.Deserialize(ms) as RuleSetDefinition;
        }

    }

    /// <summary>
    /// Represents a ruleset trigger
    /// </summary>
    [XmlType(nameof(RuleSetDefinition), Namespace = "http://openiz.org/cdss")]
    public class RuleSetTrigger
    {

        /// <summary>
        /// Type of the trigger
        /// </summary>
        [XmlAttribute("type")]
        public String Type { get; set; }

        /// <summary>
        /// The event of the trigger
        /// </summary>
        [XmlAttribute("event")]
        public RuleSetTriggerEvent Event { get; set; }

    }
}
