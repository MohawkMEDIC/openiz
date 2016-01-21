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
    /// Act relationships
    /// </summary>
    [Serializable]
    [DataContract(Name = "ActRelationship", Namespace = "http://openiz.org/model")]
    public class ActRelationship : VersionBoundRelationData<Act>
    {
        // The entity key
        private Guid m_targetActKey;
        // The target entity
        [NonSerialized]
        private Act m_targetAct;
        // The association type key
        private Guid m_relationshipTypeKey;
        // The association type
        [NonSerialized]
        private Concept m_relationshipType;

        /// <summary>
        /// The target of the association
        /// </summary>
        [DataMember(Name = "targetRef")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid TargetActKey
        {
            get { return this.m_targetActKey; }
            set
            {
                this.m_targetActKey = value;
                this.m_targetAct = null;
            }
        }

        /// <summary>
        /// Target act reference
        /// </summary>
        [DelayLoad]
        [IgnoreDataMember]
        public Act TargetAct
        {
            get
            {
                this.m_targetAct = base.DelayLoad(this.m_targetActKey, this.m_targetAct);
                return this.m_targetAct;
            }
            set
            {
                this.m_targetAct = value;
                if (value == null)
                    this.m_targetActKey = Guid.Empty;
                else
                    this.m_targetActKey = value.Key;
            }
        }

        /// <summary>
        /// Association type key
        /// </summary>
        [DataMember(Name = "relationshipTypeRef")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid RelationshipTypeKey
        {
            get { return this.m_relationshipTypeKey; }
            set
            {
                this.m_relationshipTypeKey = value;
                this.m_relationshipType = null;
            }
        }

        /// <summary>
        /// Gets or sets the association type
        /// </summary>
        [IgnoreDataMember]
        [DelayLoad]
        public Concept RelationshipType
        {
            get
            {
                this.m_relationshipType = base.DelayLoad(this.m_relationshipTypeKey, this.m_relationshipType);
                return this.m_relationshipType;
            }
            set
            {
                this.m_relationshipType = value;
                if (value == null)
                    this.m_relationshipTypeKey = Guid.Empty;
                else
                    this.m_relationshipTypeKey = value.Key;
            }
        }

  
    }
}
