using Newtonsoft.Json;
using System.Xml.Serialization;

namespace OpenIZ.Core.Data.Warehouse
{
    /// <summary>
    /// Represents the SQL for an actual query
    /// </summary>
    [XmlType(nameof(DatamartStoredQueryDefinition), Namespace = "http://openiz.org/warehousing"), JsonObject(nameof(DatamartStoredQueryDefinition))]
    public class DatamartStoredQueryDefinition
    {

        /// <summary>
        /// Provider identifier
        /// </summary>
        [XmlAttribute("provider"), JsonProperty("provider")]
        public string ProviderId { get; set; }

        /// <summary>
        /// The SQL 
        /// </summary>
        [XmlText, JsonProperty("sql")]
        public string Query { get; set; }
    }
}