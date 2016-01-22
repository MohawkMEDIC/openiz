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
 * Date: 2016-1-19
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// A class representing a generic concept used in the OpenIZ datamodel
    /// </summary>
    [Serializable]
    [DataContract(Name = "Concept", Namespace = "http://openiz.org/model")]
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

        /// <summary>
        /// Gets or sets an indicator which dictates whether the concept is a system concept
        /// </summary>
        [DataMember(Name = "isReadonly")]
        public bool IsSystemConcept { get; set; }
        /// <summary>
        /// Gets or sets the unchanging mnemonic for the concept
        /// </summary>
        [DataMember(Name = "mnemonic")]
        public String Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the status concept key
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataMember(Name = "statusConcept")]
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
        [IgnoreDataMember]
        public Concept Status
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
        [IgnoreDataMember]
        public List<ConceptRelationship> Relationship
        {
            get
            {
                if(this.m_relationships == null && this.IsDelayLoadEnabled)
                {
                    var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptRelationship>>();
                    this.m_relationships = persistenceService.Query(r => this.Key == r.SourceEntityKey && this.VersionSequence >= r.EffectiveVersionSequenceId && (r.ObsoleteVersionSequenceId == null || this.VersionSequence < r.ObsoleteVersionSequenceId), null).ToList();
                }
                return this.m_relationships;
            }
        }

        /// <summary>
        /// Gets or sets the class identifier
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataMember(Name = "class")]
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
        [IgnoreDataMember]
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
        [IgnoreDataMember]
        public List<ConceptReferenceTerm> ReferenceTerms
        {
            get
            {
                if(this.m_referenceTerms == null && this.IsDelayLoadEnabled)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();
                    this.m_referenceTerms = dataPersistence.Query(r => this.Key == r.SourceEntityKey && this.VersionSequence >= r.EffectiveVersionSequenceId && (r.ObsoleteVersionSequenceId == null || this.VersionSequence < r.ObsoleteVersionSequenceId) , null).ToList();
                }
                return this.m_referenceTerms;
            }
        }

        /// <summary>
        /// Gets the concept names
        /// </summary>
        [DelayLoad(null)]
        [IgnoreDataMember]
        public List<ConceptName> ConceptNames
        {
            get
            {
                if(this.m_conceptNames == null && this.IsDelayLoadEnabled)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptName>>();
                    this.m_conceptNames = dataPersistence.Query(o => o.SourceEntityKey == this.Key && this.VersionSequence >= o.EffectiveVersionSequenceId && (o.ObsoleteVersionSequenceId == null || this.VersionSequence < o.ObsoleteVersionSequenceId), null).ToList();
                }
                return this.m_conceptNames;
            }
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