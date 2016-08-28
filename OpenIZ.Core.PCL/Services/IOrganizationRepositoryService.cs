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
 * User: khannan
 * Date: 2016-8-27
 */
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Provides operations for managing organizations.
	/// </summary>
	public interface IOrganizationRepositoryService
	{
		/// <summary>
		/// Searches for a organization using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the organization.</param>
		/// <returns>Returns a list of organizations who match the specified predicate.</returns>
		IEnumerable<Organization> Find(Expression<Func<Organization, bool>> predicate);

		/// <summary>
		/// Searches for a Organization using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the organization.</param>
		/// <param name="count">The count of the organizations to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of organizations who match the specified predicate.</returns>
		IEnumerable<Organization> Find(Expression<Func<Organization, bool>> predicate, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets the specified organization.
		/// </summary>
		/// <param name="id">The id of the organization.</param>
		/// <param name="versionId">The version id of the organization.</param>
		/// <returns>Returns the specified organization.</returns>
		Organization Get(Guid id, Guid versionId);

		/// <summary>
		/// Inserts the specified organization.
		/// </summary>
		/// <param name="organization">The organization to insert.</param>
		/// <returns>Returns the inserted organization.</returns>
		Organization Insert(Organization organization);

		/// <summary>
		/// Obsoletes the specified organization.
		/// </summary>
		/// <param name="id">The id of the organization to obsolete.</param>
		/// <returns>Returns the obsoleted organization.</returns>
		Organization Obsolete(Guid id);

		/// <summary>
		/// Saves the specified organization.
		/// </summary>
		/// <param name="organization">The organization to save.</param>
		/// <returns>Returns the saved organization.</returns>
		Organization Save(Organization organization);
	}
}
