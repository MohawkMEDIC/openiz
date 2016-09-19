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
 * Date: 2016-8-22
 */

using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents an act repository service.
	/// </summary>
	public interface IActRepositoryService
	{
		/// <summary>
		/// Finds acts based on a specific query.
		/// </summary>
		/// <param name="query">The query to use to find the acts.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalResults">The total results of the query.</param>
		/// <returns>Returns a list of acts.</returns>
		IEnumerable<Act> FindActs(Expression<Func<Act, bool>> query, int offset, int? count, out int totalResults);

		/// <summary>
		/// Finds substance administrations based on a specific query.
		/// </summary>
		/// <param name="query">The query to use to find the substance administrations.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalResults">The total results of the query.</param>
		/// <returns>Returns a list of substance administrations.</returns>
		IEnumerable<SubstanceAdministration> FindSubstanceAdministrations(Expression<Func<SubstanceAdministration, bool>> query, int offset, int? count, out int totalResults);

		/// <summary>
		/// Gets a specific act by key and version key.
		/// </summary>
		/// <param name="key">The key of the act.</param>
		/// <param name="versionKey">The version key of the act.</param>
		/// <returns>Returns an act.</returns>
		Act Get(Guid key, Guid versionKey);

		/// <summary>
		/// Inserts a specific act.
		/// </summary>
		/// <param name="act">The act to be inserted.</param>
		/// <returns>Returns the inserted act.</returns>
		Act Insert(Act act);

		/// <summary>
		/// Obsoletes a specific act.
		/// </summary>
		/// <param name="key">The key of the act to obsolete.</param>
		/// <returns>Returns the obsoleted act.</returns>
		Act Obsolete(Guid key);

		/// <summary>
		/// Inserts or updates the specific act.
		/// </summary>
		/// <param name="act">The act to be inserted or saved.</param>
		/// <returns>Returns the inserted or saved act.</returns>
		Act Save(Act act);

		/// <summary>
		/// Validates an act.
		/// </summary>
		/// <param name="act">The act to be validated.</param>
		/// <returns>Returns the validated act.</returns>
		Act Validate(Act act);
	}
}