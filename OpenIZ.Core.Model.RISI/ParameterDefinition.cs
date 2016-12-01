using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI.Model
{
    /// <summary>
    /// Represents a parameter definition
    /// </summary>
    [XmlType(nameof(ParameterDefinition), Namespace = "http://openiz.org/risi")]
    public class ParameterDefinition : BaseEntityData
    {

        /// <summary>
        /// Gets the type of parameter
        /// </summary>
        [XmlElement("type")]
        public Guid TypeId { get; set; }

        /// <summary>
        /// Gets the name of the parameter
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }


    }
}