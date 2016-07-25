using System;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Represents trigger actions
    /// </summary>
    [XmlType(nameof(TriggerActionType), Namespace = "http://openiz.org/protocol")]
    [Flags]
    public enum TriggerActionType
    {
        Always = Inserted | Inserting | Updated | Updating | Obsoleted | Obsoleting | Querying | Queried,
        AfterPersistence = Inserted | Updated | Obsoleted,
        BeforePersistence = Inserting | Updating | Obsoleting,
        Inserted = 0x01,
        Inserting = 0x02,
        Updated = 0x04,
        Updating = 0x08,
        Obsoleted = 0x10,
        Obsoleting = 0x20,
        Queried = 0x40,
        Querying = 0x80
    }

    /// <summary>
    /// Represents a trigger that may fire the protocol to begin
    /// </summary>
    [XmlType(nameof(ProtocolTrigger), Namespace = "http://openiz.org/protocol")]
    public class ProtocolTrigger : IdentifiedElement
    {

        /// <summary>
        /// The action which triggers this action
        /// </summary>
        [XmlAttribute("action")]
        public TriggerActionType Action { get; set; }

        /// <summary>
        /// The type on which the action is subscribed
        /// </summary>
        [XmlAttribute("type")]
        public String TypeXml { get; set; }

        /// <summary>
        /// Protocol where clause
        /// </summary>
        [XmlElement("where")]
        public ProtocolWhereClause Where { get; set; }

        /// <summary>
        /// Gets the type
        /// </summary>
        public Type Type
        {
            get
            {
                return Type.GetType(this.TypeXml);
            }
        }

    }

}