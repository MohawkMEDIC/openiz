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
 * Date: 2016-8-2
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Event;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Caching.Memory.Configuration;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
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
        /// Start the service
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            this.m_tracer.TraceInformation("Starting Memory Caching Service...");

            this.Starting?.Invoke(this, EventArgs.Empty);

            MemoryCache.Current.Clear();

            // handles when a item is being mapped
            this.m_mappingHandler = (o, e) =>
            {
                var cacheItem = MemoryCache.Current.TryGetEntry(e.ObjectType, e.Key);
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

            // Relationship / Associated Entities are to be refreshed whenever persisted
            if (ApplicationContext.Current.GetService<IDataPersistenceService<ActParticipation>>() != null)
            {
                ApplicationContext.Current.GetService<IDataPersistenceService<ActParticipation>>().Updated += (o, e) => MemoryCache.Current.RemoveObject(typeof(Act), e.Data.SourceEntityKey);
                ApplicationContext.Current.GetService<IDataPersistenceService<ActParticipation>>().Inserted += (o, e) => MemoryCache.Current.RemoveObject(typeof(Act), e.Data.SourceEntityKey);
                ApplicationContext.Current.GetService<IDataPersistenceService<ActParticipation>>().Obsoleted += (o, e) => MemoryCache.Current.RemoveObject(typeof(Act), e.Data.SourceEntityKey);
            }
            if (ApplicationContext.Current.GetService<IDataPersistenceService<ActRelationship>>() != null)
            {
                ApplicationContext.Current.GetService<IDataPersistenceService<ActRelationship>>().Updated += (o, e) => MemoryCache.Current.RemoveObject(typeof(Act), e.Data.SourceEntityKey);
                ApplicationContext.Current.GetService<IDataPersistenceService<ActRelationship>>().Inserted += (o, e) => MemoryCache.Current.RemoveObject(typeof(Act), e.Data.SourceEntityKey);
                ApplicationContext.Current.GetService<IDataPersistenceService<ActRelationship>>().Obsoleted += (o, e) => MemoryCache.Current.RemoveObject(typeof(Act), e.Data.SourceEntityKey);
            }
            if (ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>() != null)
            {
                EventHandler<PostPersistenceEventArgs<EntityRelationship>> clearSource = (o, e) =>
                {
                    if (e.Data?.SourceEntity != null)
                        MemoryCache.Current.RemoveObject(e.Data.SourceEntity.GetType(), e.Data.SourceEntityKey);
                    else
                    {
                        MemoryCache.Current.RemoveObject(typeof(Patient), e.Data.SourceEntityKey);
                        MemoryCache.Current.RemoveObject(typeof(ManufacturedMaterial), e.Data.SourceEntityKey);
                        MemoryCache.Current.RemoveObject(typeof(Place), e.Data.SourceEntityKey);
                        MemoryCache.Current.RemoveObject(typeof(Organization), e.Data.SourceEntityKey);
                        MemoryCache.Current.RemoveObject(typeof(Provider), e.Data.SourceEntityKey);
                        MemoryCache.Current.RemoveObject(typeof(Material), e.Data.SourceEntityKey);
						MemoryCache.Current.RemoveObject(typeof(Person), e.Data.SourceEntityKey);
						MemoryCache.Current.RemoveObject(typeof(UserEntity), e.Data.SourceEntityKey);
						MemoryCache.Current.RemoveObject(typeof(Entity), e.Data.SourceEntityKey);
						MemoryCache.Current.RemoveObject(typeof(EntityRelationship), e.Data.Key);
                    }
                };
                ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>().Updated += clearSource;
                ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>().Inserted += clearSource;
                ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>().Obsoleted += clearSource;
            }

            // Now we start timers
            var timerService = ApplicationContext.Current.GetService<ITimerService>();
            if(timerService != null)
            {
                if (timerService.IsRunning)
                {
                    timerService.AddJob(new CacheCleanupTimerJob(), new TimeSpan(this.m_configuration.Types.Select(o => o.MaxCacheAge).Min()));
                    timerService.AddJob(new CacheRegulatorTimerJob(), new TimeSpan(0, 1, 0));
                }
                else
                    timerService.Started += (s, e) =>
                    {
                        timerService.AddJob(new CacheCleanupTimerJob(), new TimeSpan(this.m_configuration.Types.Select(o => o.MaxCacheAge).Min()));
                        timerService.AddJob(new CacheRegulatorTimerJob(), new TimeSpan(0, 1, 0));
                    };
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

        /// <summary>
        /// Get the specified cache item
        /// </summary>
        public object GetCacheItem(Type tdata, Guid key)
        {
            return MemoryCache.Current.TryGetEntry(tdata, key);
        }

        /// <summary>
        /// Add the specified item to the memory cache
        /// </summary>
        public void Add(IdentifiedData data)
        {
            MemoryCache.Current.AddUpdateEntry(data);
        }

        /// <summary>
        /// Remove the object from the cache
        /// </summary>
        public void Remove(Type tdata, Guid key)
        {
            MemoryCache.Current.RemoveObject(tdata, key);
        }
    }
}
