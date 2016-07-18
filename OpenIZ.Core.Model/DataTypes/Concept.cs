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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// A class representing a generic concept used in the OpenIZ datamodel
    /// </summary>
    
    [XmlType("Concept",  Namespace = "http://openiz.org/model"), JsonObject("Concept")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Concept")]
    [Classifier(nameof(Mnemonic)), KeyLookup(nameof(Mnemonic))]
    public class Concept : VersionedEntityData<Concept>
    {

        /// <summary>
        /// Ctor
        /// </summary>
        public Concept()
        {
            this.ConceptNames = new List<ConceptName>();
            this.ConceptSets = new List<ConceptSet>();
            this.ReferenceTerms = new List<ConceptReferenceTerm>();
            this.Relationship = new List<ConceptRelationship>();
        }

        /// <summary>
        /// Gets or sets an indicator which dictates whether the concept is a system concept
        /// </summary>
        [XmlElement("isReadonly"), JsonProperty("isReadonly")]
        public bool IsSystemConcept { get; set; }
        /// <summary>
        /// Gets or sets the unchanging mnemonic for the concept
        /// </summary>
        [XmlElement("mnemonic"), JsonProperty("mnemonic")]
        public String Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the status concept key
        /// </summary>
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("statusConcept"), JsonProperty("statusConcept")]
        public Guid? StatusConceptKey { get; set; }
       

        /// <summary>
        /// Gets or sets the status of the concept
        /// </summary>
        [DataIgnore, XmlIgnore, JsonIgnore, SerializationReference(nameof(StatusConceptKey))]
		public Concept StatusConcept
        {
            get
            {
                if(this.StatusConceptKey.HasValue)
                    return EntitySource.Current.Provider.Get<Concept>(this.StatusConceptKey);
                return null;
            }
            set
            {
                this.StatusConceptKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets a list of concept relationships
        /// </summary>
        [XmlElement("relationship"), JsonProperty("relationship")]
        [AutoLoad]
        public List<ConceptRelationship> Relationship { get; set; }

        /// <summary>
        /// Gets or sets the class identifier
        /// </summary>

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("conceptClass"), JsonProperty("conceptClass")]
        public Guid? ClassKey
        {
            get
            {
                return this.Class?.Key;
            }
            set
            {
                if (this.Class?.Key != value)
                    this.Class = this.EntityProvider?.Get<ConceptClass>(value);
            }
        }

        /// <summary>
        /// Gets or sets the classification of the concept
        /// </summary>
        [AutoLoad]
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(ClassKey))]
		public ConceptClass Class { get; set; }

        /// <summary>
        /// Gets a list of concept reference terms
        /// </summary>
        [XmlElement("referenceTerm"), JsonProperty("referenceTerm")]
        [AutoLoad]
        public List<ConceptReferenceTerm> ReferenceTerms { get; set; }

        /// <summary>
        /// Gets the concept names
        /// </summary>
        //[DelayLoad(null)]
        [XmlElement("name"), JsonProperty("name")]
        [AutoLoad]
        public List<ConceptName> ConceptNames { get; set; }

        /// <summary>
        /// Concept sets as identifiers for XML purposes only
        /// </summary>
        [DataIgnore, XmlElement("conceptSet"), JsonProperty("conceptSet")]
        //[Bundle(nameof(ConceptSets))]
        public List<Guid> ConceptSetsXml
        {
            get
            {
                return this.ConceptSets?.Select(o => o.Key.Value).ToList();
            }
            set
            {
                this.ConceptSets = value?.Select(o => this.EntityProvider?.Get<ConceptSet>(o)).ToList();
            }
        }

        /// <summary>
        /// Gets concept sets to which this concept is a member
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(ConceptSetsXml))]
		public List<ConceptSet> ConceptSets { get; set; }


    }
}