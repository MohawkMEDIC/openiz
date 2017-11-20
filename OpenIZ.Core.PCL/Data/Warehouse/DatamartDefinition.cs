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
