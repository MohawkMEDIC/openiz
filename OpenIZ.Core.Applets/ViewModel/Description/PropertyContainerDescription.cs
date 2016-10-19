using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.ViewModel.Description
{
    /// <summary>
    /// Property container description
    /// </summary>
    [XmlType(nameof(PropertyContainerDescription), Namespace = "http://openiz.org/model/view")]
    public abstract class PropertyContainerDescription
    {

        /// <summary>
        /// Type model description
        /// </summary>
        public PropertyContainerDescription()
        {
            this.Properties = new List<PropertyModelDescription>();
        }

        /// <summary>
        /// Identifies the properties to be included
        /// </summary>
        [XmlElement("property")]
        public List<PropertyModelDescription> Properties { get; set; }


        /// <summary>
        /// Whether to retrieve all children
        /// </summary>
        [XmlAttribute("all")]
        public bool All { get; set; }
    }
}
