﻿/*
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
using OpenIZ.Persistence.Data.ADO.Data.Model.Concepts;
using System;

namespace OpenIZ.Persistence.Data.ADO.Data.Model.Entities
{
	/// <summary>
	/// Represents an entity in the database
	/// </summary>
	[Table("ent_vrsn_tbl")]
	public class DbEntityVersion : DbVersionedData
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [Column("ent_id"), ForeignKey(typeof(DbEntity), nameof(DbEntity.Key))]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the status concept identifier.
        /// </summary>
        /// <value>The status concept identifier.</value>
        [Column("sts_cd_id"), ForeignKey(typeof(DbConcept), nameof(DbConcept.Key))]
		public Guid StatusConceptKey {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type concept identifier.
		/// </summary>
		/// <value>The type concept identifier.</value>
		[Column("typ_cd_id"), ForeignKey(typeof(DbConcept), nameof(DbConcept.Key))]
		public Guid TypeConceptKey {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the version id
        /// </summary>
        [Column("ent_vrsn_id"), PrimaryKey]
        public override Guid VersionKey { get; set; }
    }
}
