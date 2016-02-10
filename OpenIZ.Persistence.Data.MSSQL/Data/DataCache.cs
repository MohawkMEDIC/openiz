using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Map;
using OpenIZ.Persistence.Data.MSSQL.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Data
{
    /// <summary>
    /// Cache item in the object cache
    /// </summary>
    public struct CacheItem
    {

        // Model instance
        private IdentifiedData m_modelInstance;

        /// <summary>
        /// Creates a new cache item
        /// </summary>
        public CacheItem(IdentifiedData modelItem, Object domainItem)
        {
            this.m_modelInstance = modelItem;
            //this.ModelInstance = modelItem;
            this.DomainItem = domainItem;
            this.AccessTime = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the model class
        /// </summary>
        public IdentifiedData ModelInstance
        {
            get
            {
                this.AccessTime = DateTime.Now;
                return this.m_modelInstance;
            }
            set
            {
                this.m_modelInstance = value;
                this.AccessTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Domain class
        /// </summary>
        public Object DomainItem { get; set; }
        /// <summary>
        /// Last accessed time
        /// </summary>
        public DateTime AccessTime { get; private set; }
    }

    /// <summary>
    /// Represents the data cache
    /// </summary>
    public sealed class DataCache
    {

        // Current cache
        private static DataCache s_currentItem;

        // Current lock object
        private static Object s_lockObject = new object();

        // Configuration
        private SqlConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(SqlServerConstants.ConfigurationSectionName) as SqlConfiguration;

        // Cache itmes
        private Dictionary<Guid, CacheItem> m_cache = new Dictionary<Guid, CacheItem>();

        // TODO: Move this to a singleton
        private static ModelMapper s_mapper = new ModelMapper(typeof(DataCache).Assembly.GetManifestResourceStream(SqlServerConstants.ModelMapResourceName));

        /// <summary>
        /// Private ctor
        /// </summary>
        private DataCache()
        { }

        /// <summary>
        /// Gets the currently initialized application cache
        /// </summary>
        public static DataCache Current
        {
            get
            {
                if (s_currentItem == null)
                    lock (s_lockObject)
                        if (s_currentItem == null)
                            s_currentItem = new DataCache();
                return s_currentItem;
            }
        }

        /// <summary>
        /// Gets the cache item
        /// </summary>
        internal IdentifiedData Get(Guid key)
        {
            CacheItem existingItem = default(CacheItem);
            if (this.m_cache.TryGetValue(key, out existingItem))
                return existingItem.ModelInstance;
            return null;
        }

        /// <summary>
        /// Adds the specified data to the cache
        /// </summary>
        public void Add(IdentifiedData modelItem, Object domainItem)
        {
            if (this.m_configuration.MaxCacheSize == 0)
                return; // no caching

            this.ReduceCache();

            CacheItem existingItem = default(CacheItem);
            Guid key = modelItem.Key;
            if (modelItem is IVersionedEntity)
                key = (modelItem as IVersionedEntity).VersionKey;

            if (this.m_cache.TryGetValue(key, out existingItem))
            {
                existingItem.ModelInstance = modelItem;
                existingItem.DomainItem = domainItem;
            }
            else
                lock (s_lockObject)
                    this.m_cache.Add(key, new CacheItem(modelItem, domainItem));
        }

        /// <summary>
        /// Reduces the cache to the configuration size
        /// </summary>
        private void ReduceCache()
        {
            if (this.m_cache.Count > this.m_configuration.MaxCacheSize)
                lock (s_lockObject)
                {
                    var values = this.m_cache.OrderByDescending(o => o.Value.AccessTime).Take(this.m_cache.Count - (int)(this.m_configuration.MaxCacheSize * 0.75));
                    foreach (var itm in values)
                        this.m_cache.Remove(itm.Key);
                }
        }

        /// <summary>
        /// Remove data from the cache
        /// </summary>
        public void Remove(IdentifiedData data)
        {
            if (this.m_configuration.MaxCacheSize == 0)
                return;

            lock (s_lockObject)
            {
                if (data is IVersionedEntity)
                    this.m_cache.Remove((data as IVersionedEntity).VersionKey);
                else
                    this.m_cache.Remove(data.Key);
            }
        }

        /// <summary>
        /// Get item from the cache or add it 
        /// </summary>
        public TModel GetOrAdd<TModel, TDomain>(Guid key, TDomain domainObject) where TModel : IdentifiedData, new()
        where TDomain : class, new()
        {

            // No cache
            if (this.m_configuration.MaxCacheSize == 0)
                return s_mapper.MapDomainInstance<TDomain, TModel>(domainObject); 

            // Lookup existing item
            CacheItem existingItem = default(CacheItem);
            if (!this.m_cache.TryGetValue(key, out existingItem))
            {
                this.ReduceCache();

                // Not found convert the model item and store it in cache
                TModel modelItem = s_mapper.MapDomainInstance<TDomain, TModel>(domainObject);
                lock (s_lockObject)
                    if (!this.m_cache.ContainsKey(key))
                        this.m_cache.Add(key, new CacheItem(modelItem, domainObject));
                return modelItem as TModel;
            }
            else if (!domainObject.IsSame(existingItem.DomainItem))
            {
                // Found but not equal, update cache
                TModel modelItem = s_mapper.MapDomainInstance<TDomain, TModel>(domainObject);
                existingItem.ModelInstance = modelItem;
                existingItem.DomainItem = domainObject;
                return modelItem as TModel;
            }
            else
                return existingItem.ModelInstance as TModel;
        }


        /// <summary>
        /// Is the cache enabled
        /// </summary>
        public bool IsCacheEnabled
        {
            get
            {
                return this.m_configuration.MaxCacheSize > 0;
            }
        }

        /// <summary>
        /// Gets the current size of the cahce
        /// </summary>
        public int Count
        {
            get
            {
                return this.m_cache.Count;
            }
        }
    }
}
