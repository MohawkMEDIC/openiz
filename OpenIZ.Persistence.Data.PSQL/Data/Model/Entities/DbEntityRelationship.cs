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
 * Date: 2016-6-14
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.PSQL.Data.Model.Entities
{
    /// <summary>
    /// Represents a relationship between two entities
    /// </summary>
    [TableName("entity_relationship")] 
    public class DbEntityRelationship : IDbVersionedAssociation
    {

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>The entity identifier.</value>
        [Column("ent_id"), NotNull]
        public Guid EntityUuid
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target entity
        /// </summary>
        [Column("target"), NotNull]
        public Guid TargetKey { get; set; }


        /// <summary>
        /// Gets or sets the link type concept
        /// </summary>
        [Column("relationshipType")]
        public Guid RelationshipTypeKey { get; set; }

        /// <summary>
        /// Quantity 
        /// </summary>
        [Column("quantity")]
        public int Quantity { get; set; }
    }
}
