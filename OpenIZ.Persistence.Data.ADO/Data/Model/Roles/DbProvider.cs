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
 * Date: 2017-1-14
 */
using System;
using OpenIZ.OrmLite.Attributes;
using OpenIZ.Persistence.Data.ADO.Data.Model.Concepts;

namespace OpenIZ.Persistence.Data.ADO.Data.Model.Roles
{
	/// <summary>
	/// Represents a health care provider in the database
	/// </summary>
	[Table("pvdr_tbl")]
	public class DbProvider : DbPersonSubTable
    {

		/// <summary>
		/// Gets or sets the specialty.
		/// </summary>
		/// <value>The specialty.</value>
		[Column("spec_cd_id"), ForeignKey(typeof(DbConcept), nameof(DbConcept.Key))]
		public Guid Specialty {
			get;
			set;
		}

	}
}

