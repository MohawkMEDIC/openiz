using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// A class representing a generic concept used in the OpenIZ datamodel
    /// </summary>
    public class Concept : VersionedEntityData
    {
        // Concept class id
        private Guid m_classId;
        // Backing field for relationships
        private List<ConceptRelationship> m_relationships;
        // Concept class
        private ConceptClass m_class;
        // Reference terms
        private List<ConceptReferenceTerm> m_referenceTerms;

        /// <summary>
        /// Gets or sets an indicator which dictates whether the concept is a system concept
        /// </summary>
        public bool IsSystemConcept { get; set; }
        /// <summary>
        /// Gets or sets the unchanging mnemonic for the concept
        /// </summary>
        public String Mnemonic { get; set; }
        /// <summary>
        /// Gets a list of concept relationships
        /// </summary>
        public List<ConceptRelationship> Relationship
        {
            get
            {
                if(this.m_relationships == null && this.DelayLoad)
                {
                    var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptRelationship>>();
                    this.m_relationships = persistenceService.Query(r => r.EffectiveVersion.Key == this.Key && r.EffectiveVersion.CreationTime <= this.CreationTime && (r.ObsoleteVersion.ObsoletionTime == null || r.ObsoleteVersion.ObsoletionTime <= this.CreationTime), null).ToList();
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
        public List<ConceptReferenceTerm> ReferenceTerms
        {
            get
            {
                if(this.m_referenceTerms == null && this.DelayLoad)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();
                    this.m_referenceTerms = dataPersistence.Query(r => r.EffectiveVersion.Key == this.Key && r.EffectiveVersion.CreationTime <= this.CreationTime && (r.ObsoleteVersion.ObsoletionTime == null || r.ObsoleteVersion.ObsoletionTime <= this.CreationTime), null).ToList();
                }
                return this.m_referenceTerms;
            }
        }
    }
}