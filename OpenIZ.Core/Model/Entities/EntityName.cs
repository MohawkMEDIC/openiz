using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Linq;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a name for an entity
    /// </summary>
    [Serializable]
    [DataContract(Name = "EntityName", Namespace = "http://openiz.org/model")]
    public class EntityName : VersionBoundRelationData<Entity>
    {
        // Name use key
        private Guid? m_nameUseKey;
        // Name use concept
        [NonSerialized]
        private Concept m_nameUseConcept;
        // Name components
        [NonSerialized]
        private List<EntityNameComponent> m_nameComponents;

        /// <summary>
        /// Gets or sets the name use key
        /// </summary>
        [DataMember(Name = "nameUseRef")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid? NameUseKey
        {
            get { return this.m_nameUseKey; }
            set
            {
                this.m_nameUseKey = value;
                this.m_nameUseConcept = null;
            }
        }

        /// <summary>
        /// Gets or sets the name use
        /// </summary>
        [DelayLoad]
        [IgnoreDataMember]
        public Concept NameUse
        {
            get { return base.DelayLoad(this.m_nameUseKey, this.m_nameUseConcept); }
            set
            {
                this.m_nameUseConcept = value;
                this.m_nameUseKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the component types
        /// </summary>
        [DelayLoad]
        public List<EntityNameComponent> Component
        {
            get
            {
                if (this.m_nameComponents == null && this.IsDelayLoad)
                {
                    var dataPersistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<EntityNameComponent>>();
                    this.m_nameComponents = dataPersistenceService.Query(o => o.SourceEntityKey == this.Key, null).ToList();
                }
                return this.m_nameComponents;
            }
        }
    }
}