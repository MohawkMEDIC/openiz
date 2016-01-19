using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// A generic class representing components of a larger item (i.e. address, name, etc);
    /// </summary>
    /// <typeparam name="TBoundModel"></typeparam>
    [Serializable]
    [DataContract(Name = "GenericComponentValues", Namespace = "http://openiz.org/model")]
    public abstract class GenericComponentValues<TBoundModel> : BoundRelationData<TBoundModel> where TBoundModel : IdentifiedData
    {
        // Component type
        private Guid m_componentTypeKey;
        // Component type
        [NonSerialized]
        private Concept m_componentType;

        /// <summary>
        /// Gets or sets the value of the component
        /// </summary>
        [DataMember(Name = "value")]
        public String Value { get; set; }

        /// <summary>
        /// Component type key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "typeRef")]
        public Guid TypeKey
        {
            get { return this.m_componentTypeKey; }
            set
            {
                this.m_componentTypeKey = value;
                this.m_componentType = null;
            }
        }

        /// <summary>
        /// Gets or sets the type of address component
        /// </summary>
        [IgnoreDataMember]
        [DelayLoad]
        public Concept Type
        {
            get { return base.DelayLoad(this.m_componentTypeKey, this.m_componentType); }
            set
            {
                this.m_componentType = value;
                if (value == null)
                    this.m_componentTypeKey = Guid.Empty;
                else
                    this.m_componentTypeKey = value.Key;
            }
        }
    }
}