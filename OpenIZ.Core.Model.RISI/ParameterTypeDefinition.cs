using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI
{
    /// <summary>
    /// Represents a parameter type definition
    /// </summary>
    [XmlType(nameof(ParameterTypeDefinition), Namespace = "http://openiz.org/risi")]
    [XmlRoot(nameof(ParameterTypeDefinition), Namespace = "http://openiz.org/risi")]
    public class ParameterTypeDefinition : BaseEntityData
    {

        /// <summary>
        /// Gets the name of the type
        /// </summary>
        [XmlElement("name")]
        public String Name { get; set; }

        /// <summary>
        /// System type
        /// </summary>
        [XmlIgnore]
        public Type SystemType { get; set; }

        /// <summary>
        /// Represents system type in xm
        /// </summary>
        [XmlElement("systemType")]
        public String SystemTypeXml { get { return this.SystemType.AssemblyQualifiedName; } set { this.SystemType = System.Type.GetType(value); } }

        /// <summary>
        /// Represents the auto-complete source
        /// </summary>
        [XmlElement("listAutoComplete", Type = typeof(ListAutoCompleteSourceDefinition))]
        [XmlElement("queryAutoComplete", Type = typeof(QueryAutoCompleteSourceDefinition))]
        public AutoCompleteSourceDefinition AutoCompleteSource { get; set; }

    }
}
