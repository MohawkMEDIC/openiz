/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2017-1-11
 */
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
        private static XmlSerializer s_xsz = new XmlSerializer(typeof(DatamartSchema));

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
            return s_xsz.Deserialize(source) as DatamartSchema;
        }
    }
}