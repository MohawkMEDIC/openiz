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
 * Date: 2016-2-1
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;



namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents data that is identified by a key
    /// </summary>
    [XmlType("IdentifiedData",  Namespace = "http://openiz.org/model"), JsonObject("IdentifiedData")]
    public abstract class IdentifiedData : IIdentifiedEntity
    {
        /// <summary>
        /// Threadstatic source provider so it can be overridden in serialization
        /// </summary>
        [ThreadStatic]
        public static IEntitySourceProvider SourceProvider;

        /// <summary>
        /// Entity source
        /// </summary>
        public IdentifiedData()
        {
            this.EntityProvider = EntitySource.Current.Provider;
            this.Key = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the entity source
        /// </summary>
        [DataIgnore, XmlIgnore, JsonIgnore]
        public IEntitySourceProvider EntityProvider { get; set; }

        /// <summary>
        /// The internal primary key value of the entity
        /// </summary>
        [AutoLoad, XmlElement("id"), JsonProperty("id")]
        public Guid? Key { get; set; }

        /// <summary>
        /// True if key should be serialized
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeKey()
        {
            return this.Key.HasValue;
        }

        /// <summary>
        /// Gets the type
        /// </summary>
        [DataIgnore, XmlIgnore, JsonProperty("$type")]
        public virtual String Type
        {
            get
            {
                return this.GetType().GetTypeInfo().GetCustomAttribute<JsonObjectAttribute>().Id;
            }
            set { }
        }
        
        /// <summary>
        /// Clone the specified data
        /// </summary>
        public IdentifiedData Clone()
        {
            var retVal = this.MemberwiseClone() as IdentifiedData;
            return retVal;
        }

        /// <summary>
        /// True if this is a placeholder
        /// </summary>
        [DataIgnore, XmlIgnore, JsonIgnore]
        public bool IsLogicalNull { get; internal set;  }
      
    }
}
