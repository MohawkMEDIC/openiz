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
    /// Entity address
    /// </summary>
    [Serializable]
    [DataContract(Name = "EntityAddress", Namespace = "http://openiz.org/model")]
    public class EntityAddress : VersionBoundRelationData<Entity>
    {

        // Address use key
        private Guid? m_addressUseKey;
        // Address use concept
        [NonSerialized]
        private Concept m_addressUseConcept;
        // Address components
        [NonSerialized]
        private List<EntityAddressComponent> m_addressComponents;

        /// <summary>
        /// Gets or sets the address use key
        /// </summary>
        [DataMember(Name = "addressUseRef")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid? AddressUseKey
        {
            get { return this.m_addressUseKey; }
            set
            {
                this.m_addressUseKey = value;
                this.m_addressUseConcept = null;
            }
        }

        /// <summary>
        /// Gets or sets the address use
        /// </summary>
        [DelayLoad]
        [IgnoreDataMember]
        public Concept AddressUse
        {
            get {
                this.m_addressUseConcept = base.DelayLoad(this.m_addressUseKey, this.m_addressUseConcept);
                return this.m_addressUseConcept;
            }
            set
            {
                this.m_addressUseConcept = value;
                this.m_addressUseKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the component types
        /// </summary>
        [DelayLoad]
        public List<EntityAddressComponent> Component
        {
            get
            {
                if(this.m_addressComponents == null && this.IsDelayLoad)
                {
                    var dataPersistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<EntityAddressComponent>>();
                    this.m_addressComponents = dataPersistenceService.Query(o => o.SourceEntityKey == this.Key, null).ToList();
                }
                return this.m_addressComponents;
            }
        }
    }
}