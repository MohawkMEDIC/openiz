using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.EntityLoader
{
    /// <summary>
    /// Delay load provider
    /// </summary>
    public interface IEntitySourceProvider
    {
        
        /// <summary>
        /// Get the specified object
        /// </summary>
        TObject Get<TObject>(Guid key);

        /// <summary>
        /// Get the specified object
        /// </summary>
        TObject Get<TObject>(Guid key, Guid versionKey);

        /// <summary>
        /// Query the specified data from the delay load provider
        /// </summary>
        IEnumerable<TObject> Query<TObject>(Expression<Func<TObject, bool>> query);
    }
}
