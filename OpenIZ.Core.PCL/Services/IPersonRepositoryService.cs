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
 * Date: 2017-1-6
 */
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Principal;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a repository service for managing persons.
	/// </summary>
	public interface IPersonRepositoryService
	{
		/// <summary>
		/// Searches for a person using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the person.</param>
		/// <returns>Returns a list of persons who match the specified predicate.</returns>
		IEnumerable<Person> Find(Expression<Func<Person, bool>> predicate);

		/// <summary>
		/// Searches for a person using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the person.</param>
		/// <param name="count">The count of the persons to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of persons who match the specified predicate.</returns>
		IEnumerable<Person> Find(Expression<Func<Person, bool>> predicate, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets the specified person.
		/// </summary>
		/// <param name="id">The id of the person.</param>
		/// <param name="versionId">The version id of the person.</param>
		/// <returns>Returns the specified person.</returns>
		Person Get(Guid id, Guid versionId);

		/// <summary>
		/// Get the person based off the user identity
		/// </summary>
		Person Get(IIdentity identity);

		/// <summary>
		/// Inserts the specified person.
		/// </summary>
		/// <param name="person">The person to insert.</param>
		/// <returns>Returns the inserted person.</returns>
		Person Insert(Person person);

		/// <summary>
		/// Obsoletes the specified person.
		/// </summary>
		/// <param name="id">The id of the person to obsolete.</param>
		/// <returns>Returns the obsoleted person.</returns>
		Person Obsolete(Guid id);

		/// <summary>
		/// /// Saves the specified person.
		/// </summary>
		/// <param name="person">The person to save.</param>
		/// <returns>Returns the saved person.</returns>
		Person Save(Person person);
	}
}