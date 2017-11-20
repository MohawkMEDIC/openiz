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
 * Date: 2016-11-30
 */
using OpenIZ.Core.Interfaces;
using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a repository service base
	/// </summary>
	public interface IRepositoryService<TModel> : IAuditEventSource where TModel : IdentifiedData
	{
		/// <summary>
		/// Gets the specified model.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		TModel Get(Guid key);

        /// <summary>
        /// Gets the specified model.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="versionKey">The key of the version.</param>
        /// <returns>Returns the model.</returns>
        TModel Get(Guid key, Guid versionKey);

        /// <summary>
        /// Finds the specified data.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns a list of identified data.</returns>
        IEnumerable<TModel> Find(Expression<Func<TModel, bool>> query);

		/// <summary>
		/// Finds the specified data.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <returns>Returns a list of identified data.</returns>
		IEnumerable<TModel> Find(Expression<Func<TModel, bool>> query, int offset, int? count, out int totalResults);

		/// <summary>
		/// Inserts the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>TModel.</returns>
		TModel Insert(TModel data);

		/// <summary>
		/// Saves the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>Returns the model.</returns>
		TModel Save(TModel data);

		/// <summary>
		/// Obsoletes the specified data.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		TModel Obsolete(Guid key);
	}
}