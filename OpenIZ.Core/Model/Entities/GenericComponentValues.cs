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
        
        private Concept m_componentType;

        /// <summary>
        /// Component type key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "type")]
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
        [DelayLoad(nameof(TypeKey))]
        public Concept Type
        {
            get {
                this.m_componentType = base.DelayLoad(this.m_componentTypeKey, this.m_componentType);
                return this.m_componentType;
            }
            set
            {
                this.m_componentType = value;
                if (value == null)
                    this.m_componentTypeKey = Guid.Empty;
                else
                    this.m_componentTypeKey = value.Key;
            }
        }

        /// <summary>
        /// Forces refreshing of delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_componentType = null;
        }
    }
}