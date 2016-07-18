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
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a basic information class which classifies the use of an identifier
    /// </summary>
    
    [XmlType(nameof(IdentifierType),  Namespace = "http://openiz.org/model"), JsonObject("IdentifierType")]
    [XmlRoot(nameof(IdentifierType), Namespace = "http://openiz.org/model")]
    public class IdentifierType : BaseEntityData
    {

       
        /// <summary>
        /// Gets or sets the id of the scope concept
        /// </summary>
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("scopeConcept"), JsonProperty("scopeConcept")]
        public Guid?  ScopeConceptKey
        {
            get { return this.ScopeConcept?.Key; }
            set
            {
                if (this.ScopeConcept?.Key != value)
                    this.ScopeConcept = this.EntityProvider?.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the concept which identifies the type
        /// </summary>
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("typeConcept"), JsonProperty("typeConcept")]
        public Guid?  TypeConceptKey
        {
            get { return this.TypeConcept?.Key; }
            set
            {
                if (this.TypeConcept?.Key != value)
                    this.TypeConcept = this.EntityProvider?.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Type concept
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(TypeConceptKey))]
		public Concept TypeConcept { get; set; }

        /// <summary>
        /// Gets the scope of the identifier
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(ScopeConceptKey))]
		public Concept ScopeConcept { get; set; }


    }
}