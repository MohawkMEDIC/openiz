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
 * Date: 2016-8-2
 */
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Principal;

namespace OpenIZ.Core.Services
{
	public interface IProviderRepositoryService
	{
		/// <summary>
		/// Searches for a provider using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the provider.</param>
		/// <returns>Returns a list of providers who match the specified predicate.</returns>
		IEnumerable<Provider> Find(Expression<Func<Provider, bool>> predicate);

		/// <summary>
		/// Searches for a provider using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the provider.</param>
		/// <param name="count">The count of the providers to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of providers who match the specified predicate.</returns>
		IEnumerable<Provider> Find(Expression<Func<Provider, bool>> predicate, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets the specified provider.
		/// </summary>
		/// <param name="id">The id of the provider.</param>
		/// <param name="versionId">The version id of the provider.</param>
		/// <returns>Returns the specified provider.</returns>
		Provider Get(Guid id, Guid versionId);

		/// <summary>
		/// Get the provider based off the user identity
		/// </summary>
		Provider Get(IIdentity identity);

		/// <summary>
		/// Inserts the specified provider.
		/// </summary>
		/// <param name="provider">The provider to insert.</param>
		/// <returns>Returns the inserted provider.</returns>
		Provider Insert(Provider provider);

		/// <summary>
		/// Obsoletes the specified provider.
		/// </summary>
		/// <param name="id">The id of the provider to obsolete.</param>
		/// <returns>Returns the obsoleted provider.</returns>
		Provider Obsolete(Guid id);

		/// <summary>
		/// /// Saves the specified provider.
		/// </summary>
		/// <param name="provider">The provider to save.</param>
		/// <returns>Returns the saved provider.</returns>
		Provider Save(Provider provider);
	}
}