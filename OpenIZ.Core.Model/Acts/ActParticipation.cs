/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-2-1
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
using OpenIZ.Core.Model.EntityLoader;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Associates an entity which participates in an act
    /// </summary>
    [Classifier(nameof(ParticipationRole))]
    [XmlType(Namespace = "http://openiz.org/model", TypeName = "ActParticipation")]
    public class ActParticipation : VersionedAssociation<Act>
    {

        /// <summary>
        /// Gets or sets the target entity reference
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("player"), JsonProperty("player")]
        public Guid? PlayerEntityKey
        {
            get { return this.PlayerEntity?.Key; }
            set
            {
                if (value != this.PlayerEntity?.Key)
                    this.PlayerEntity = this.EntityProvider.Get<Entity>(value);
            }
        }

        /// <summary>
        /// Gets or sets the participation role key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [DataIgnore, XmlElement("participationRole"), JsonProperty("participationRole")]
        public Guid? ParticipationRoleKey
        {
            get { return this.ParticipationRole?.Key; }
            set
            {
                if (value != this.ParticipationRole?.Key)
                    this.ParticipationRole = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the entity which participated in the act
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(PlayerEntityKey))]
		public Entity PlayerEntity { get; set; }

        /// <summary>
        /// Gets or sets the role that the entity played in participating in the act
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(ParticipationRoleKey))]
		public Concept ParticipationRole { get; set; }

    }
}
