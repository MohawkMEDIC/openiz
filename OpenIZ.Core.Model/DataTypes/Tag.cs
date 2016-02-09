/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-24
 */
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents the base class for tags
    /// </summary>
    
    [XmlType(Namespace = "http://openiz.org/model")]
    public abstract class Tag<TSourceType> : Association<TSourceType> where TSourceType : IdentifiedData
    {

        /// <summary>
        /// Gets or sets the key of the tag
        /// </summary>
        [XmlElement("key"), JsonProperty("key")]
        public String TagKey { get; set; }

        /// <summary>
        /// Gets or sets the value of the tag
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public String Value { get; set; }

    }

    /// <summary>
    /// Represents a tag associated with an entity
    /// </summary>
    
    [XmlType("EntityTag",  Namespace = "http://openiz.org/model"), JsonObject("EntityTag")]
    public class EntityTag : Tag<Entity>
    {

    }


    /// <summary>
    /// Represents a tag on an act
    /// </summary>
    
    [XmlType("ActTag",  Namespace = "http://openiz.org/model"), JsonObject("ActTag")]
    public class ActTag : Tag<Act>
    {

    }

}
