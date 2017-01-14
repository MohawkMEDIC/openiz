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



namespace OpenIZ.Persistence.Data.PSQL.Data.Model.Entities
{
	/// <summary>
	/// Represents a material in the database
	/// </summary>
	[TableName("material")]
	public class DbMaterial : IDbVersionedAssociation
    {

		/// <summary>
		/// Gets or sets the quantity of an entity within its container.
		/// </summary>
		/// <value>The quantity.</value>
		[Column("quantity")]
		public decimal Quantity {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the form concept.
		/// </summary>
		/// <value>The form concept.</value>
		[Column("form_concept_uuid")]
		public Guid FormConceptKey {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the quantity concept.
		/// </summary>
		/// <value>The quantity concept.</value>
		[Column("quantity_concept_uuid")]
		public Guid QuantityConceptKey {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the expiry date.
		/// </summary>
		/// <value>The expiry date.</value>
		[Column("expiry")]
		public DateTime ExpiryDate {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is administrative.
		/// </summary>
		/// <value><c>true</c> if this instance is administrative; otherwise, <c>false</c>.</value>
		[Column("isAdministrative")]
		public bool IsAdministrative {
			get;
			set;
		}
	}

	/// <summary>
	/// Manufactured material.
	/// </summary>
	[TableName("manufactured_material")]
	public class DbManufacturedMaterial : IDbVersionedAssociation
	{

		/// <summary>
		/// Gets or sets the lot number.
		/// </summary>
		/// <value>The lot number.</value>
		[Column("lotNumber"), Indexed, Collation("NOCASE")]
		public String LotNumber {
			get;
			set;
		}
	}

}

