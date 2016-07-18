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
    /// Represents an entity telecom address
    /// </summary>
    [Classifier(nameof(AddressUse)), SimpleValue(nameof(Value))]
    [XmlType("EntityTelecomAddress",  Namespace = "http://openiz.org/model"), JsonObject("EntityTelecomAddress")]
    public class EntityTelecomAddress : VersionedAssociation<Entity>
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        public EntityTelecomAddress()
        {
            
        }

        /// <summary>
        /// Creates a new entity telecom address with specified use and value
        /// </summary>
        public EntityTelecomAddress(Guid addressUseKey, String value)
        {
            this.AddressUseKey = addressUseKey;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the name use key
        /// </summary>
        [DataIgnore, XmlElement("use"), JsonProperty("use")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid? AddressUseKey
        {
            get { return this.AddressUse?.Key; }
            set
            {
                if (this.AddressUse?.Key != value)
                    this.AddressUse = this.EntityProvider?.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the name use
        /// </summary>
        [AutoLoad, XmlIgnore, JsonIgnore, SerializationReference(nameof(AddressUseKey))]
		public Concept AddressUse { get; set; }

        /// <summary>
        /// Gets or sets the value of the telecom address
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public String Value { get; set; }



    }
}