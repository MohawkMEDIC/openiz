using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Protocol rule definition
    /// </summary>
    [XmlType(nameof(ProtocolVariableDefinition), Namespace = "http://openiz.org/cdss")]
    public class ProtocolVariableDefinition : PropertyAssignAction 
    {

        /// <summary>
        /// Gets the name of the variable
        /// </summary>
        [XmlAttribute("name")]
        public string VariableName { get; set; }


    }
}