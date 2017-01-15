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
using OpenIZ.Persistence.Data.ADO.Data.Attributes;
using System;



namespace OpenIZ.Persistence.Data.ADO.Data.Model.Entities
{
	/// <summary>
	/// Represents an entity name related to an entity
	/// </summary>
	[Table("ent_name_tbl")]
	public class DbEntityName : DbEntityVersionedAssociation
    {
        [Column("name_id")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the use concept.
        /// </summary>
        /// <value>The use concept.</value>
        [Column("use_cd_id")]
		public Guid UseConceptKey {
			get;
			set;
		}
	}

    /// <summary>
    /// Represents a component of a name
    /// </summary>
    [Table("ent_name_cmp_tbl")]
    public class DbEntityNameComponent : DbGenericNameComponent
    {
        /// <summary>
        /// Gets or sets the linked name
        /// </summary>
        [Column("name_id")]
        public Guid NameKey { get; set; }
    }


}

