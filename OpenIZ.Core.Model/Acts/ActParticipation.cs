/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-7-16
 */
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Associates an entity which participates in an act
    /// </summary>
    /// <remarks>
    /// <para>
    /// An act participation instance is used to link an <see cref="Entity"/> entity instance to an <see cref="Act"/> act instance. It is said that the
    /// player (<see cref="PlayerEntityKey"/>) participates in the act (<see cref="ActKey"/>) in a particular role (<see cref="ParticipationRoleKey"/>).
    /// </para>
    /// <para>
    /// Act participations can also be quantified. For example, if 100 doses of a particlar material (<see cref="ManufacturedMaterial"/>) were consumed
    /// as part of an act, then the quantity would be 100.
    /// </para>
    /// </remarks>
    [Classifier(nameof(ParticipationRole))]
    [XmlType(Namespace = "http://openiz.org/model", TypeName = "ActParticipation"), JsonObject(nameof(ActParticipation))]
    public class ActParticipation : VersionedAssociation<Act>
    {

        private Guid? m_playerKey;

        private Entity m_player;
        private Guid? m_participationRoleKey;

        private Concept m_participationRole;

        /// <summary>
        /// Default constructor for act participation
        /// </summary>
        public ActParticipation()
        {
        }

		/// <summary>
		/// Act participation relationship between <paramref name="roleType" /> and <paramref name="player" />
		/// </summary>
		/// <param name="roleType">Type of the role.</param>
		/// <param name="player">The player.</param>
		public ActParticipation(Guid? roleType, Entity player)
        {
            this.ParticipationRoleKey = roleType;
            this.PlayerEntity = player;
        }

        /// <summary>
        /// Entity relationship between <paramref name="roleType" /> and <paramref name="playerKey" />
        /// </summary>
        /// <param name="roleType">Type of the role.</param>
        /// <param name="playerKey">The player key.</param>
        public ActParticipation(Guid? roleType, Guid? playerKey)
        {
            this.ParticipationRoleKey = roleType;
            this.PlayerEntityKey = playerKey;
        }

        /// <summary>
        /// Gets or sets the target entity reference
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("player"), JsonProperty("player")]
        public Guid? PlayerEntityKey
        {
            get { return this.m_playerKey; }
            set
            {
                if (this.m_playerKey != value)
                {
                    this.m_playerKey = value;
                    this.m_player = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the participation role key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Binding(typeof(ActParticipationKey))]
        [XmlElement("participationRole"), JsonProperty("participationRole")]
        public Guid? ParticipationRoleKey
        {
            get { return this.m_participationRoleKey; }
            set
            {
                if (this.m_participationRoleKey != value)
                {
                    this.m_participationRoleKey = value;
                    this.m_participationRole = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the entity which participated in the act
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [SerializationReference(nameof(PlayerEntityKey))]
        public Entity PlayerEntity
        {
            get
            {
                this.m_player = base.DelayLoad(this.m_playerKey, this.m_player);
                return this.m_player;
            }
            set
            {
                this.m_player = value;
                this.m_playerKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the role that the entity played in participating in the act
        /// </summary>
        [XmlIgnore, JsonIgnore, AutoLoad]
        [SerializationReference(nameof(ParticipationRoleKey))]
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
                this.m_participationRoleKey = value?.Key;
            }
        }

        /// <summary>
        /// The entity that this relationship targets
        /// </summary>
        [JsonProperty("act"), XmlElement("act")]
        public Guid? ActKey
        {
            get
            {
                return this.SourceEntityKey;
            }
            set
            {
                this.SourceEntityKey = value;
            }
        }

        /// <summary>
        /// The entity that this relationship targets
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(ActKey)), DataIgnore]
        public Act Act
        {
            get
            {
                return this.SourceEntity;
            }
            set
            {
                this.SourceEntity = value;
            }
        }

        /// <summary>
        /// Gets or sets the quantity of player in the act
        /// </summary>
        [XmlElement("quantity"), JsonProperty("quantity")]
        public int? Quantity { get; set; }

        /// <summary>
        /// Forces a delay load from the underlying model
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_participationRole = null;
            this.m_player = null;
        }

        /// <summary>
        /// Clean
        /// </summary>
        /// <returns></returns>
        public override IdentifiedData Clean()
        {
            this.m_player = this.m_player?.Clean() as Entity;
            return this;
        }

        /// <summary>
        /// Determine if this is empty
        /// </summary>
        /// <returns></returns>
        public override bool IsEmpty()
        {
            return !this.ParticipationRoleKey.HasValue && this.ParticipationRole == null ||
                this.PlayerEntity == null && !this.PlayerEntityKey.HasValue;
        }

        /// <summary>
        /// Determine equality
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as ActParticipation;
            if (other == null) return false;
            return base.SemanticEquals(obj) && other.ActKey == this.ActKey &&
                other.PlayerEntityKey == this.PlayerEntityKey &&
                other.ParticipationRoleKey == this.ParticipationRoleKey;
        }

        /// <summary>
        /// Don't serialize source entity
        /// </summary>
        public override bool ShouldSerializeSourceEntityKey()
        {
            return false;
        }

        /// <summary>
        /// Should serialize quantity
        /// </summary>
        public bool ShouldSerializeQuantity()
        {
            return this.Quantity.HasValue;
        }

        /// <summary>
        /// Should serialize act key
        /// </summary>
        public bool ShouldSerializeActKey() => ActKey.HasValue;

        /// <summary>
        /// Represent as string
        /// </summary>
        public override string ToString()
        {
            return string.Format("({0}) {1} = {2}", this.ParticipationRole?.ToString() ?? this.ParticipationRoleKey?.ToString(), this.PlayerEntityKey, this.Quantity);
        }
    }
}
