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
 * Date: 2017-4-22
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections;

namespace OpenIZ.Core.Data.Warehouse
{

    /// <summary>
    /// Warehouse object property value
    /// </summary>
    [XmlType(nameof(DataWarehouseObjectPropertyValue), Namespace = "http://openiz.org/warehousing"), JsonObject(nameof(DataWarehouseObjectPropertyValue))]
    public class DataWarehouseObjectPropertyValue
    {

        /// <summary>
        /// Creates a new warehouse object
        /// </summary>
        public DataWarehouseObjectPropertyValue()
        {

        }

        /// <summary>
        /// Creates a new property value from kvp provided
        /// </summary>
        public DataWarehouseObjectPropertyValue(KeyValuePair<string, object> kvp)
        {
            this.Name = kvp.Key;
            if (kvp.Value is IDictionary<String, Object>)
                this.Value = (kvp.Value as IDictionary<String, Object>).Select(o=>new DataWarehouseObjectPropertyValue(o)).ToList();
            else
                this.Value = kvp.Value;
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [XmlAttribute("name")]
        public String Name { get; set; }

        /// <summary>
        /// Value of the object
        /// </summary>
        [XmlElement("string", typeof(String))]
        [XmlElement("bool", typeof(bool))]
        [XmlElement("decimal", typeof(Decimal))]
        [XmlElement("int", typeof(Int32))]
        [XmlElement("uuid", typeof(Guid))]
        [XmlElement("dateTime", typeof(DateTime))]
        [XmlElement("timestamp", typeof(DateTimeOffset))]
        [XmlElement("bytea", typeof(byte[]))]
        [XmlElement("object", typeof(List<DataWarehouseObjectPropertyValue>))]
        public Object Value { get; set; }
        
        /// <summary>
        /// Expand the value 
        /// </summary>
        public Object Expand()
        {
            if (this.Value is IList)
                return (this.Value as IList).OfType<DataWarehouseObjectPropertyValue>().ToDictionary(o => o.Name, o => o.Expand());
            else
                return this.Value;
        }
    }

    /// <summary>
    /// Data warehousing object wrapper 
    /// </summary>
    [XmlType(nameof(DataWarehouseObject), Namespace = "http://openiz.org/warehousing"), JsonObject(nameof(DataWarehouseObject))]
    [XmlRoot(nameof(DataWarehouseObject), Namespace = "http://openiz.org/warehousing")]
    public class DataWarehouseObject 
    {
        
        /// <summary>
        /// Data warehouse object wrapper
        /// </summary>
        public DataWarehouseObject()
        {
            this.Properties = new List<DataWarehouseObjectPropertyValue>();
        }

        /// <summary>
        /// Data warehouse object
        /// </summary>
        public DataWarehouseObject(IDictionary<String, Object> wrap) : this()
        {
            this.Properties = wrap.Select(o => new DataWarehouseObjectPropertyValue(o)).ToList();

        }

    
        /// <summary>
        /// Gets or sets the properties
        /// </summary>
        [XmlElement("property"), JsonProperty("p")]
        public List<DataWarehouseObjectPropertyValue> Properties { get; set; }

        /// <summary>
        /// Create an expando object
        /// </summary>
        public dynamic ToExpando()
        {
            return this.Properties.ToDictionary(o => o.Name, o => o.Expand());
        }
    }
}
