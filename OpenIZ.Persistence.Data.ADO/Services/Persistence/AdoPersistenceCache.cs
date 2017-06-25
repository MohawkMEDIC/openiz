using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Services;
using OpenIZ.OrmLite;
using System;
using System.Linq;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Represents a temporary cache
    /// </summary>
    internal class AdoPersistenceCache
    {
        // Context
        private DataContext m_context;

        // Cache
        private IDataCachingService m_cache = ApplicationContext.Current.GetService<IDataCachingService>();

        /// <summary>
        /// Create a new persistence cache
        /// </summary>
        public AdoPersistenceCache(DataContext context)
        {
            this.m_context = context;
        }

        /// <summary>
        /// Add to cache
        /// </summary>
        public void Add(IdentifiedData data)
        {
            if(data != null)
                this.m_context.AddCacheCommit(data);
        }

        /// <summary>
        /// Get cache item
        /// </summary>
        public TReturn GetCacheItem<TReturn>(Guid key)
        {
            return (TReturn)(this.m_context.GetCacheCommit(key) ??
                this.m_cache.GetCacheItem(key));
        }

        /// <summary>
        /// Get cache item
        /// </summary>
        public IdentifiedData GetCacheItem(Guid key)
        {
            return (this.m_context.GetCacheCommit(key) ??
                this.m_cache.GetCacheItem(key)) as IdentifiedData;
        }
    }
}