using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// A class representing a generic concept used in the OpenIZ datamodel
    /// </summary>
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
        // Previous version id
        private Guid? m_previousVersionId;
        // Previous version
        private Concept m_previousVersion;
        // Status id
        private Guid? m_conceptStatusId;
        // Status
        private Concept m_conceptStatus;

        /// <summary>
        /// Gets or sets an indicator which dictates whether the concept is a system concept
        /// </summary>
        public bool IsSystemConcept { get; set; }
        /// <summary>
        /// Gets or sets the unchanging mnemonic for the concept
        /// </summary>
        public String Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the status concept key
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid? StatusConceptId
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
        [DelayLoad]
        public Concept Status
        {
            get
            {
                if (this.m_conceptStatus == null &&
                    this.DelayLoad &&
                    this.m_conceptStatusId.HasValue)
                {
                    var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
                    this.m_conceptStatus = persistenceService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(this.m_conceptStatusId.Value), null, true);
                }
                return this.m_conceptStatus;
            }
            set
            {
                this.m_previousVersion = value;
                this.m_conceptStatusId = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the previous version key
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Guid? PreviousVersionKey
        {
            get
            {
                return this.m_previousVersionId;
            }
            set
            {
                this.m_previousVersionId = value;
                this.m_previousVersion = null;
            }
        }

        /// <summary>
        /// Gets or sets the previous version
        /// </summary>
        [DelayLoad]
        public override Concept PreviousVersion
        {
            get
            {
                if(this.m_previousVersion == null &&
                    this.DelayLoad &&
                    this.m_previousVersionId.HasValue)
                {
                    var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
                    this.m_previousVersion = persistenceService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(this.Key, this.m_previousVersionId.Value), null, true);
                }
                return this.m_previousVersion;
            }
            set
            {
                this.m_previousVersion = value;
                this.m_previousVersionId = value?.VersionKey;
            }
        }

        /// <summary>
        /// Gets a list of concept relationships
        /// </summary>
        [DelayLoad]
        public List<ConceptRelationship> Relationship
        {
            get
            {
                if(this.m_relationships == null && this.DelayLoad)
                {
                    var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptRelationship>>();
                    this.m_relationships = persistenceService.Query(r => this.Key == r.TargetEntityKey && this.VersionSequence >= r.EffectiveVersionSequenceId && (r.ObsoleteVersionSequenceId == null || this.VersionSequence <= r.ObsoleteVersionSequenceId), null).ToList();
                }
                return this.m_relationships;
            }
        }

        /// <summary>
        /// Gets or sets the class identifier
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid ClassId
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
        [DelayLoad]
        public ConceptClass Class
        {
            get
            {
                if(this.m_class == null &&
                    this.DelayLoad &&
                    this.m_classId != Guid.Empty)
                {
                    var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptClass>>();
                    this.m_class = persistenceService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(this.m_classId), null, true);
                }
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
        [DelayLoad]
        public List<ConceptReferenceTerm> ReferenceTerms
        {
            get
            {
                if(this.m_referenceTerms == null && this.DelayLoad)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();
                    this.m_referenceTerms = dataPersistence.Query(r => this.Key == r.TargetEntityKey && this.VersionSequence >= r.EffectiveVersionSequenceId && (r.ObsoleteVersionSequenceId == null || this.VersionSequence <= r.ObsoleteVersionSequenceId) , null).ToList();
                }
                return this.m_referenceTerms;
            }
        }

        /// <summary>
        /// Gets the concept names
        /// </summary>
        [DelayLoad]
        public List<ConceptName> ConceptNames
        {
            get
            {
                if(this.m_conceptNames == null && this.m_delayLoad)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptName>>();
                    this.m_conceptNames = dataPersistence.Query(o => o.TargetEntityKey == this.Key && this.VersionSequence >= o.EffectiveVersionSequenceId && (o.ObsoleteVersionSequenceId == null || this.VersionSequence <= o.ObsoleteVersionSequenceId), null).ToList();
                }
                return this.m_conceptNames;
            }
        }

    }
}