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
    public class DatamartStoredQuery
    {

        /// <summary>
        /// Gets or sets the provider identifier
        /// </summary>
        [XmlAttribute("provider"), JsonProperty("provider")]
        public String ProviderId { get; set; }

        /// <summary>
        /// Definition of the query
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Definition of the query
        /// </summary>
        [XmlText, JsonProperty("select")]
        public String Definition { get; set; }

    }
}
