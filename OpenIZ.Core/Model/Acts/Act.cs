using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents the base class for an act
    /// </summary>
    [Serializable]
    [DataContract(Namespace ="http://openiz.org/model", Name ="Act")]
    [Resource(ModelScope.Clinical)]
    public class Act : VersionedEntityData<Act>
    {

        private Guid m_classConceptKey;
        private Guid m_typeConceptKey;
        private Guid m_statusConceptKey;
        private Guid m_moodConceptKey;
        private Guid? m_reasonConceptKey;
        
        private Concept m_classConcept;
        private Concept m_typeConcept;
        private Concept m_statusConcept;
        private Concept m_moodConcept;
        private Concept m_reasonConcept;

        
        private List<ActRelationship> m_relationships;
        private List<ActNote> m_notes;
        private List<ActTag> m_tags;
        private List<ActExtension> m_extensions;
        private List<ActIdentifier> m_identifiers;
        private List<ActParticipation> m_participations;
        
        /// <summary>
        /// Gets or sets an indicator which identifies whether the object is negated
        /// </summary>
        [DataMember(Name = "isNegated")]
        public Boolean IsNegated { get; set; }

        /// <summary>
        /// Gets or sets the start time of the act
        /// </summary>
        [DataMember(Name = "startTime")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the stop time of the act
        /// </summary>
        [DataMember(Name = "stopTime")]
        public DateTime? StopTime { get; set; }

        /// <summary>
        /// Class concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "classConcept")]
        public virtual Guid ClassConceptKey
        {
            get { return this.m_classConceptKey; }
            set
            {
                this.m_classConceptKey = value;
                this.m_classConcept = null;
            }
        }

        /// <summary>
        /// Mood concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "moodConcept")]
        public virtual Guid MoodConceptKey
        {
            get { return this.m_moodConceptKey; }
            set
            {
                this.m_moodConceptKey = value;
                this.m_moodConcept = null;
            }
        }


        /// <summary>
        /// Reason concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "reasonConcept")]
        public Guid? ReasonConceptKey
        {
            get { return this.m_reasonConceptKey; }
            set
            {
                this.m_reasonConceptKey = value;
                this.m_reasonConcept = null;
            }
        }

        /// <summary>
        /// Status concept id
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "statusConcept")]
        public Guid StatusConceptKey
        {
            get { return this.m_statusConceptKey; }
            set
            {
                this.m_statusConceptKey = value;
                this.m_statusConcept = null;
            }
        }

        /// <summary>
        /// Type concept identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "typeConcept")]
        public Guid TypeConceptKey
        {
            get { return this.m_typeConceptKey; }
            set
            {
                this.m_typeConceptKey = value;
                this.m_typeConcept = null;
            }
        }


        /// <summary>
        /// Class concept datal load property
        /// </summary>
        [IgnoreDataMember]
        [DelayLoad(nameof(ClassConceptKey))]
        public Concept ClassConcept
        {
            get
            {
                this.m_classConcept = base.DelayLoad(this.m_classConceptKey, this.m_classConcept);
                return this.m_classConcept;
            }
        }

        /// <summary>
        /// Mood concept data load property
        /// </summary>
        [IgnoreDataMember]
        [DelayLoad(nameof(MoodConceptKey))]
        public Concept MoodConcept
        {
            get
            {
                this.m_moodConcept = base.DelayLoad(this.m_moodConceptKey, this.m_moodConcept);
                return this.m_moodConcept;
            }
            set
            {
                this.m_moodConcept = value;
                if (value == null)
                    this.m_moodConceptKey = Guid.Empty;
                else
                    this.m_moodConceptKey = value.Key;
            }
        }

        /// <summary>
        /// Mood concept data load property
        /// </summary>
        [IgnoreDataMember]
        [DelayLoad(nameof(ReasonConceptKey))]
        public Concept ReasonConcept
        {
            get
            {
                this.m_reasonConcept = base.DelayLoad(this.m_reasonConceptKey, this.m_reasonConcept);
                return this.m_reasonConcept;
            }
            set
            {
                this.m_reasonConcept = value;
                this.m_reasonConceptKey = value?.Key;
            }
        }

        /// <summary>
        /// Status concept id
        /// </summary>
        [DelayLoad(nameof(StatusConceptKey))]
        [IgnoreDataMember]
        public Concept StatusConcept
        {
            get
            {
                this.m_statusConcept = base.DelayLoad(this.m_statusConceptKey, this.m_statusConcept);
                return this.m_statusConcept;
            }
            set
            {
                this.m_statusConcept = value;
                if (value == null)
                    this.m_statusConceptKey = Guid.Empty;
                else
                    this.m_statusConceptKey = value.Key;
            }
        }

        /// <summary>
        /// Type concept identifier
        /// </summary>
        [DelayLoad(nameof(TypeConceptKey))]
        [IgnoreDataMember]
        public Concept TypeConcept
        {
            get
            {
                this.m_typeConcept = base.DelayLoad(this.m_typeConceptKey, this.m_typeConcept);
                return this.m_typeConcept;
            }
            set
            {
                this.m_typeConcept = value;
                if (value == null)
                    this.m_typeConceptKey = Guid.Empty;
                else
                    this.m_typeConceptKey = value.Key;
            }
        }

        /// <summary>
        /// Gets the identifiers associated with this act
        /// </summary>
        [DelayLoad(null)]
        [DataMember(Name = "identifier")]
        public List<ActIdentifier> Identifiers
        {
            get
            {
                if (this.m_identifiers == null && this.IsDelayLoadEnabled)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<ActIdentifier>>();
                    this.m_identifiers = dataPersistence.Query(o => o.SourceEntityKey == this.Key && this.VersionSequence >= o.EffectiveVersionSequenceId && (o.ObsoleteVersionSequenceId == null || this.VersionSequence < o.ObsoleteVersionSequenceId), null).ToList();
                }
                return this.m_identifiers;
            }
        }

        /// <summary>
        /// Gets a list of all associated acts for this act
        /// </summary>
        [DelayLoad(null)]
        [DataMember(Name = "relationship")]
        public List<ActRelationship> Relationships
        {
            get
            {
                if (this.m_relationships == null && this.IsDelayLoadEnabled)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<ActRelationship>>();
                    this.m_relationships = dataPersistence.Query(o => o.SourceEntityKey == this.Key && this.VersionSequence >= o.EffectiveVersionSequenceId && (o.ObsoleteVersionSequenceId == null || this.VersionSequence < o.ObsoleteVersionSequenceId), null).ToList();
                }
                return this.m_relationships;
            }
        }

        /// <summary>
        /// Gets a list of all extensions associated with the act
        /// </summary>
        [DelayLoad(null)]
        [DataMember(Name= "extension")]
        public List<ActExtension> Extensions
        {
            get
            {
                if (this.m_extensions == null && this.IsDelayLoadEnabled)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<ActExtension>>();
                    this.m_extensions = dataPersistence.Query(o => o.SourceEntityKey == this.Key && this.VersionSequence >= o.EffectiveVersionSequenceId && (o.ObsoleteVersionSequenceId == null || this.VersionSequence < o.ObsoleteVersionSequenceId), null).ToList();
                }
                return this.m_extensions;
            }
        }

        /// <summary>
        /// Gets a list of all notes associated with the act
        /// </summary>
        [DelayLoad(null)]
        [DataMember(Name = "note")]
        public List<ActNote> Notes
        {
            get
            {
                if (this.m_notes == null && this.IsDelayLoadEnabled)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<ActNote>>();
                    this.m_notes = dataPersistence.Query(o => o.SourceEntityKey == this.Key && this.VersionSequence >= o.EffectiveVersionSequenceId && (o.ObsoleteVersionSequenceId == null || this.VersionSequence < o.ObsoleteVersionSequenceId), null).ToList();
                }
                return this.m_notes;
            }
        }

        /// <summary>
        /// Gets a list of all tags associated with the act
        /// </summary>
        [DelayLoad(null)]
        [DataMember(Name = "tag")]
        public List<ActTag> Tags
        {
            get
            {
                if (this.m_tags == null && this.IsDelayLoadEnabled)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<ActTag>>();
                    this.m_tags = dataPersistence.Query(o => o.SourceEntityKey == this.Key && o.ObsoletionTime == null, null).ToList();
                }
                return this.m_tags;
            }
        }

        /// <summary>
        /// Forces the delay load properties in this type to reload
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_moodConcept = this.m_reasonConcept  = this.m_classConcept = this.m_statusConcept = this.m_typeConcept = null;
            this.m_relationships = null;
            this.m_extensions = null;
            this.m_identifiers = null;
            this.m_notes = null;
            this.m_tags = null;
            this.m_participations = null;
            this.m_reasonConcept = null;
        }


    }
}
