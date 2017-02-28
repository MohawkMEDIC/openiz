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