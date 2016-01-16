using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using System;
using System.ComponentModel;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a reference term relationship between a concept and reference term
    /// </summary>
    public class ConceptReferenceTerm : VersionBoundRelationData<Concept>
    {
        // Reference term id
        private Guid m_referenceTermId;
        // Reference term
        private ReferenceTerm m_referenceTerm;
        // ConceptRelationship type
        private Guid m_relationshipTypeId;
        // Relationship type
        private ConceptRelationshipType m_relationshipType;

        /// <summary>
        /// Gets or sets the reference term identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid ReferenceTermId {
            get { return this.m_referenceTermId; }
            set
            {
                this.m_referenceTerm = null;
                this.m_referenceTermId = value;
            }
        }

        /// <summary>
        /// Gets or set the reference term
        /// </summary>
        [DelayLoad]
        public ReferenceTerm ReferenceTerm
        {
            get
            {
                if (this.m_referenceTerm == null &&
                    this.DelayLoad &&
                    this.m_referenceTermId != Guid.Empty)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTerm>>();
                    this.m_referenceTerm = dataPersistence.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(this.m_referenceTermId), null, true);
                }
                return this.m_referenceTerm;

            }
            set
            {
                this.m_referenceTerm = value;
                if (value == null)
                    this.m_referenceTermId = Guid.Empty;
                else
                    this.m_referenceTermId = value.Key;
            }
        }

        /// <summary>
        /// Gets or sets the relationship type identifier
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
        [DelayLoad]
        public ConceptRelationshipType RelationshipType {
            get
            {
                if(this.m_relationshipType == null &&
                    this.DelayLoad &&
                    this.m_relationshipTypeId != Guid.Empty)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptRelationshipType>>();
                    this.m_relationshipType = dataPersistence.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(this.m_relationshipTypeId), null, true);
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