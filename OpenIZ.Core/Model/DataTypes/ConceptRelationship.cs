using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.ComponentModel;
using System.Linq;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a relationship between two concepts
    /// </summary>
    public class ConceptRelationship : VersionBoundRelationData<Concept>
    {

        // Target concept id
        private Guid m_targetConceptId;
        // Target concept
        private Concept m_targetConcept;
        // Relaltionship type id
        private Guid m_relationshipTypeId;
        // Relationship type
        private Concept m_relationshipType;

        /// <summary>
        /// Gets or sets the target concept identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid TargetConceptId
        {
            get { return this.m_targetConceptId; }
            set
            {
                this.m_targetConceptId = value;
                this.m_targetConcept = null;
            }
        }

        /// <summary>
        /// Gets or sets the target concept
        /// </summary>
        public Concept TargetConcept
        {
            get
            {
                if(this.m_targetConcept == null &&
                    this.DelayLoad &&
                    this.m_targetConceptId != Guid.Empty)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
                    this.m_targetConcept = dataPersistence.Get(new Identifier<Guid>(this.m_targetConceptId), null, true);
                }
                return this.m_targetConcept;
            }
            set
            {
                this.m_targetConcept = value;
                if (value != null)
                    this.m_targetConceptId = value.Key;
                else
                    this.m_targetConceptId = Guid.Empty;
            }
        }

        /// <summary>
        /// Relationship type
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid RelationshipTypeId {
            get { return this.m_relationshipTypeId; }
            set
            {
                this.m_relationshipTypeId = value;
                this.m_relationshipType = null;
            }
        }

        /// <summary>
        /// Gets or sets the relationship type
        /// </summary>
        public Concept RelationshipType
        {
            get
            {
                if(this.m_relationshipType == null &&
                    this.DelayLoad &&
                    this.m_relationshipTypeId != Guid.Empty)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
                    this.m_relationshipType = dataPersistence.Get(new Identifier<Guid>(this.m_relationshipTypeId), null, true);
                }
                return this.m_relationshipType;
            }
            set
            {
                this.m_relationshipType = value;
                if (value == null)
                    this.m_relationshipTypeId = Guid.Empty;
                else
                    this.m_relationshipTypeId = value.Key;
            }
        }
    }
}