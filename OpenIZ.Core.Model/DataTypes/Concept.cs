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
    [Classifier(nameof(Mnemonic))]
    public class Concept : VersionedEntityData<Concept>
    {

      
        // Concept class id
        private Guid m_classId;
        // Backing field for relationships
        private List<ConceptRelationship> m_relationships;
        // Concept class
        private ConceptClass m_class;
        // Reference terms
        private List<ConceptReferenceTerm> m_referenceTerms;
        // Names
        private List<ConceptName> m_conceptNames;
        // Status id
        private Guid? m_conceptStatusId;
        // Status
        private Concept m_conceptStatus;
        // Concept set
        private List<ConceptSet> m_conceptSet;
        /// <summary>
        /// Gets or sets an indicator which dictates whether the concept is a system concept
        /// </summary>
        [XmlElement("isReadonly"), JsonProperty("isReadonly")]
        public bool IsSystemConcept { get; set; }
        /// <summary>
        /// Gets or sets the unchanging mnemonic for the concept
        /// </summary>
        [XmlElement("mnemonic"), JsonProperty("mnemonic")]
        [Unique]
        public String Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the status concept key
        /// </summary>
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("statusConcept"), JsonProperty("statusConcept")]
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
        [DelayLoad(nameof(StatusConceptKey))]
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
        [DelayLoad(null)]
        [XmlElement("relationship"), JsonProperty("relationship")]
        public List<ConceptRelationship> Relationship
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_relationships = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, this.m_relationships);

                return this.m_relationships;
            }
            set
            {
                this.m_relationships = value;
            }
        }

        /// <summary>
        /// Gets or sets the class identifier
        /// </summary>
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("conceptClass"), JsonProperty("conceptClass")]
        public Guid  ClassKey
        {
            get { return this.m_classId; }
            set
            {
                this.m_classId = value;
                this.m_class = null;
            }
        }

        /// <summary>
        /// Gets or sets the classification of the concept
        /// </summary>
        [DelayLoad(nameof(ClassKey))]
        [AutoLoad]
        [XmlIgnore, JsonIgnore]
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
                if (value == null)
                    this.m_classId = Guid.Empty;
                else
                    this.m_classId = value.Key;
            }
        }

        /// <summary>
        /// Gets a list of concept reference terms
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("referenceTerm"), JsonProperty("referenceTerm")]
        public List<ConceptReferenceTerm> ReferenceTerms
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_referenceTerms = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, this.m_referenceTerms);

                return this.m_referenceTerms;
            }
            set
            {
                this.m_referenceTerms = value;
            }
        }

        /// <summary>
        /// Gets the concept names
        /// </summary>
        //[DelayLoad(null)]
        [XmlElement("name"), JsonProperty("name")]
        public List<ConceptName> ConceptNames
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_conceptNames = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, this.m_conceptNames);

                return this.m_conceptNames;
            }
            set
            {
                this.m_conceptNames = value;
            }
        }

        /// <summary>
        /// Concept sets as identifiers for XML purposes only
        /// </summary>
        [XmlElement("conceptSet"), JsonProperty("conceptSet")]
        [DelayLoad(null)]
        //[Bundle(nameof(ConceptSets))]
        public List<Guid> ConceptSetsXml
        {
            get
            {
                return this.ConceptSets?.Select(o => o.Key).ToList();
            }
            set
            {
                ; // nothing
            }
        }

        /// <summary>
        /// Gets concept sets to which this concept is a member
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [DelayLoad(null)]
        public List<ConceptSet> ConceptSets
        {
            get
            {
                if(this.m_conceptSet == null &&
                    this.IsDelayLoadEnabled)
                    this.m_conceptSet = EntitySource.Current.Provider.Query<ConceptSet>(s => s.Concepts.Any(c => c.Key == this.Key)).ToList();
                return this.m_conceptSet;
            }
            set
            {
                this.m_conceptSet = value;
            }
        }

        /// <summary>
        /// Reference terms
        /// </summary>
        public void SetDelayLoadProperties(List<ConceptName> names, List<ConceptReferenceTerm> referenceTerms)
        {
            this.m_conceptNames = names;
            this.m_referenceTerms = referenceTerms;
        }

        /// <summary>
        /// Refresh the specified object's delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_class = null;
            this.m_conceptNames = null;
            this.m_conceptStatus = null;
            this.m_referenceTerms = null;
            this.m_relationships = null;
        }
    }
}