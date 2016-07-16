using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Event;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Caching.Memory
{
    /// <summary>
    /// Memory cache service
    /// </summary>
    public class MemoryCacheService : IDataCachingService
    {

        /// <summary>
        /// Cache of data
        /// </summary>
        private Dictionary<Type, Dictionary<Guid, IdentifiedData>> m_cache = new Dictionary<Type, Dictionary<Guid, IdentifiedData>>();
        private EventHandler<ModelMapEventArgs> m_mappingHandler = null;
        private EventHandler<ModelMapEventArgs> m_mappedHandler = null;
        private Object m_lock = new object();
        /// <summary>
        /// True when the memory cache is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_mappedHandler != null && m_mappedHandler != null;
            }
        }

        /// <summary>
        /// Service is starting
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Service has started
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Service is stopping
        /// </summary>
        public event EventHandler Stopped;
        /// <summary>
        /// Service has stopped
        /// </summary>
        public event EventHandler Stopping;

        /// <summary>
        /// Persistence event handler
        /// </summary>
        public void HandlePostPersistenceEvent(TransactionMode txMode, Object data)
        {
            Type objData = data?.GetType();
            var idData = data as IIdentifiedEntity;
            var verData = data as IVersionedEntity;

            Dictionary<Guid, IdentifiedData> cache = null;
            if (this.m_cache.TryGetValue(objData, out cache))
            {
                Guid key = verData?.VersionKey ?? idData?.Key ?? Guid.Empty;
                if (cache.ContainsKey(verData?.VersionKey ?? idData?.Key ?? Guid.Empty))
                    lock(this.m_lock)
                        cache[key] = (data as IdentifiedData);
                else
                    lock (this.m_lock)
                        if (!cache.ContainsKey(key))
                            cache.Add(key, data as IdentifiedData);
            }

        }

        /// <summary>
        /// Start the service
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            // handles when a item is being mapped
            this.m_mappingHandler = (o, e) =>
            {
                Dictionary<Guid, IdentifiedData> cache;
                if (this.m_cache.TryGetValue(e.ObjectType, out cache))
                {
                    IdentifiedData cacheEntry = null;
                    if (cache.TryGetValue(e.Key, out cacheEntry))
                        e.ModelObject = cacheEntry;
                    else
                        e.ModelObject = null;
                }
            };

            // Handles when an item is no longer being mapped
            this.m_mappedHandler = (o, e) =>
            {
                Dictionary<Guid, IdentifiedData> cache;
                if (this.m_cache.TryGetValue(e.ObjectType, out cache))
                {
                    IdentifiedData cacheEntry = null;
                    if (!cache.TryGetValue(e.Key, out cacheEntry)) // Not cached so lets cache it!
                        lock (this.m_lock)
                            if(!cache.ContainsKey(e.Key))
                                cache.Add(e.Key, e.ModelObject);
                    else lock(this.m_lock)
                        cache[e.Key] = e.ModelObject; // update cache
                }
                else
                {
                    // Lock the master cache 
                    lock (this.m_cache)
                    {
                        if (!this.m_cache.TryGetValue(e.ObjectType, out cache))
                        {
                            cache = new Dictionary<Guid, IdentifiedData>();
                            cache.Add(e.Key, e.ModelObject);
                            this.m_cache.Add(e.ObjectType, cache);
                        }
                        else
                        {
                            lock (this.m_lock) // add and return
                                cache.Add(e.Key, e.ModelObject);
                            return;
                        }
                    }

                    // We want to subscribe when this object is changed so we can keep the cache fresh
                    var idpType = typeof(IDataPersistenceService<>).MakeGenericType(e.ObjectType);
                    var ppeArgType = typeof(PostPersistenceEventArgs<>).MakeGenericType(e.ObjectType);
                    var evtHdlrType = typeof(EventHandler<>).MakeGenericType(ppeArgType);
                    var svcInstance = ApplicationContext.Current.GetService(idpType);

                    // Construct the delegate
                    var senderParm = Expression.Parameter(typeof(Object), "o");
                    var eventParm = Expression.Parameter(ppeArgType, "e");
                    var eventTxMode = Expression.MakeMemberAccess(eventParm, ppeArgType.GetProperty("Mode"));
                    var eventData = Expression.MakeMemberAccess(eventParm, ppeArgType.GetProperty("Data"));
                    var handlerLambda = Expression.Lambda(evtHdlrType, Expression.Call(Expression.Constant(this), typeof(MemoryCacheService).GetMethod("HandlePostPersistenceEvent"),
                        eventTxMode, eventData), senderParm, eventParm);
                    var delInstance = handlerLambda.Compile();

                    // Bind to events
                    idpType.GetEvent("Inserted").AddEventHandler(svcInstance, delInstance);
                    idpType.GetEvent("Updated").AddEventHandler(svcInstance, delInstance);
                    idpType.GetEvent("Obsoleted").AddEventHandler(svcInstance, delInstance);

                }
            };

        ModelMapper.MappingToModel += this.m_mappingHandler;
            ModelMapper.MappedToModel += this.m_mappedHandler;

            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

    /// <summary>
    /// Stopping
    /// </summary>
    /// <returns></returns>
    public bool Stop()
    {
        this.Stopping?.Invoke(this, EventArgs.Empty);

        ModelMapper.MappingToModel -= this.m_mappingHandler;
        ModelMapper.MappedToModel -= this.m_mappedHandler;

        this.m_mappingHandler = null;
        this.m_mappedHandler = null;

        this.Stopped?.Invoke(this, EventArgs.Empty);
        return true;
    }
}
}
