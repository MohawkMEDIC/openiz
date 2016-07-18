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
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenIZ.Core.Model.EntityLoader;

namespace OpenIZ.Core.Model.Roles
{
    /// <summary>
    /// Represents a provider role of a person
    /// </summary>
    
    [XmlType("Provider",  Namespace = "http://openiz.org/model"), JsonObject("Provider")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Provider")]
    public class Provider : Person
    {

      

        /// <summary>
        /// Creates a new provider
        /// </summary>
        public Provider()
        {
            this.DeterminerConceptKey = DeterminerKeys.Specific;
            this.ClassConceptKey = EntityClassKeys.Provider;
        }

        /// <summary>
        /// Gets or sets the provider specialty key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("providerSpecialty"), JsonProperty("providerSpecialty")]
        public Guid? ProviderSpecialtyKey
        {
            get
            {
                return this.ProviderSpecialty?.Key;
            }
            set
            {
                if (this.ProviderSpecialty?.Key != value)
                    this.ProviderSpecialty = this.EntityProvider?.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the provider specialty
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(ProviderSpecialtyKey))]
		public Concept ProviderSpecialty { get; set; }
 
    }
}
