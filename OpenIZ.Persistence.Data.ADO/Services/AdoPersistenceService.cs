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
using System;

using System.Linq;
using System.Reflection;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Model;
using System.Collections.Generic;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Security;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Persistence.Data.ADO.Data;
using System.Security.Principal;
using System.Diagnostics;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Exceptions;
using OpenIZ.Persistence.Data.ADO.Configuration;
using System.Threading;
using OpenIZ.Core.Services;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Attributes;
using OpenIZ.Persistence.Data.ADO.Services.Persistence;

namespace OpenIZ.Persistence.Data.ADO.Services
{


    /// <summary>
    /// Represents a dummy service which just adds the persistence services to the context
    /// </summary>
    public class AdoPersistenceService : IDaemonService
    {

        private static ModelMapper s_mapper;
        private static AdoConfiguration s_configuration;
        // Cache
        private static Dictionary<Type, IAdoPersistenceService> s_persistenceCache = new Dictionary<Type, IAdoPersistenceService>();

        /// <summary>
        /// Get configuration
        /// </summary>
        public static AdoConfiguration GetConfiguration() { return s_configuration; }

        /// <summary>
        /// Gets the mode mapper
        /// </summary>
        /// <returns></returns>
        public static ModelMapper GetMapper() { return s_mapper; }

        /// <summary>
        /// Get the specified persister type
        /// </summary>
        public static IAdoPersistenceService GetPersister(Type tDomain)
        {
            IAdoPersistenceService retVal = null;
            if(!s_persistenceCache.TryGetValue(tDomain, out retVal))
            {
                var idpType = typeof(IDataPersistenceService<>).MakeGenericType(tDomain);
                retVal = ApplicationContext.Current.GetService(idpType) as IAdoPersistenceService;
                if (retVal != null)
                    lock (s_persistenceCache)
                        if (!s_persistenceCache.ContainsKey(tDomain))
                            s_persistenceCache.Add(tDomain, retVal);
            }
            return retVal;
        }

        /// <summary>
        /// Static CTOR
        /// </summary>
        static AdoPersistenceService()
        {
            var tracer = new TraceSource(AdoDataConstants.TraceSourceName);
            s_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(AdoDataConstants.ConfigurationSectionName) as AdoConfiguration;
            try
            {
                s_mapper = new ModelMapper(typeof(AdoPersistenceService).GetTypeInfo().Assembly.GetManifestResourceStream(AdoDataConstants.MapResourceName));
            }
            catch (ModelMapValidationException ex)
            {
                tracer.TraceEvent(TraceEventType.Error, ex.HResult, "Error validating model map: {0}", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                tracer.TraceEvent(TraceEventType.Error, ex.HResult, "Error validating model map: {0}", ex);
                throw ex;
            }
        }

        
        /// <summary>
        /// Generic versioned persister service for any non-customized persister
        /// </summary>
        internal class GenericBasePersistenceService<TModel, TDomain> : BaseDataPersistenceService<TModel, TDomain>
            where TDomain : class, IDbBaseData, new()
            where TModel : BaseEntityData, new()
        {

            /// <summary>
            /// Ensure exists
            /// </summary>
            public override TModel Insert(DataContext context, TModel data, IPrincipal principal)
            {
                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    if (rp.GetCustomAttribute<DataIgnoreAttribute>() != null)
                        continue;

                    var instance = rp.GetValue(data);
                    if (instance != null)
                        data.UpdateParentKeys(rp);
                }
                return base.Insert(context, data, principal);
            }

            /// <summary>
            /// Update the specified object
            /// </summary>
            public override TModel Update(DataContext context, TModel data, IPrincipal principal)
            {
                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    if (rp.GetCustomAttribute<DataIgnoreAttribute>() != null)
                        continue;

                    var instance = rp.GetValue(data);
                    if (instance != null)
                        DataModelExtensions.EnsureExists(instance as IdentifiedData, context, principal);
                }
                return base.Update(context, data, principal);
            }
        }

        /// <summary>
        /// Generic versioned persister service for any non-customized persister
        /// </summary>
        internal class GenericIdentityPersistenceService<TModel, TDomain> : IdentifiedPersistenceService<TModel, TDomain>
            where TModel : IdentifiedData, new()
            where TDomain : class, IDbIdentified, new()
        {
            /// <summary>
            /// Ensure exists
            /// </summary>
            public override TModel Insert(DataContext context, TModel data, IPrincipal principal)
            {
                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    if (rp.GetCustomAttribute<DataIgnoreAttribute>() != null)
                        continue;

                    var instance = rp.GetValue(data);
                    if (instance != null)
                        DataModelExtensions.EnsureExists(instance as IdentifiedData, context, principal);
                }
                return base.Insert(context, data, principal);
            }

