/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-8-2
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Event;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Caching.Memory.Configuration;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
	    private static object s_lock = new object();

        // Non cached types
        private HashSet<Type> m_nonCached = new HashSet<Type>();

        /// <summary>
        /// True when the memory cache is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_mappingHandler != null;
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
        public event EventHandler<DataCacheEventArgs> Added;
        public event EventHandler<DataCacheEventArgs> Updated;
        public event EventHandler<DataCacheEventArgs> Removed;



        /// <summary>
        /// Start the service
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            this.m_tracer.TraceInformation("Starting Memory Caching Service...");

            this.Starting?.Invoke(this, EventArgs.Empty);

            // subscribe to events
            this.Added += (o, e) => this.EnsureCacheConsistency(e);
            this.Updated += (o, e) => this.EnsureCacheConsistency(e);
            this.Removed += (o, e) => this.EnsureCacheConsistency(e);

            MemoryCache.Current.Clear();

            // handles when a item is being mapped
            this.m_mappingHandler = (o, e) =>
            {
                var cacheItem = MemoryCache.Current.TryGetEntry(e.Key);
                if (cacheItem != null)
                {
                    e.ModelObject = cacheItem as IdentifiedData;
                    e.Cancel = true;
                }
                //this.GetOrUpdateCacheItem(e);
            };

            // Subscribe to message mapping
            ModelMapper.MappingToModel += this.m_mappingHandler;
            ModelMapper.MappedToModel += this.m_mappedHandler;
            

            // Now we start timers
            var timerService = ApplicationContext.Current.GetService<ITimerService>();
            if(timerService != null && this.m_configuration.Types.Count > 0)
            {
                if (timerService.IsRunning)
                {
                    timerService.AddJob(new CacheCleanupTimerJob(), new TimeSpan(this.m_configuration.MaxCacheAge));
                    timerService.AddJob(new CacheRegulatorTimerJob(), new TimeSpan(0, 1, 0));
                }
                else
                    timerService.Started += (s, e) =>
                    {
                        timerService.AddJob(new CacheCleanupTimerJob(), new TimeSpan(this.m_configuration.MaxCacheAge));
                        timerService.AddJob(new CacheRegulatorTimerJob(), new TimeSpan(0, 1, 0));
                    };
            }

            // Look for non-cached types
            foreach (var itm in typeof(IdentifiedData).Assembly.GetTypes().Where(o => o.GetCustomAttribute<NonCachedAttribute>() != null || o.GetCustomAttribute<XmlRootAttribute>() == null))
                this.m_nonCached.Add(itm);

            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Ensure cache consistency
        /// </summary>
        private void EnsureCacheConsistency(DataCacheEventArgs e)
        {
            lock (s_lock)
            {
                //// Relationships should always be clean of source/target so the source/target will load the new relationship
                if (e.Object is ActParticipation)
                {
                    var ptcpt = (e.Object as ActParticipation);

                    this.Remove(ptcpt.SourceEntityKey.GetValueOrDefault());
                    this.Remove(ptcpt.PlayerEntityKey.GetValueOrDefault());
                    //MemoryCache.Current.RemoveObject(ptcpt.PlayerEntity?.GetType() ?? typeof(Entity), ptcpt.PlayerEntityKey);
                }
                else if (e.Object is ActRelationship)
                {
                    var rel = (e.Object as ActRelationship);
                    this.Remove(rel.SourceEntityKey.GetValueOrDefault());
                    this.Remove(rel.TargetActKey.GetValueOrDefault());
                }
                else if (e.Object is EntityRelationship)
                {
                    var rel = (e.Object as EntityRelationship);
                    this.Remove(rel.SourceEntityKey.GetValueOrDefault());
                    this.Remove(rel.TargetEntityKey.GetValueOrDefault());
                }
                else if (e.Object is Act) // We need to remove RCT 
                {
                    var act = e.Object as Act;
                    var rct = act.Participations.FirstOrDefault(x => x.ParticipationRoleKey == ActParticipationKey.RecordTarget || x.ParticipationRole?.Mnemonic == "RecordTarget");
                    if (rct != null)
                        MemoryCache.Current.RemoveObject(rct.PlayerEntityKey);
                }
            }
        }

        /// <summary>
        /// Either gets or updates the existing cache item
        /// </summary>
        /// <param name="e"></param>
        private void GetOrUpdateCacheItem(ModelMapEventArgs e)
        {
            var cacheItem = MemoryCache.Current.TryGetEntry(e.Key);
            if (cacheItem == null)
                this.Add(e.ModelObject);
            else
            {
                // Obsolete?
                var cVer = cacheItem as IVersionedEntity;
                var dVer = e.ModelObject as IVersionedEntity;
                if (cVer?.VersionSequence < dVer?.VersionSequence) // Cache is older than this item
                    this.Add(dVer as IdentifiedData);
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
            return MemoryCache.Current.TryGetEntry(key) as TData;
        }

        /// <summary>
        /// Get the specified cache item
        /// </summary>
        public object GetCacheItem(Guid key)
        {
            return MemoryCache.Current.TryGetEntry(key);
        }

        /// <summary>
        /// Add the specified item to the memory cache
        /// </summary>
        public void Add(IdentifiedData data)
        {
			// if the data is null, continue
	        if (data == null || !data.Key.HasValue ||
                    (data as BaseEntityData)?.ObsoletionTime.HasValue == true ||
                    this.m_nonCached.Contains(data.GetType()))
	        {
		        return;
	        }

            var exist = MemoryCache.Current.TryGetEntry(data.Key);
            MemoryCache.Current.AddUpdateEntry(data);
            if (exist != null)
                this.Updated?.Invoke(this, new DataCacheEventArgs(data));
            else
                this.Added?.Invoke(this, new DataCacheEventArgs(data));
        }

        /// <summary>
        /// Remove the object from the cache
        /// </summary>
        public void Remove(Guid key)
        {
            var exist = MemoryCache.Current.TryGetEntry(key);
            if (exist != null)
            {
                MemoryCache.Current.RemoveObject(key);
                this.Removed?.Invoke(this, new DataCacheEventArgs(exist));
            }
        }
    }
}
