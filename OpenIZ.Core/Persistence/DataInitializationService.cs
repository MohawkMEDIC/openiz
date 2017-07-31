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
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Security;
using OpenIZ.Core.Services;
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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Interfaces;
using System.ComponentModel;

namespace OpenIZ.Core.Persistence
{
    /// <summary>
    /// Data initialization service
    /// </summary>
    [Description("Dataset Installation Service")]
    public class DataInitializationService : IDaemonService, IReportProgressChanged
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
        /// Fired when progress changes
        /// </summary>
        public event EventHandler<Services.ProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// Event handler which fires after startup
        /// </summary>
        private EventHandler m_persistenceHandler;

        /// <summary>
        /// Install dataset
        /// </summary>
        public void InstallDataset(DatasetInstall ds)
        {
            try
            {

                this.m_traceSource.TraceInformation("Applying {0} ({1} objects)...", ds.Id, ds.Action.Count);

                int i = 0;
                foreach (var itm in ds.Action.Where(o=>o.Element != null))
                {

                    try
                    {
                        this.ProgressChanged?.Invoke(this, new Services.ProgressChangedEventArgs(i++ / (float)ds.Action.Count, ds.Id));

                        // IDP Type
                        Type idpType = typeof(IDataPersistenceService<>);
                        idpType = idpType.MakeGenericType(new Type[] { itm.Element.GetType() });
                        var idpInstance = ApplicationContext.Current.GetService(idpType) as IDataPersistenceService;

                        // Don't insert duplicates
                        var getMethod = idpType.GetMethod("Get");

                        this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "{0} {1}", itm.ActionName, itm.Element);

                        Object target = null, existing = null;
                        if (itm.Element.Key.HasValue)
                        {
                            ApplicationContext.Current.GetService<IDataCachingService>()?.Remove(itm.Element.Key.Value);
                            existing = idpInstance.Get(itm.Element.Key.Value);
                        }
                        if (existing != null)
                        {
                            if ((itm as DataInsert)?.SkipIfExists == true) continue;
                            target = (existing as IdentifiedData).Clone();
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

                        // ensure version sequence is set
                        // the reason this exists is because in FHIR code system values have the same mnemonic values in different code systems
                        // since we cannot insert duplicate mnemonic values, we want to associate the existing concept with the new reference term
                        // for the new code system
                        //
                        // i.e.
                        //
                        // Code System Address Use has the following codes:
                        // home
                        // work
                        // temp
                        // old
                        // we want to insert reference terms and concepts so we can find an associated concept
                        // for a given reference term and code system
                        // 
                        // Code System Contact Point Use has the following codes:
                        // home
                        // work
                        // temp
                        // old
                        // mobile
                        //
                        // we can insert new reference terms for these reference terms, but cannot insert new concept using the same values for the mnemonic
                        // so we associate the new reference term and the concept
                        if (target is IVersionedAssociation)
                        {
                            var ivr = target as IVersionedAssociation;

                            // Get the type this is bound to
                            Type stype = target.GetType();
                            while (!stype.IsGenericType || stype.GetGenericTypeDefinition() != typeof(VersionedAssociation<>))
                                stype = stype.BaseType;

                            ApplicationContext.Current.GetService<IDataCachingService>()?.Remove(ivr.SourceEntityKey.Value);
                            var idt = typeof(IDataPersistenceService<>).MakeGenericType(stype.GetGenericArguments()[0]);
                            var idp = ApplicationContext.Current.GetService(idt) as IDataPersistenceService;
                            ivr.EffectiveVersionSequenceId = (idp.Get(ivr.SourceEntityKey.Value) as IVersionedEntity)?.VersionSequence;
                            if (ivr.EffectiveVersionSequenceId == null)
                                throw new KeyNotFoundException($"Dataset contains a reference to an unkown source entity : {ivr.SourceEntityKey}");
                            target = ivr;
                        }

                        if (existing == null && (itm is DataInsert || (itm is DataUpdate && (itm as DataUpdate).InsertIfNotExists)))
                            idpInstance.Insert(target);
                        else if (!(itm is DataInsert))
                            typeof(IDataPersistenceService).GetMethod(itm.ActionName, new Type[] { typeof(Object) }).Invoke(idpInstance, new object[] { target });
                    }
                    catch
                    {
                        if (!itm.IgnoreErrors)
                            throw;
                    }
                }
                this.m_traceSource.TraceInformation("Applied {0} changes", ds.Action.Count);

            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, "Error applying dataset {0}: {1}", ds.Id, e);
                throw;
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public DataInitializationService()
        {
            this.m_persistenceHandler = (o, e) => {

                this.InstallDataDirectory();
            };
        }

        /// <summary>
        /// Install data directory contents
        /// </summary>
        public void InstallDataDirectory(EventHandler<Services.ProgressChangedEventArgs> fileProgress = null)
        {
            try
            {
                // Set system principal 
                AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);

                String dataDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Data");
                this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Scanning Directory {0} for datasets", dataDirectory);

                XmlSerializer xsz = new XmlSerializer(typeof(DatasetInstall));
                var datasetFiles = Directory.GetFiles(dataDirectory, "*.dataset");
                Array.Sort(datasetFiles);
                int i = 0;
                // Perform migrations
                foreach (var f in datasetFiles)
                {

                    try
                    {

                        var logFile = Path.ChangeExtension(f, "completed");
                        if (File.Exists(logFile))
                            continue; // skip

                        using (var fs = File.OpenRead(f))
                        {
                            var ds = xsz.Deserialize(fs) as DatasetInstall;
                            fileProgress?.Invoke(this, new Services.ProgressChangedEventArgs(++i / (float)datasetFiles.Length, ds.Id));
                            this.m_traceSource.TraceEvent(TraceEventType.Information, 0, "Installing {0}...", Path.GetFileName(f));
                            this.InstallDataset(ds);
                        }


                        File.Move(f, logFile);
                    }
                    catch (Exception ex)
                    {
                        this.m_traceSource.TraceEvent(TraceEventType.Error, ex.HResult, "Error applying {0}: {1}", f, ex);
                        throw;
                    }
                }
            }
            finally
            {
                this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Un-binding event handler");
                ApplicationContext.Current.Started -= this.m_persistenceHandler;
            }
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
