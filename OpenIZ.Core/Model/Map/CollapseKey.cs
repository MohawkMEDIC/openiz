using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Association map
    /// </summary>
    [XmlType(nameof(CollapseKey), Namespace = "http://openiz.org/model/map")]
    public class CollapseKey
    {
        /// <summary>
        /// Gets or sets the name of the property can be collapsed if a key is used
        /// </summary>
        [XmlAttribute("propertyName")]
        public String PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the key in the domain model which "PropertyName" can be collapsed
        /// </summary>
        [XmlAttribute("keyName")]
        public String KeyName { get; set; }
    }
}