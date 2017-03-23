using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OpenIZ.Core.Data.Warehouse
{
    /// <summary>
    /// Represents a datamart schema which gives hints to the properties to be stored from 
    /// a dynamic object
    /// </summary>
    [XmlType(nameof(DatamartSchema), Namespace = "http://openiz.org/warehousing"), JsonObject(nameof(DatamartSchema))]
    [XmlRoot(nameof(DatamartSchema), Namespace = "http://openiz.org/warehousing")]
    public class DatamartSchema : IDatamartSchemaPropertyContainer
    {

        /// <summary>
        /// Datamart schema
        /// </summary>
        public DatamartSchema()
        {
            this.Queries = new List<DatamartStoredQuery>();
            this.Properties = new List<DatamartSchemaProperty>();
        }

        /// <summary>
        /// Gets or sets the unique identifier for the schema itself
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the element in the database
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the property names for the schema element
        /// </summary>
        [XmlElement("property"), JsonProperty("property")]
        public List<DatamartSchemaProperty> Properties { get; set; }

        /// <summary>
        /// Gets or sets the query associated with the schema
        /// </summary>
        [XmlElement("sqp"), JsonProperty("sqp")]
        public List<DatamartStoredQuery> Queries { get; set; }

        /// <summary>
        /// Load a datamart schema from a stream
        /// </summary>
        public static DatamartSchema Load(Stream source)
        {
            XmlSerializer xsz = new XmlSerializer(typeof(DatamartSchema));
            return xsz.Deserialize(source) as DatamartSchema;
        }
    }
}