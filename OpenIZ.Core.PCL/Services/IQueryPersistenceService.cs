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
using System;
using System.Collections.Generic;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Query persistence service
	/// </summary>
	public interface IQueryPersistenceService
	{
		/// <summary>
		/// Register a query set
		/// </summary>
		/// <param name="queryId">The unique identifier for the query</param>
		/// <param name="results">The results to be stored in the query</param>
		/// <param name="tag">A user tag for the query result set. Can be used to determine
		/// the type of data being returned</param>
		bool RegisterQuerySet(Guid queryId, IEnumerable<Guid> results, object tag);

		/// <summary>
		/// Returns true if the query identifier is already registered
		/// </summary>
		/// <param name="queryId"></param>
		/// <returns></returns>
		bool IsRegistered(Guid queryId);

		/// <summary>
		/// Get query results from the query set result store
		/// </summary>
		/// <param name="queryId">The identifier for the query</param>
		/// <param name="offset">The query offset</param>
		/// <param name="count">The number of records to pop</param>
		IEnumerable<Guid> GetQueryResults(Guid queryId, int offset, int count);

		/// <summary>
		/// Get the query tag value from the result store
		/// </summary>
		object GetQueryTag(Guid queryId);

		/// <summary>
		/// Count the number of remaining query results
		/// </summary>
		/// <param name="queryId">Unique identifier for the query to count remaining results</param>
		long QueryResultTotalQuantity(Guid queryId);
	}
}