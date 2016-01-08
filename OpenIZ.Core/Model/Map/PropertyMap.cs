using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Represents a property map
    /// </summary>
    [XmlType(nameof(PropertyMap), Namespace = "http://openiz.org/model/map")]
    public class PropertyMap
    {
        /// <summary>
        /// Gets or sets the name of the property in the model
        /// </summary>
        [XmlAttribute("modelName")]
        public String ModelName { get; set; }

        /// <summary>
        /// Gets or sets the name of the property in the domain model
        /// </summary>
        [XmlAttribute("domainName")]
        public String DomainName { get; set; }

        /// <summary>
        /// Identifies the route 
        /// </summary>
        [XmlElement("via")]
        public List<PropertyMap> Via { get; set; }
    }
}