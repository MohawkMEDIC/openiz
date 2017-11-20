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
 * Date: 2016-8-28
 */
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Provides operations for managing identifier types.
	/// </summary>
	public interface IIdentifierTypeRepositoryService
	{
		/// <summary>
		/// Searches for an identifier type using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the identifier type.</param>
		/// <returns>Returns a list of identifier types who match the specified query.</returns>
		IEnumerable<IdentifierType> Find(Expression<Func<IdentifierType, bool>> query);

		/// <summary>
		/// Searches for an identifier type using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the organization.</param>
		/// <param name="count">The count of the organizations to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of identifier types who match the specified query.</returns>
		IEnumerable<IdentifierType> Find(Expression<Func<IdentifierType, bool>> query, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets the specified identifier type.
		/// </summary>
		/// <param name="id">The id of the identifier type.</param>
		/// <param name="versionId">The version id of the identifier type.</param>
		/// <returns>Returns the specified identifier type.</returns>
		IdentifierType Get(Guid id, Guid versionId);

		/// <summary>
		/// Inserts the specified identifier type.
		/// </summary>
		/// <param name="identifierType">The identifier type to insert.</param>
		/// <returns>Returns the inserted organization.</returns>
		IdentifierType Insert(IdentifierType identifierType);

		/// <summary>
		/// Obsoletes the specified identifier type.
		/// </summary>
		/// <param name="id">The id of the identifier type to obsolete.</param>
		/// <returns>Returns the obsoleted identifier type.</returns>
		IdentifierType Obsolete(Guid id);

		/// <summary>
		/// Saves the specified identifier type.
		/// </summary>
		/// <param name="identifierType">The identifier type to save.</param>
		/// <returns>Returns the saved identifier type.</returns>
		IdentifierType Save(IdentifierType identifierType);
	}
}