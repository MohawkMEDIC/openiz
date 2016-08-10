using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a template definition
    /// </summary>
    [KeyLookup(nameof(Mnemonic))]
    [XmlType(nameof(TemplateDefinition), Namespace = "http://openiz.org/model"), JsonObject(nameof(TemplateDefinition))]
    public class TemplateDefinition : NonVersionedEntityData
    {

        /// <summary>
        /// Gets or sets the mnemonic
        /// </summary>
        [JsonProperty("mnemonic"), XmlElement("mnemonic")]
        public String Mnemonic { get; set; }

        /// <summary>
        /// Gets or set the name 
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the oid of the concept set
        /// </summary>
        [XmlElement("oid"), JsonProperty("oid")]
        public String Oid { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [XmlElement("description"), JsonProperty("description")]
        public String Description { get; set; }


    }
}
