/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-1
 */
using OpenIZ.Persistence.Data.ADO.Data.Attributes;
using OpenIZ.Persistence.Data.ADO.Data.Model.Concepts;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data.Model.Acts
{
    /// <summary>
    /// Represents a link between an act and an entity
    /// </summary>
    [Table("act_participation")]
    public class DbActParticipation : DbActVersionedAssociation
    {
        /// <summary>
        /// Gets or sets the primary key
        /// </summary>
        [Column("act_ptcpt_id"), PrimaryKey]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the role that the player plays in the act
        /// </summary>
        [Column("rol_cd_id"), ForeignKey(typeof(DbConcept), nameof(DbConcept.Key))]
        public Guid ParticipationRoleKey { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        [Column("qty")]
        public int Quantity { get; set; }

        /// <summary>
        /// Target entity key
        /// </summary>
        [Column("ent_id"), ForeignKey(typeof(DbEntity), nameof(DbEntity.Key))]
        public Guid TargetKey { get; set; }
    }
}
