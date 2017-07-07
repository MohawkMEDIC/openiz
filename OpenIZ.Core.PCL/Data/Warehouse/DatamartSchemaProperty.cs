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
using System.Xml.Serialization;

namespace OpenIZ.Core.Data.Warehouse
{

    /// <summary>
    /// Represents a single property on the data mart schema
    /// </summary>
    [XmlType(nameof(DatamartSchemaProperty), Namespace = "http://openiz.org/warehousing"), JsonObject(nameof(DatamartSchemaProperty))]
    public class DatamartSchemaProperty : IDatamartSchemaPropertyContainer
    {
        /// <summary>
        /// Gets or sets the identifier of the warehouse property
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the property
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the type of property
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public SchemaPropertyType Type { get; set; }

        /// <summary>
        /// Gets or sets the attributes associated with the property
        /// </summary>
        [XmlAttribute("attributes"), JsonProperty("attributes")]
        public SchemaPropertyAttributes Attributes { get; set; }

        /// <summary>
        /// Gets or sets the sub-properties of this property
        /// </summary>
        [XmlElement("property"), JsonProperty("property")]
        public List<DatamartSchemaProperty> Properties { get; set; }
    }

    /// <summary>
    /// Identifies the type which a schema property carries
    /// </summary>
    [XmlType(nameof(SchemaPropertyAttributes), Namespace = "http://openiz.org/warehousing"), Flags]
    public enum SchemaPropertyAttributes
    {
        /// <summary>
        /// No attributes
        /// </summary>
        [XmlEnum("none")]
        None = 0x0,
        /// <summary>
        /// Indexed
        /// </summary>
        [XmlEnum("index")]
        Indexed = 0x1,
        /// <summary>
        /// Not null
        /// </summary>
        [XmlEnum("nonnull")]
        NotNull = 0x2,
        /// <summary>
        /// Unique
        /// </summary>
        [XmlEnum("unique")]
        Unique = 0x4
    }

    /// <summary>
    /// Identifies the type which a schema property carries
    /// </summary>
    [XmlType(nameof(SchemaPropertyType), Namespace = "http://openiz.org/warehousing")]
    public enum SchemaPropertyType
    {
        /// <summary>
        /// The object represents a string
        /// </summary>
        [XmlEnum("string")]
        String = 0,
        /// <summary>
        /// The object represents an integer
        /// </summary>
        [XmlEnum("int")]
        Integer = 1,
        /// <summary>
        /// The object represents a floating point number
        /// </summary>
        [XmlEnum("float")]
        Float = 2,
        /// <summary>
        /// Date
        /// </summary>
        [XmlEnum("date")]
        Date = 3,
        /// <summary>
        /// Identifies the column is a boolean
        /// </summary>
        [XmlEnum("bool")]
        Boolean = 4,
        /// <summary>
        /// Identifies the column is a UUID
        /// </summary>
        [XmlEnum("uuid")]
        Uuid = 5, 
        /// <summary>
        /// Identifies the column as binary
        /// </summary>
        [XmlEnum("binary")]
        Binary = 6,
        /// <summary>
        /// Decimal
        /// </summary>
        [XmlEnum("decimal")]
        Decimal = 7, 
        /// <summary>
        /// Identifies the column is an object which has other data
        /// </summary>
        [XmlEnum("object")]
        Object = 8,
        /// <summary>
        /// Identifies a column has a date and a time
        /// </summary>
        DateTime = 9,
        /// <summary>
        /// Represents a timestamp (with timezone)
        /// </summary>
        TimeStamp = 10
    }
}