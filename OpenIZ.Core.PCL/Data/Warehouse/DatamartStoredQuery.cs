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
    /// Represents a stored query creation statement
    /// </summary>
    [XmlType(nameof(DatamartStoredQuery), Namespace = "http://openiz.org/warehousing"), JsonObject(nameof(DatamartStoredQuery))]
    [XmlRoot(nameof(DatamartStoredQuery), Namespace = "http://openiz.org/warehousing")]
    public class DatamartStoredQuery : IDatamartSchemaPropertyContainer
    {

        /// <summary>
        /// Stored query
        /// </summary>
        public DatamartStoredQuery()
        {
            this.Definition = new List<DatamartStoredQueryDefinition>();
            this.Properties = new List<DatamartSchemaProperty>();
        }

        /// <summary>
        /// Gets or sets the provider identifier
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Definition of the query
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the property names for the schema element
        /// </summary>
        [XmlElement("property"), JsonProperty("property")]
        public List<DatamartSchemaProperty> Properties { get; set; }

        /// <summary>
        /// Definition of the query
        /// </summary>
        [XmlElement("sql"), JsonProperty("select")]
        public List<DatamartStoredQueryDefinition> Definition { get; set; }

    }
}
