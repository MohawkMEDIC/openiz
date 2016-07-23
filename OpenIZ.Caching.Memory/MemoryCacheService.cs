/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-7-16
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Event;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Caching.Memory.Configuration;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Caching.Memory
{
    /// <summary>
    /// Memory cache service
    /// </summary>
    public class MemoryCacheService : IDataCachingService, IDaemonService
    {

        /// <summary>
        /// Cache of data
        /// </summary>
        private EventHandler<ModelMapEventArgs> m_mappingHandler = null;
        private EventHandler<ModelMapEventArgs> m_mappedHandler = null;

        // Memory cache configuration
        private MemoryCacheConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.caching.memory") as MemoryCacheConfiguration;
        private TraceSource m_tracer = new TraceSource("OpenIZ.Caching.Memory");

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
            if (txMode == TransactionMode.Commit)
                MemoryCache.Current.AddUpdateEntry(data);
        }

        /// <summary>
        /// Start the service
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            this.m_tracer.TraceInformation("Starting Memory Caching Service...");

            this.Starting?.Invoke(this, EventArgs.Empty);

            // handles when a item is being mapped
            this.m_mappingHandler = (o, e) =>
            {
                this.GetOrUpdateCacheItem(e);
            };

            // Handles when an item is no longer being mapped
            this.m_mappedHandler = (o, e) =>
            {
                this.GetOrUpdateCacheItem(e);
            };

            // Subscribe to message mapping
            ModelMapper.MappingToModel += this.m_mappingHandler;
            ModelMapper.MappedToModel += this.m_mappedHandler;

            // Now we start timers
            var timerService = ApplicationContext.Current.GetService<ITimerService>();
            if(timerService != null)
            {
                timerService.AddJob(new CacheCleanupTimerJob(), new TimeSpan(this.m_configuration.Types.Select(o=>o.MaxCacheAge).Min()));
                timerService.AddJob(new CacheRegulatorTimerJob(), new TimeSpan(0, 1, 0));
            }
            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Either gets or updates the existing cache item
        /// </summary>
        /// <param name="e"></param>
        private void GetOrUpdateCacheItem(ModelMapEventArgs e)
        {
            var cacheItem = MemoryCache.Current.TryGetEntry(e.ObjectType, e.Key);
            if (cacheItem == null)
                MemoryCache.Current.AddUpdateEntry(e.ModelObject);
            else
            {
                // Obsolete?
                var cVer = cacheItem as IVersionedEntity;
                var dVer = e.ModelObject as IVersionedEntity;
                if (cVer?.VersionSequence < dVer?.VersionSequence) // Cache is older than this item
                    MemoryCache.Current.AddUpdateEntry(dVer);
                e.ModelObject = cacheItem as IdentifiedData;
                e.Cancel = true;
            }
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

        /// <summary>
        /// Gets the specified cache item
        /// </summary>
        /// <returns></returns>
        public TData GetCacheItem<TData>(Guid key) where TData : IdentifiedData
        {
            return MemoryCache.Current.TryGetEntry(typeof(TData), key) as TData;
        }
    }
}
