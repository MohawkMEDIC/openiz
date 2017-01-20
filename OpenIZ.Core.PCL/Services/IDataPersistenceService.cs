using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Non-generic form of the data persistene service
    /// </summary>
    public interface IDataPersistenceService
    {

        /// <summary>
        /// Inserts the specified object
        /// </summary>
        Object Insert(Object data);

        /// <summary>
        /// Updates the specified data
        /// </summary>
        Object Update(Object data);

        /// <summary>
        /// Obsoletes the specified data
        /// </summary>
        Object Obsolete(Object data);

        /// <summary>
        /// Gets the specified data
        /// </summary>
        Object Get(Guid id);

        /// <summary>
        /// Query based on the expression given
        /// </summary>
        IEnumerable Query(Expression query, int offset, int? count, out int totalResults);
    }

}
