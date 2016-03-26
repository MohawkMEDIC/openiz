﻿/*
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
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents an entity telecom address
    /// </summary>
    
    [XmlType("EntityTelecomAddress",  Namespace = "http://openiz.org/model"), JsonObject("EntityTelecomAddress")]
    public class EntityTelecomAddress : VersionedAssociation<Entity>
    {

        // Name use key
        private Guid? m_nameUseKey;
        // Name use concept
        
        private Concept m_nameUseConcept;

        /// <summary>
        /// Gets or sets the name use key
        /// </summary>
        [XmlElement("addressUse"), JsonProperty("addressUse")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? AddressUseKey
        {
            get { return this.m_nameUseKey; }
            set
            {
                this.m_nameUseKey = value;
                this.m_nameUseConcept = null;
            }
        }

        /// <summary>
        /// Gets or sets the name use
        /// </summary>
        [DelayLoad(nameof(AddressUseKey))]
        [XmlIgnore, JsonIgnore]
        public Concept AddressUse
        {
            get {
                this.m_nameUseConcept = base.DelayLoad(this.m_nameUseKey, this.m_nameUseConcept);
                return this.m_nameUseConcept;
            }
            set
            {
                this.m_nameUseConcept = value;
                this.m_nameUseKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the value of the telecom address
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public String Value { get; set; }

        /// <summary>
        /// Forces refresh of the delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_nameUseConcept = null;
        }

    }
}