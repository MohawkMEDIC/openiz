using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
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
    /// Associates an entity which participates in an act
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "http://openiz.org/model", Name ="ActParticipation")]
    public class ActParticipation : VersionBoundRelationData<Act>
    {

        private Guid m_targetEntityKey;
        [NonSerialized]
        private Entity m_targetEntity;
        private Guid m_participationRoleKey;
        [NonSerialized]
        private Concept m_participationRole;

        /// <summary>
        /// Gets or sets the target entity reference
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "targetEntityRef")]
        public Guid TargetEntityKey
        {
            get { return this.m_targetEntityKey; }
            set
            {
                this.m_targetEntityKey = value;
                this.m_targetEntity = null;
            }
        }

        /// <summary>
        /// Gets or sets the participation role key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "participationRoleRef")]
        public Guid ParticipationRoleKey
        {
            get { return this.m_participationRoleKey; }
            set
            {
                this.m_participationRoleKey = value;
                this.m_participationRole = null;
            }
        }

        /// <summary>
        /// Gets or sets the entity which participated in the act
        /// </summary>
        [IgnoreDataMember]
        [DelayLoad]
        public Entity TargetEntity
        {
            get
            {
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
        /// Gets or sets the role that the entity played in participating in the act
        /// </summary>
        [IgnoreDataMember]
        [DelayLoad]
        public Concept ParticipationRole
        {
            get
            {
                this.m_participationRole = base.DelayLoad(this.m_participationRoleKey, this.m_participationRole);
                return this.m_participationRole;
            }
            set
            {
                this.m_participationRole = value;
                if (value == null)
                    this.m_participationRoleKey = Guid.Empty;
                else
                    this.m_participationRoleKey = value.Key;
            }
        }
    }
}
