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
	/// Represents one or more entity addresses linked to an Entity
	/// </summary>
	[TableName("entity_address")]
	public class DbEntityAddress : DbEntityLink
	{
		
		/// <summary>
		/// Gets or sets the use concept identifier.
		/// </summary>
		/// <value>The use concept identifier.</value>
		[Column("use")]
		public Guid UseConceptKey {
			get;
			set;
		}

	}

	/// <summary>
	/// Represents an identified address component
	/// </summary>
	[TableName("entity_address_comp")]
	public class DbEntityAddressComponent : DbGenericNameComponent
	{

		/// <summary>
		/// Gets or sets the address identifier.
		/// </summary>
		/// <value>The address identifier.</value>
		[Column("address_uuid")]
		public Guid AddressKey {
			get;
			set;
		}
	

	}

}

