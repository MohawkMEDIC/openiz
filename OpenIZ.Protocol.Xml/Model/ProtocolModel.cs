using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Identifies a model which the protocol creates
    /// </summary>
    [XmlType(nameof(ProtocolModel), Namespace = "http://openiz.org/protocol")]
    public class ProtocolModel
    {

        /// <summary>
        /// Identifies the element to be proposed
        /// </summary>
        [XmlElement("PatientEncounter", typeof(PatientEncounter), Namespace = "http://openiz.org/model")]
        [XmlElement("Act", typeof(Act), Namespace = "http://openiz.org/model")]
        [XmlElement("Observation", typeof(Observation), Namespace = "http://openiz.org/model")]
        [XmlElement("TextObservation", typeof(TextObservation), Namespace = "http://openiz.org/model")]
        [XmlElement("QuantityObservation", typeof(QuantityObservation), Namespace = "http://openiz.org/model")]
        [XmlElement("CodedObservation", typeof(CodedObservation), Namespace = "http://openiz.org/model")]
        [XmlElement("SubstanceAdministration", typeof(SubstanceAdministration), Namespace = "http://openiz.org/model")]
        public Act Element { get; set; }

    }
}