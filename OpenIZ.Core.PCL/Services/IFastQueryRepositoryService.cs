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
    /// Represents a query repository service which can find lean queries
    /// </summary>
    public interface IFastQueryRepositoryService : IPersistableQueryRepositoryService
    {

        /// <summary>
        /// Perform a quick search (instructs the data persistence layer not to load as many properties)
        /// </summary>
        IEnumerable<TEntity> FindFast<TEntity>(Expression<Func<TEntity, bool>> query, int offset, int? count, out int totalResults, Guid queryId) where TEntity : IdentifiedData;

    }
}
