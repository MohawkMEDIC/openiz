using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Persistable query provider is an extensable interface which can perform a query with state
    /// </summary>
    public interface IPersistableQueryProvider
    {

        /// <summary>
        /// Performs a query which 
        /// </summary>
        /// <typeparam name="TEntity">The underlying entity type which is being queried</typeparam>
        /// <param name="query">The query to be executed</param>
        /// <param name="offset">The offset</param>
        /// <param name="count">The number of results</param>
        /// <param name="totalResults">The total results in the query</param>
        /// <param name="queryId">The unique identifier for the query</param>
        IEnumerable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> query, int offset, int? count, out int totalResults, Guid queryId) where TEntity : IdentifiedData;

    }
}
