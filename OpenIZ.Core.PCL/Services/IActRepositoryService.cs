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
 * Date: 2016-8-3
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
	/// TODO: Should the entire interface be genericised?
	public interface IActRepositoryService 
	{
		/// <summary>
		/// Finds acts based on a specific query.
		/// </summary>
		/// <typeparam name="TAct">The type of the act.</typeparam>
		/// <param name="query">The query.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <returns>Returns a list of acts which match the specific query.</returns>
		IEnumerable<TAct> Find<TAct>(Expression<Func<TAct, bool>> query, int offset, int? count, out int totalResults) where TAct : Act;

		/// <summary>
		/// Get the specified act.
		/// </summary>
		/// <typeparam name="TAct">The type of the act.</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>Returns the act.</returns>
		TAct Get<TAct>(Guid key, Guid versionId) where TAct : Act;

		/// <summary>
		/// Insert the specified act.
		/// </summary>
		/// <typeparam name="TAct">The type of the act.</typeparam>
		/// <param name="act">The act.</param>
		/// <returns>Returns the act.</returns>
		TAct Insert<TAct>(TAct act) where TAct : Act;

		/// <summary>
		/// Obsoletes a specific act.
		/// </summary>
		/// <typeparam name="TAct">The type of the act.</typeparam>
		/// <param name="key">The key.</param>
		/// <returns>Returns the act.</returns>
		TAct Obsolete<TAct>(Guid key) where TAct : Act;

        /// <summary>
        /// Cancels a specific act.
        /// </summary>
        /// <typeparam name="TAct">The type of the act.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Returns the act.</returns>
        TAct Cancel<TAct>(TAct act) where TAct : Act;

        /// <summary>
        /// Nullifies a specific act.
        /// </summary>
        /// <typeparam name="TAct">The type of the act.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Returns the act.</returns>
        TAct Nullify<TAct>(TAct act) where TAct : Act;

        /// <summary>
        /// Inserts or updates the specific act.
        /// </summary>
        /// <typeparam name="TAct">The type of the act.</typeparam>
        /// <param name="act">The act.</param>
        /// <returns>Returns the act.</returns>
        TAct Save<TAct>(TAct act) where TAct : Act;

		/// <summary>
		/// Validates an act.
		/// </summary>
		/// <typeparam name="TAct">The type of the act.</typeparam>
		/// <param name="act">The act.</param>
		/// <returns>Returns the act.</returns>
		TAct Validate<TAct>(TAct act) where TAct : Act;
	}
}