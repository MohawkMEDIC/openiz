using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents a data persistence provider that can store and continue queries
    /// </summary>
    public interface IStoredQueryDataPersistenceService<TEntity> : IDataPersistenceService<TEntity>
    {
        /// <summary>
        /// Queries or continues a query 
        /// </summary>
        IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> query, Guid queryId, int offset, int? count, IPrincipal authContext, out int totalCount);
    }
}
