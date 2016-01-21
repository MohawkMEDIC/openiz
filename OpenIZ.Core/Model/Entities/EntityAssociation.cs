using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents an association between two entities
    /// </summary>
    [Serializable]
    [DataContract(Name = "EntityAssociation", Namespace = "http://openiz.org/model")]
    public class EntityAssociation : VersionBoundRelationData<Entity>
    {

        // The entity key
        private Guid m_targetEntityKey;
        // The target entity
        [NonSerialized]
        private Entity m_targetEntity;
        // The association type key
        private Guid m_associationTypeKey;
        // The association type
        [NonSerialized]
        private Concept m_associationType;

        /// <summary>
        /// The target of the association
        /// </summary>
        [DataMember(Name = "targetRef")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid TargetEntityKey
        {
            get { return this.m_targetEntityKey; }
            set { this.m_targetEntityKey = value;
                this.m_targetEntity = null;
            }
        }

        /// <summary>
        /// Target entity reference
        /// </summary>
        [DelayLoad]
        [IgnoreDataMember]
        public Entity TargetEntity
        {
            get {
                this.m_targetEntity = base.DelayLoad(this.m_targetEntityKey, this.m_targetEntity);
                return this.m_targetEntity;
            }
            set
            {
                this.m_targetEntity = value;
                if (value == null)
                    this.m_targetEntityKey = Guid.Empty;
                else
                    this.m_targetEntityKey = value.Key;
            }
        }

        /// <summary>
        /// Association type key
        /// </summary>
        [DataMember(Name = "associationTypeRef")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid AssociationTypeKey
        {
            get { return this.m_associationTypeKey; }
            set
            {
                this.m_associationTypeKey = value;
                this.m_associationType = null;
            }
        }

        /// <summary>
        /// Gets or sets the association type
        /// </summary>
        [IgnoreDataMember]
        [DelayLoad]
        public Concept AssociationType
        {
            get {
                this.m_associationType = base.DelayLoad(this.m_associationTypeKey, this.m_associationType);
                return this.m_associationType;
            }
            set
            {
                this.m_associationType = value;
                if (value == null)
                    this.m_associationTypeKey = Guid.Empty;
                else
                    this.m_associationTypeKey = value.Key;
            }
        }
    }
}