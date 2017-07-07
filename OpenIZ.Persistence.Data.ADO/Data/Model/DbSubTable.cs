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
 * Date: 2017-1-15
 */
using OpenIZ.OrmLite.Attributes;
using OpenIZ.Persistence.Data.ADO.Data.Model.Acts;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data.Model
{
    /// <summary>
    /// Gets or sets the derived parent class
    /// </summary>
    public abstract class DbSubTable 
    {

        /// <summary>
        /// Parent key
        /// </summary>
        public abstract Guid ParentKey { get; set; }


    }

    /// <summary>
    /// Act based sub-table
    /// </summary>
    public abstract class DbActSubTable : DbSubTable
    {
        /// <summary>
        /// Gets or sets the parent key
        /// </summary>
        [Column("act_vrsn_id"), ForeignKey(typeof(DbActVersion), nameof(DbActVersion.VersionKey)), PrimaryKey, AlwaysJoin]
        public override Guid ParentKey { get; set; }
    }

    /// <summary>
    /// Act based sub-table
    /// </summary>
    public abstract class DbObsSubTable : DbActSubTable
    {
        /// <summary>
        /// Gets or sets the parent key
        /// </summary>
        [Column("act_vrsn_id"), ForeignKey(typeof(DbObservation), nameof(DbObservation.ParentKey)), PrimaryKey]
        public override Guid ParentKey { get; set; }
    }

    /// <summary>
    /// Entity based sub-table
    /// </summary>
    public abstract class DbEntitySubTable : DbSubTable
    {
        /// <summary>
        /// Gets or sets the parent key
        /// </summary>
        [Column("ent_vrsn_id"), ForeignKey(typeof(DbEntityVersion), nameof(DbEntityVersion.VersionKey)), PrimaryKey, AlwaysJoin]
        public override Guid ParentKey { get; set; }
    }

    /// <summary>
    /// Represents a person based sub-table
    /// </summary>
    public abstract class DbPersonSubTable : DbEntitySubTable
    {
        /// <summary>
        /// Gets or sets the parent key
        /// </summary>
        [Column("ent_vrsn_id"), ForeignKey(typeof(DbPerson), nameof(DbPerson.ParentKey)), PrimaryKey, AlwaysJoin]
        public override Guid ParentKey { get; set; }
    }
}