            /// <summary>
            /// Update the specified object
            /// </summary>
            public override TModel Update(DataContext context, TModel data, IPrincipal principal)
            {
                foreach (var rp in typeof(TModel).GetRuntimeProperties().Where(o => typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(o.PropertyType.GetTypeInfo())))
                {
                    if (rp.GetCustomAttribute<DataIgnoreAttribute>() != null)
                        continue;

                    var instance = rp.GetValue(data);
                    if (instance != null)
                        DataModelExtensions.EnsureExists(instance as IdentifiedData, context, principal);
                }
                return base.Update(context, data, principal);
            }
        }

        // Tracer
        private TraceSource m_tracer = new TraceSource(AdoDataConstants.TraceSourceName);

        // When service is running
        private bool m_running = false;
        /// <summary>
        /// Service is starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Service is stopping
        /// </summary>
        public event EventHandler Stopping;
        /// <summary>
        /// Service has started
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Service has stopped
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// True when the service is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_running;
            }
        }

        /// <summary>
        /// Start the service and bind all of the sub-services
        /// </summary>
        public bool Start()
        {
            // notify startup
            this.Starting?.Invoke(this, EventArgs.Empty);
            if (this.m_running) return true;

            // Verify schema version
            using (DataContext mdc = s_configuration.Provider.GetReadonlyConnection())
            {
                Version dbVer = new Version(mdc.FirstOrDefault<String>("get_sch_vrsn")),
                    oizVer = typeof(AdoPersistenceService).Assembly.GetName().Version;

                if (oizVer < dbVer)
                    throw new InvalidOperationException(String.Format("Invalid Schema Version. OpenIZ version {0} is older than the database schema version {1}", oizVer, dbVer));
                this.m_tracer.TraceInformation("OpenIZ Schema Version {0}", dbVer);
            }

            // Iterate the persistence services
            foreach (var t in typeof(AdoPersistenceService).GetTypeInfo().Assembly.ExportedTypes.Where(o => o.Namespace == "OpenIZ.Persistence.Data.ADO.Services.Persistence" && !o.GetTypeInfo().IsAbstract))
            {
                try
                {
                    this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "Loading {0}...", t.AssemblyQualifiedName);
                    ApplicationContext.Current.AddServiceProvider(t);

                    // Add to cache since we're here anyways
                    s_persistenceCache.Add(t.GetGenericArguments()[0], Activator.CreateInstance(t) as IAdoPersistenceService);
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error adding service {0} : {1}", t.AssemblyQualifiedName, e);
                }
            }

            // Now iterate through the map file and ensure we have all the mappings, if a class does not exist create it
            try
            {
                this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "Creating secondary model maps...");

                var map = ModelMap.Load(typeof(AdoPersistenceService).GetTypeInfo().Assembly.GetManifestResourceStream("OpenIZ.Persistence.Data.ADO.Data.ModelMap.xml"));
                foreach (var itm in map.Class)
                {
                    // Is there a persistence service?
                    var idpType = typeof(IDataPersistenceService<>);
                    Type modelClassType = Type.GetType(itm.ModelClass),
                        domainClassType = Type.GetType(itm.DomainClass);
                    idpType = idpType.MakeGenericType(modelClassType);

                    if (modelClassType.IsAbstract || domainClassType.IsAbstract) continue;

                    // Already created
                    if (ApplicationContext.Current.GetService(idpType) != null)
                        continue;

                    this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "Creating map {0} > {1}", modelClassType, domainClassType);


                    if (modelClassType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IBaseEntityData)) &&
                        domainClassType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDbBaseData)))
                    {
                        // Construct a type
                        var pclass = typeof(GenericBasePersistenceService<,>);
                        pclass = pclass.MakeGenericType(modelClassType, domainClassType);
                        ApplicationContext.Current.AddServiceProvider(pclass);
                        // Add to cache since we're here anyways
                        s_persistenceCache.Add(modelClassType, Activator.CreateInstance(pclass) as IAdoPersistenceService);
                    }
                    else
                    {
                        // Construct a type
                        var pclass = typeof(GenericIdentityPersistenceService<,>);
                        pclass = pclass.MakeGenericType(modelClassType, domainClassType);
                        ApplicationContext.Current.AddServiceProvider(pclass);
                        s_persistenceCache.Add(modelClassType, Activator.CreateInstance(pclass) as IAdoPersistenceService);
                    }

                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error initializing local persistence: {0}", e);
                throw e;
            }

            // Bind some basic service stuff
            ApplicationContext.Current.GetService<IDataPersistenceService<Core.Model.Security.SecurityUser>>().Inserting += (o, e) =>
            {
                if (String.IsNullOrEmpty(e.Data.SecurityHash))
                    e.Data.SecurityHash = Guid.NewGuid().ToString();
            };
            ApplicationContext.Current.GetService<IDataPersistenceService<Core.Model.Security.SecurityUser>>().Updating += (o, e) =>
            {
                e.Data.SecurityHash = Guid.NewGuid().ToString();
            };

            // Attempt to cache concepts
            this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "Caching concept dictionary...");
            if (ApplicationContext.Current.GetService<IDataCachingService>() != null)
                new Thread((o) =>
                {
                    int t;
                    ApplicationContext.Current.GetService<IDataPersistenceService<Core.Model.DataTypes.Concept>>().Query(c => c.Key == c.Key, 0, 10000, null, out t);
                    ApplicationContext.Current.GetService<IDataPersistenceService<Core.Model.DataTypes.ConceptSet>>().Query(c => c.Key == c.Key, 0, 1000, null, out t);

                }).Start();
            this.m_running = true;
            this.Started?.Invoke(this, EventArgs.Empty);

            return true;
        }

        /// <summary>
        /// Stop the service
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);
            this.m_running = false;
            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}

