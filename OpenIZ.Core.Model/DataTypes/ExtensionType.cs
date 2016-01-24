using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Instructions on how an extensionshould be handled
    /// </summary>
    
    [XmlType("ExtensionType",  Namespace = "http://openiz.org/model"), JsonObject("ExtensionType")]
    public class ExtensionType : BaseEntityData
    {

        /// <summary>
        /// Gets or sets the extension handler
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Type ExtensionHandler { get; set; }
        
        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public String Name { get; set; }

    }
}
