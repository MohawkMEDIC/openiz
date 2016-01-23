using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Instructions on how an extensionshould be handled
    /// </summary>
    
    [XmlType("ExtensionType", Namespace = "http://openiz.org/model")]
    public class ExtensionType : BaseEntityData
    {

        /// <summary>
        /// Gets or sets the extension handler
        /// </summary>
        [XmlIgnore]
        public Type ExtensionHandler { get; set; }
        
        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [XmlElement("name")]
        public String Name { get; set; }

    }
}
