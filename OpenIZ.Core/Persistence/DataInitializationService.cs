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
 * Date: 2016-7-18
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Reflection;
using OpenIZ.Core.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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

                try
                {
                    String dataDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Data");
                    this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Scanning Directory {0} for datasets", dataDirectory);

                    XmlSerializer xsz = new XmlSerializer(typeof(DatasetInstall));
                    var datasetFiles = Directory.GetFiles(dataDirectory, "*.dataset");
                    Array.Sort(datasetFiles);
                    // Perform migrations
                    foreach (var f in datasetFiles)
                    {
						if (f.Contains("000-NullFlavor"))
						{
							continue;
						}

                        try
                        {

                            var logFile = Path.ChangeExtension(f, "completed");
                            if (File.Exists(logFile))
                                continue; // skip

                            using (var fs = File.OpenRead(f))
                            {
                                var ds = xsz.Deserialize(fs) as DatasetInstall;
                                this.m_traceSource.TraceInformation("Applying {0} ({1} objects)...", ds.Id, ds.Action.Count);

                                foreach (var itm in ds.Action)
                                {
                                   
                                    // IDP Type
                                    Type idpType = typeof(IDataPersistenceService<>);
                                    idpType = idpType.MakeGenericType(new Type[] { itm.Element.GetType() });
                                    var idpInstance = ApplicationContext.Current.GetService(idpType);

                                    // Don't insert duplicates
                                    var getMethod = idpType.GetMethod("Get");

                                    this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "{0} {1}", itm.ActionName, itm.Element);

                                    Object target = null, existing = null;
                                    if(itm.Element.Key.HasValue)
                                        existing = getMethod.MakeGenericMethod(new Type[] { typeof(Guid) }).Invoke(idpInstance, new object[] { new Identifier<Guid>(itm.Element.Key.Value), AuthenticationContext.SystemPrincipal, false });
                                    if (existing != null)
                                    {
                                        target = existing;
                                        target.CopyObjectData(itm.Element);
                                    }
                                    else
                                        target = itm.Element;

                                   
                                    // Association
                                    if (itm.Association != null)
                                        foreach (var ascn in itm.Association)
                                        {
                                            var pi = target.GetType().GetRuntimeProperty(ascn.PropertyName);
                                            var mi = pi.PropertyType.GetRuntimeMethod("Add", new Type[] { ascn.Element.GetType() });
                                            mi.Invoke(pi.GetValue(target), new object[] { ascn.Element });
                                        }

                                    if (existing == null && (itm is DataInsert || (itm is DataUpdate && (itm as DataUpdate).InsertIfNotExists)))
                                        idpType.GetMethod("Insert", new Type[] { itm.Element.GetType(), typeof(IPrincipal), typeof(TransactionMode) }).Invoke(idpInstance, new object[] { target, AuthenticationContext.SystemPrincipal, TransactionMode.Commit });
                                    else if (!(itm is DataInsert))
                                        idpType.GetMethod(itm.ActionName, new Type[] { itm.Element.GetType(), typeof(IPrincipal), typeof(TransactionMode) }).Invoke(idpInstance, new object[] { target, AuthenticationContext.SystemPrincipal, TransactionMode.Commit });

                                }
                                this.m_traceSource.TraceInformation("Applied {0} changes", ds.Action.Count);

                            }


                            File.Move(f, logFile);
                        }
                        catch (Exception ex)
                        {
                            this.m_traceSource.TraceEvent(TraceEventType.Error, ex.HResult, "Error applying {0}: {1}", f, ex);
                        }
                    }
                }
                finally
                {
                    this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Un-binding event handler");
                    ApplicationContext.Current.Started -= this.m_persistenceHandler;
                }
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
