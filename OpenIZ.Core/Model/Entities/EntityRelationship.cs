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
    [DataContract(Name = "EntityRelationship", Namespace = "http://openiz.org/model")]
    public class EntityRelationship : VersionBoundRelationData<Entity>
    {

        // The entity key
        private Guid m_targetEntityKey;
        // The target entity
        
        private Entity m_targetEntity;
        // The association type key
        private Guid m_associationTypeKey;
        // The association type
        
        private Concept m_relationshipType;

        /// <summary>
        /// The target of the association
        /// </summary>
        [DataMember(Name = "target")]
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
        [DelayLoad(nameof(TargetEntityKey))]
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
        [DataMember(Name = "relationshipType")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid RelationshipTypeKey
        {
            get { return this.m_associationTypeKey; }
            set
            {
                this.m_associationTypeKey = value;
                this.m_relationshipType = null;
            }
        }

        /// <summary>
        /// Gets or sets the association type
        /// </summary>
        [IgnoreDataMember]
        [DelayLoad(nameof(RelationshipTypeKey))]
        public Concept RelationshipType
        {
            get {
                this.m_relationshipType = base.DelayLoad(this.m_associationTypeKey, this.m_relationshipType);
                return this.m_relationshipType;
            }
            set
            {
                this.m_relationshipType = value;
                if (value == null)
                    this.m_associationTypeKey = Guid.Empty;
                else
                    this.m_associationTypeKey = value.Key;
            }
        }
    }
}