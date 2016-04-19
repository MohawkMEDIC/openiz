/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-20
 */
using MARC.Everest.Threading;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// SQL Persistence daemon service ensures all the persistence classes in this
    /// assembly are added to the current application context
    /// </summary>
    public class SqlPersistenceService : IDaemonService
    {
        // Service providers
        private List<Type> m_serviceProviders = new List<Type>();

        /// <summary>
        /// Represents a source of trace logs from this object
        /// </summary>
        protected TraceSource m_traceSource = new TraceSource(SqlServerConstants.PersistenceTraceSourceName);

        /// <summary>
        /// Whether the daemon is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_serviceProviders.Count > 0;
            }
        }

        /// <summary>
        /// Persistence services registered
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Persistence services are being registered
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Persistence services are stopped
        /// </summary>
        public event EventHandler Stopped;
        /// <summary>
        /// Persistence services are stopping
        /// </summary>
        public event EventHandler Stopping;

        /// <summary>
        /// Registers all persistence service
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            this.m_traceSource.TraceInformation("Compiling model map...");
            var cache = DataCache.Current; // Force load map

            var assemblyTypes = typeof(SqlPersistenceService).Assembly.GetTypes();
            foreach (Type t in assemblyTypes.Where(o => o.GetInterface(typeof(IDataPersistenceService<>).FullName) != null))
            {
                this.m_traceSource.TraceInformation("Added PersistenceService {0}", t.FullName);
                this.m_serviceProviders.Add(t);
                ApplicationContext.Current.AddServiceProvider(t);
            }
            this.Started?.Invoke(this, EventArgs.Empty);

            // Load system concepts into cache
            if (DataCache.Current.IsCacheEnabled)
            {
                this.m_traceSource.TraceInformation("Loading system concepts into data cache...");
                var concepts = new ConceptPersistenceService().Query(o => o.IsSystemConcept, null);
                m_traceSource.TraceInformation("{0} items loaded into cache", DataCache.Current.Count);
            }
            return true;
        }

        /// <summary>
        /// Un-register the handlers
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);

            foreach (Type t in this.m_serviceProviders)
                ApplicationContext.Current.RemoveServiceProvider(t);
            this.m_serviceProviders.Clear();
            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
