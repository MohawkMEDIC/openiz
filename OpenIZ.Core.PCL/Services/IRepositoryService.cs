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
    /// Represents a repository service base
    /// </summary>
    public interface IRepositoryService<TModel> where TModel : IdentifiedData
    {

        /// <summary>
        /// Gets the specified data
        /// </summary>
        TModel Get(Guid key);

        /// <summary>
        /// Finds the specified data
        /// </summary>
        /// <returns></returns>
        IEnumerable<TModel> Find(Expression<Func<TModel, bool>> query);

        /// <summary>
        /// Inserts the specified data
        /// </summary>
        TModel Insert(TModel data);

        /// <summary>
        /// Saves the specified data
        /// </summary>
        TModel Save(TModel data);

        /// <summary>
        /// Obsoletes the specified data
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TModel Obsolete(Guid key);
    }
}
