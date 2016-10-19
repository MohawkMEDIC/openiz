/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-16
 */
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Linq;
using OpenIZ.Core.Model.EntityLoader;
using Newtonsoft.Json;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Entity address
    /// </summary>
    [Classifier(nameof(AddressUse))]
    [XmlType("EntityAddress",  Namespace = "http://openiz.org/model"), JsonObject("EntityAddress")]
    public class EntityAddress : VersionedAssociation<Entity>
    {

        /// <summary>
        /// Create the address from components
        /// </summary>
        public EntityAddress(Guid useKey, String streetAddressLine, String city, String province, String country, String zipCode)
        {
            this.m_addressUseKey = useKey;
            this.Component = new List<EntityAddressComponent>();
            if (!String.IsNullOrEmpty(streetAddressLine))
                this.Component.Add(new EntityAddressComponent(AddressComponentKeys.StreetAddressLine, streetAddressLine));
            if (!String.IsNullOrEmpty(city))
                this.Component.Add(new EntityAddressComponent(AddressComponentKeys.City, city));
            if (!String.IsNullOrEmpty(province))
                this.Component.Add(new EntityAddressComponent(AddressComponentKeys.State, province));
            if (!String.IsNullOrEmpty(country))
                this.Component.Add(new EntityAddressComponent(AddressComponentKeys.Country, country));
            if (!String.IsNullOrEmpty(zipCode))
                this.Component.Add(new EntityAddressComponent(AddressComponentKeys.PostalCode, zipCode));
        }
        /// <summary>
        /// Default CTOR
        /// </summary>
        public EntityAddress()
        {
            this.Component = new List<EntityAddressComponent>();
        }

        // Address use key
        private Guid? m_addressUseKey;
        // Address use concept
        private Concept m_addressUseConcept;

        /// <summary>
        /// Gets or sets the address use key
        /// </summary>
        [XmlElement("use"), JsonProperty("use")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Binding(typeof(AddressUseKeys))]
        public Guid? AddressUseKey
        {
            get { return this.m_addressUseKey; }
            set
            {
                if (this.m_addressUseKey != value)
                {
                    this.m_addressUseKey = value;
                    this.m_addressUseConcept = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the address use
        /// </summary>
        [SerializationReference(nameof(AddressUseKey))]
        [XmlIgnore, JsonIgnore]
        [AutoLoad]
        public Concept AddressUse
        {
            get {
                this.m_addressUseConcept = base.DelayLoad(this.m_addressUseKey, this.m_addressUseConcept);
                return this.m_addressUseConcept;
            }
            set
            {
                this.m_addressUseConcept = value;
                this.m_addressUseKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the component types
        /// </summary>
        [XmlElement("component"), JsonProperty("component")]
        [AutoLoad]
        public List<EntityAddressComponent> Component { get; set; }

        /// <summary>
        /// Remove empty components
        /// </summary>
        public override IdentifiedData Clean()
        {
            this.Component.RemoveAll(o => String.IsNullOrEmpty(o.Value));
            return this;
        }

        /// <summary>
        /// True if empty
        /// </summary>
        /// <returns></returns>
        public override bool IsEmpty()
        {
            return this.Component.Count == 0;
        }
    }
}