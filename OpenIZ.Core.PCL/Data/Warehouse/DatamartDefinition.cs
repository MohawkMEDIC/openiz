using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Data.Warehouse
{
    /// <summary>
    /// Represents a datamart definition which contains the definition of fields for a datamart
    /// </summary>
    [JsonObject(nameof(DatamartDefinition)), XmlType(nameof(DatamartDefinition), Namespace = "http://openiz.org/warehousing")]
    [XmlRoot(nameof(DatamartDefinition), Namespace = "http://openiz.org/warehousing")]
    public class DatamartDefinition
    {
        /// <summary>
        /// Gets or sets the identifier of the data mart
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the data mart
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the time that the data mart was created
        /// </summary>
        [XmlAttribute("creationTime"), JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the datamart schema
        /// </summary>
        [XmlElement("schema"), JsonProperty("schema")]
        public DatamartSchema Schema { get; set; }
    }
}
