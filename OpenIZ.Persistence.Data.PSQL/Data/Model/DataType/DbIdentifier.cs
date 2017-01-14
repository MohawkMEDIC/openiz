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
using PetaPoco;
using System;



namespace OpenIZ.Persistence.Data.PSQL.Data.Model.DataType
{
    /// <summary>
    /// Represents an identifier
    /// </summary>
    public abstract class DbIdentifier : DbVersionedAssociation
    {

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [Column("id_val")]
        public String Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type identifier.
        /// </summary>
        /// <value>The type identifier.</value>
        [Column("id_typ_id")]
        public Guid TypeKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the authority identifier.
        /// </summary>
        /// <value>The authority identifier.</value>
        [Column("aut_id")]
        public Guid AuthorityKey
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Entity identifier storage.
    /// </summary>
    [TableName("ent_id_tbl")]
    public class DbEntityIdentifier : DbIdentifier
    {

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [Column("ent_id_id")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets the source key
        /// </summary>
        [Column("ent_id")]
        public override Guid SourceKey { get; set; }
    }

    /// <summary>
    /// Act identifier storage.
    /// </summary>
    [TableName("act_id_tbl")]
    public class DbActIdentifier : DbIdentifier
    {
        /// <summary>
        /// Gets or sets the act key
        /// </summary>
        [Column("act_id_id")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the source key
        /// </summary>
        [Column("act_id")]
        public override Guid SourceKey { get; set; ]
    }
}

