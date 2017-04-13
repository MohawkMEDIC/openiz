using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
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
    /// Data persistence service lean mode
    /// </summary>
    public interface IFastQueryDataPersistenceService<TEntity> : IStoredQueryDataPersistenceService<TEntity> where TEntity : IdentifiedData
    {
        /// <summary>
        /// Queries or continues a query in lean mode
        /// </summary>
        IEnumerable<TEntity> QueryFast(Expression<Func<TEntity, bool>> query, Guid queryId, int offset, int? count, IPrincipal authContext, out int totalCount);

    }
}
