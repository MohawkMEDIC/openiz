using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
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

        // Trace source
        protected TraceSource m_traceSource = new TraceSource("OpenIZ.Persistence.Data.MSSQL.Services.Persistence");

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

            var assemblyTypes = typeof(SqlPersistenceService).Assembly.GetTypes();
            foreach (Type t in assemblyTypes.Where(o => o.GetInterface(typeof(IDataPersistenceService<>).FullName) != null))
            {
                this.m_traceSource.TraceInformation("Added PersistenceService {0}", t.FullName);
                this.m_serviceProviders.Add(t);
                ApplicationContext.Current.AddServiceProvider(t);
            }
            this.Started?.Invoke(this, EventArgs.Empty);
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
