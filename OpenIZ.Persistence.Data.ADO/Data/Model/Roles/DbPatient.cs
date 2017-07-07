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
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.OrmLite.Attributes;
using OpenIZ.Persistence.Data.ADO.Data.Model.Concepts;

namespace OpenIZ.Persistence.Data.ADO.Data.Model.Roles
{
	/// <summary>
	/// Represents a patient in the SQLite store
	/// </summary>
	[Table("pat_tbl")]
	public class DbPatient : DbPersonSubTable
	{

		/// <summary>
		/// Gets or sets the gender concept
		/// </summary>
		/// <value>The gender concept.</value>
		[Column("gndr_cd_id"), ForeignKey(typeof(DbConcept), nameof(DbConcept.Key))]
		public Guid GenderConceptKey {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the deceased date.
		/// </summary>
		/// <value>The deceased date.</value>
		[Column("dcsd_utc")]
		public DateTime? DeceasedDate {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the deceased date precision.
		/// </summary>
		/// <value>The deceased date precision.</value>
		[Column("dcsd_prec")]
		public string DeceasedDatePrecision {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the multiple birth order.
		/// </summary>
		/// <value>The multiple birth order.</value>
		[Column("mb_ord")]
		public int? MultipleBirthOrder {
			get;
			set;
		}

	}
}

