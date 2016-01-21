using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents an entity telecom address
    /// </summary>
    [Serializable]
    [DataContract(Name = "EntityTelecomAddress", Namespace = "http://openiz.org/model")]
    public class EntityTelecomAddress : VersionBoundRelationData<Entity>
    {

        // Name use key
        private Guid? m_nameUseKey;
        // Name use concept
        [NonSerialized]
        private Concept m_nameUseConcept;

        /// <summary>
        /// Gets or sets the name use key
        /// </summary>
        [DataMember(Name = "addressUseRef")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid? AddressUseKey
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
        public Concept AddressUse
        {
            get {
                this.m_nameUseConcept = base.DelayLoad(this.m_nameUseKey, this.m_nameUseConcept);
                return this.m_nameUseConcept;
            }
            set
            {
                this.m_nameUseConcept = value;
                this.m_nameUseKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the value of the telecom address
        /// </summary>
        [DataMember(Name = "value")]
        public String Value { get; set; }



    }
}