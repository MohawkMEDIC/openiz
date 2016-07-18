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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.EntityLoader;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a name component which is bound to a name
    /// </summary>
    [XmlType(Namespace = "http://openiz.org/model", TypeName = "EntityNameComponent")]
    [JsonObject("EntityNameComponent")]
    public class EntityNameComponent : GenericComponentValues<EntityName>
    {

        /// <summary>
        /// Entity name component 
        /// </summary>
        public EntityNameComponent()
        {

        }

        /// <summary>
        /// Creates the entity name component with the specified value
        /// </summary>
        /// <param name="value"></param>
        public EntityNameComponent(String value) : base(value)
        {

        }
        /// <summary>
        /// Creates the entity name component with the specified value and part type classifier
        /// </summary>
        /// <param name="partTypeKey"></param>
        /// <param name="value"></param>
        public EntityNameComponent(Guid partTypeKey, String value) : base(partTypeKey, value)
        {

        }
       
        /// <summary>
        /// Gets or sets the phonetic code of the reference term
        /// </summary>
        [XmlElement("phoneticCode"), JsonProperty("phoneticCode")]
        public String PhoneticCode { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the phonetic code
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [DataIgnore, XmlElement("phoneticAlgorithm"), JsonProperty("phoneticAlgorithm")]
        public Guid? PhoneticAlgorithmKey
        {
            get { return this.PhoneticAlgorithm?.Key; }
            set
            {
                if (value != this.PhoneticAlgorithm?.Key)
                    this.PhoneticAlgorithm = this.EntityProvider?.Get<PhoneticAlgorithm>(value);
            }
        }

        /// <summary>
        /// Gets or sets the phonetic algorithm
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(PhoneticAlgorithmKey))]
		public PhoneticAlgorithm PhoneticAlgorithm { get; set; }


    }
}