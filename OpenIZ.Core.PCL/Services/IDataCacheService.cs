using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents a data caching service which allows services to retrieve
    /// cached objects
    /// </summary>
    public interface IDataCachingService
    {
        /// <summary>
        /// Get the specified cache item
        /// </summary>
        TData GetCacheItem<TData>(Guid key) where TData: IdentifiedData;
    }
}
