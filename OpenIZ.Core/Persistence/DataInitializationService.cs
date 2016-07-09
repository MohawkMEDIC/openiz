using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Persistence
{
    /// <summary>
    /// Data initialization service
    /// </summary>
    public class DataInitializationService : IDaemonService
    {

        // Trace source
        private TraceSource m_traceSource = new TraceSource(OpenIzConstants.DatasetInstallSourceName);

        /// <summary>
        /// True when the service is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Fired when the service has started
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Fired when the service is starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Fired when the service has stopped
        /// </summary>
        public event EventHandler Stopped;
        /// <summary>
        /// Fired when the service is stopping
        /// </summary>
        public event EventHandler Stopping;

        /// <summary>
        /// Event handler which fires after startup
        /// </summary>
        private EventHandler m_persistenceHandler;

        /// <summary>
        /// Ctor
        /// </summary>
        public DataInitializationService()
        {
            this.m_persistenceHandler = (o, e) => {

                String dataDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Data");
                this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Scanning Directory {0} for datasets", dataDirectory);

                XmlSerializer xsz = new XmlSerializer(typeof(DatasetInstall));
                // Perform migrations
                foreach(var f in Directory.GetFiles(dataDirectory, "*.dataset"))
                {
                    try
                    {
                        using (var fs = File.OpenRead(f))
                        {
                            var ds = xsz.Deserialize(fs) as DatasetInstall;
                            this.m_traceSource.TraceInformation("Applying {0}...", ds.Id);

                            foreach (var itm in ds.Action)
                            {
                                // Association
                                if(itm.Association != null)
                                    foreach(var ascn in itm.Association)
                                    {
                                        var pi = itm.Element.GetType().GetRuntimeProperty(ascn.PropertyName);
                                        var mi = pi.PropertyType.GetRuntimeMethod("Add", new Type[] { ascn.Element.GetType() });
                                        mi.Invoke(pi.GetValue(itm.Element), new object[] { ascn.Element });
                                    }

                                // IDP Type
                                Type idpType = typeof(IDataPersistenceService<>);
                                idpType = idpType.MakeGenericType(new Type[] { itm.Element.GetType() });
                                var svc = ApplicationContext.Current.GetService(idpType);

                                // Don't insert duplicates
                                var getMethod = idpType.GetMethod("Get");
                                var existing = getMethod.MakeGenericMethod(new Type[] { typeof(Guid) }).Invoke(svc, new object[] { new Identifier<Guid>(itm.Element.Key.Value), AuthenticationContext.SystemPrincipal, false });
                                if(existing == null && (itm is DataInsert || (itm is DataUpdate && (itm as DataUpdate).InsertIfNotExists)))
                                    idpType.GetMethod("Insert", new Type[] { itm.Element.GetType(), typeof(IPrincipal), typeof(TransactionMode) }).Invoke(svc, new object[] { itm.Element, AuthenticationContext.SystemPrincipal, TransactionMode.Commit });
                                else if(!(itm is DataInsert))
                                    idpType.GetMethod(itm.ActionName, new Type[] { itm.Element.GetType(), typeof(IPrincipal), typeof(TransactionMode) }).Invoke(svc, new object[] { itm.Element, AuthenticationContext.SystemPrincipal, TransactionMode.Commit });

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.m_traceSource.TraceEvent(TraceEventType.Error, ex.HResult, "Error applying {0}: {1}", f, ex);
                    }
                }
                this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Un-binding event handler");
                ApplicationContext.Current.Started -= this.m_persistenceHandler;
            };
        }

        /// <summary>
        /// Start the service
        /// </summary>
        public bool Start()
        {
            this.m_traceSource.TraceInformation("Binding to startup...");
            ApplicationContext.Current.Started += this.m_persistenceHandler;
            return true;
        }

        /// <summary>
        /// Stopped
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            return true;
        }
    }
}
