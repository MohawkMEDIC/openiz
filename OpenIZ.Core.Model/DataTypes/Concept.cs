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
 * Date: 2016-7-16
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
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
        /// Creates a new concept
        /// </summary>
        public Concept()
        {
            this.ReferenceTerms = new List<ConceptReferenceTerm>();
            this.ConceptNames = new List<ConceptName>();
            this.Relationship = new List<ConceptRelationship>();
            this.ConceptSets = new List<ConceptSet>();
        }

        // Concept class id
        private Guid? m_classId;
        // Concept class
        private ConceptClass m_class;
        // Status id
        private Guid? m_conceptStatusId;
        // Status
        private Concept m_conceptStatus;
        // Concept sets
        private List<ConceptSet> m_conceptSets;

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
        [Binding(typeof(StatusKeys))]
        public Guid?  StatusConceptKey
        {
            get
            {
                return this.m_conceptStatusId;
            }
            set
            {
                this.m_conceptStatusId = value;
                this.m_conceptStatus = null;
            }
        }

        /// <summary>
        /// Gets or sets the status of the concept
        /// </summary>
        [SerializationReference(nameof(StatusConceptKey))]
        [XmlIgnore, JsonIgnore]
        public Concept StatusConcept
        {
            get
            {
                this.m_conceptStatus = base.DelayLoad(this.m_conceptStatusId, this.m_conceptStatus);
                return this.m_conceptStatus;
            }
            set
            {
                this.m_conceptStatus = value;
                this.m_conceptStatusId = value?.Key;
            }
        }



        /// <summary>
        /// Gets a list of concept relationships
        /// </summary>
        [AutoLoad, XmlElement("relationship"), JsonProperty("relationship")]
        public List<ConceptRelationship> Relationship { get; set; }

        /// <summary>
        /// True if concept is empty
        /// </summary>
        /// <returns></returns>
        public override bool IsEmpty()
        {
            return String.IsNullOrEmpty(this.Mnemonic);
        }

        /// <summary>
        /// Gets or sets the class identifier
        /// </summary>

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("conceptClass"), JsonProperty("conceptClass")]
        [Binding(typeof(ConceptClassKeys))]
        public Guid?  ClassKey { get
            {
                return this.m_classId;
            }
            set
            {
                this.m_classId = value;
                this.m_class = null;
            }
        }

        /// <summary>
        /// Gets or sets the classification of the concept
        /// </summary>
        [SerializationReference(nameof(ClassKey))]
        [AutoLoad, XmlIgnore, JsonIgnore]
        public ConceptClass Class
        {
            get
            {
                this.m_class = base.DelayLoad(this.m_classId, this.m_class);
                return this.m_class;
            }
            set
            {
                this.m_class = value;
                this.m_classId = value?.Key;
            }
        }

        /// <summary>
        /// Gets a list of concept reference terms
        /// </summary>
        
        [AutoLoad, XmlElement("referenceTerm"), JsonProperty("referenceTerm")]
        public List<ConceptReferenceTerm> ReferenceTerms { get; set; }

        /// <summary>
        /// Gets the concept names
        /// </summary>
        //
        [AutoLoad, XmlElement("name"), JsonProperty("name")]
        public List<ConceptName> ConceptNames { get; set; }

        /// <summary>
        /// Concept sets as identifiers for XML purposes only
        /// </summary>
        [XmlElement("conceptSet"), JsonProperty("conceptSet")]
        public List<Guid> ConceptSetsXml { get; set; }

        /// <summary>
        /// Gets concept sets to which this concept is a member
        /// </summary>
        [DataIgnore, XmlIgnore, JsonIgnore, SerializationReference(nameof(ConceptSetsXml))]
        public List<ConceptSet> ConceptSets
        {
            get
            {
                
                if(this.m_conceptSets == null)
                    this.m_conceptSets = this.ConceptSetsXml?.Select(o=>EntitySource.Current.Get<ConceptSet>(o)).ToList();
                return this.m_conceptSets;
            }
            set
            {

                this.ConceptSetsXml = value?.Where(o=>o.Key.HasValue).Select(o => o.Key.Value).ToList();
                this.m_conceptSets = value;
            }
        }

        /// <summary>
        /// Override string
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} [M: {1}]", base.ToString(), this.Mnemonic);
        }

        /// <summary>
        /// Determine equality
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as Concept;
            if (other == null) return false;
            return base.SemanticEquals(obj) && other.Mnemonic == this.Mnemonic &&
                this.ClassKey == other.ClassKey &&
                this.ConceptNames?.SemanticEquals(other.ConceptNames) == true &&
                this.ConceptSets?.SemanticEquals(other.ConceptSets) == true &&
                this.IsSystemConcept == other.IsSystemConcept &&
                this.Relationship?.SemanticEquals(other.Relationship) == true;
        }


        /// <summary>
        /// Represent as a display string
        /// </summary>
        public override string ToDisplay()
        {
            return this.LoadCollection<ConceptName>("ConceptNames")?.FirstOrDefault().Name;
        }
    }
}