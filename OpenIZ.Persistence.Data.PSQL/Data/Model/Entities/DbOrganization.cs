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
	/// Represents an organization in the data store
	/// </summary>
	[TableName("organization")]
	public class DbOrganization : IDbVersionedAssociation
	{
		/// <summary>
		/// Gets or sets the industry concept.
		/// </summary>
		/// <value>The industry concept.</value>
		[Column("industryConcept")]
		public Guid IndustryConceptKey {
			get;
			set;
		}

	}
}

