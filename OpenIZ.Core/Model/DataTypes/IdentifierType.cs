using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using System;
using System.ComponentModel;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a basic information class which classifies the use of an identifier
    /// </summary>
    public class IdentifierType : BaseEntityData
    {

        // Type concept id
        private Guid m_typeConceptId;
        // Scope concept identifier
        private Guid? m_scopeConceptId;
        // Type concept
        private Concept m_typeConcept;
        // Scope concept
        private Concept m_scopeConcept;

        /// <summary>
        /// Gets or sets the id of the scope concept
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid? ScopeConceptId
        {
            get { return this.m_scopeConceptId; }
            set
            {
                this.m_scopeConcept = null;
                this.m_scopeConceptId = value;
            }
        }

        /// <summary>
        /// Gets or sets the concept which identifies the type
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid TypeConceptId
        {
            get { return this.m_typeConceptId; }
            set
            {
                this.m_typeConcept = null;
                this.m_typeConceptId = value;
            }
        }

        /// <summary>
        /// Type concept
        /// </summary>
        [DelayLoad]
        public Concept TypeConcept
        {
            get
            {
                if(this.m_typeConcept == null && this.DelayLoad)
                {
                    var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
                    this.m_typeConcept = persistenceService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(this.m_typeConceptId), null, true);
                }
                return this.m_typeConcept;
            }
            set
            {
                this.m_typeConcept = value;
                if (value == null)
                    this.m_typeConceptId = Guid.Empty;
                else
                    this.m_typeConceptId = value.Key;
            }
        }

        /// <summary>
        /// Gets the scope of the identifier
        /// </summary>
        [DelayLoad]
        public Concept Scope
        {
            get
            {
                if (this.m_scopeConcept == null && this.m_scopeConceptId.HasValue && this.DelayLoad)
                {
                    var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
                    this.m_scopeConcept = persistenceService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(this.m_scopeConceptId.Value), null, true);
                }
                return this.m_scopeConcept;
            }
            set
            {
                this.m_scopeConcept = value;
                this.m_scopeConceptId = value?.Key;
            }
        }


    }
}