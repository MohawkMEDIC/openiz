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
 * Date: 2017-6-10
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.BusinessRules.JavaScript;
using OpenIZ.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using OpenIZ.Core.Interfaces;
using System.ComponentModel;
using OpenIZ.Core.Model;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Services;
using System.Threading;
using Newtonsoft.Json;
using OpenIZ.Core.Diagnostics;
using System.Diagnostics.Tracing;
using OpenIZ.BusinessRules.JavaScript.JNI;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using OpenIZ.Core.Applets.ViewModel.Json;
using OpenIZ.Core.Model.Acts;
using System.Linq.Expressions;
using OpenIZ.Protocol.Xml;
using OpenIZ.Protocol.Xml.Model;
using OpenIZ.Core.Model.Roles;
using System.Diagnostics;
using OpenIZ.Core.Protocol;
using OpenIZ.Core.Services.Impl;

namespace OizDevTool.Debugger
{
    /// <summary>
    /// Business Rules debugger
    /// </summary>
    [Description("Care Plan Debugger")]
    public class ProtoDebugger : DebuggerBase
    {

        // Loaded files
        private Dictionary<String, String> m_loadedFiles = new Dictionary<string, string>();

        /// <summary>
        /// Thread event
        /// </summary>
        private ManualResetEvent m_resetEvent = new ManualResetEvent(false);

        public class ConsoleTraceWriter : TraceWriter
        {
            /// <summary>
            /// Write
            /// </summary>
            public ConsoleTraceWriter(EventLevel filter, string initializationData) : base(filter, initializationData)
            {
            }

            protected override void WriteTrace(EventLevel level, string source, string format, params object[] args)
            {
                if (source == typeof(JsConsoleProvider).FullName)
                    Console.WriteLine(format, args);
            }

        }

        /// <summary>
        /// Service manager
        /// </summary>
        private class ServiceManager : IServiceManager
        {
            /// <summary>
            /// Get a service 
            /// </summary>
            public void AddServiceProvider(Type serviceType)
            {
                ApplicationContext.Current.AddServiceProvider(serviceType);
            }

            /// <summary>
            /// Get all services
            /// </summary>
            public IEnumerable<object> GetServices()
            {
                return ApplicationContext.Current.GetServices();
            }

            /// <summary>
            /// Remove service provider
            /// </summary>
            public void RemoveServiceProvider(Type serviceType)
            {
                ApplicationContext.Current.RemoveServiceProvider(serviceType);
            }
        }

        /// <summary>
        /// File system resolver
        /// </summary>
        private class FileSystemResolver : IDataReferenceResolver
        {
            public String RootDirectory { get; set; }

            public FileSystemResolver()
            {
                this.RootDirectory = Environment.CurrentDirectory;
            }

            /// <summary>
            /// Resolve specified reference
            /// </summary>
            public Stream Resolve(string reference)
            {
                reference = reference.Replace("~", this.RootDirectory);
                if (File.Exists(reference))
                    return File.OpenRead(reference);
                else
                {
                    Console.Error.WriteLine("ERR: {0}", reference);
                    return null;
                }
            }
        }

        /// <summary>
        /// Class for protocol debugging
        /// </summary>
        private class DebugProtocolRepository : IClinicalProtocolRepositoryService
        {

            // Protocols
            private List<Protocol> m_protocols = new List<Protocol>();

            /// <summary>
            /// Find a protocol
            /// </summary>
            public IEnumerable<Protocol> FindProtocol(Expression<Func<Protocol, bool>> predicate, int offset, int? count, out int totalResults)
            {
                totalResults = 0;
                return this.m_protocols.Where(predicate.Compile()).Skip(offset).Take(count ?? 100);
            }

            /// <summary>
            /// Insert protocol into the provider
            /// </summary>
            public Protocol InsertProtocol(Protocol data)
            {
                this.m_protocols.Add(data);
                return data;
            }
        }

        /// <summary>
        /// BRE debugger
        /// </summary>
        /// <param name="sources"></param>
        public ProtoDebugger(StringCollection sources, String rootPath) : base(rootPath)
        {

            ApplicationContext.Current.AddServiceProvider(typeof(FileConfigurationService));
            ApplicationServiceContext.Current = ApplicationContext.Current;
            try
            {
                ApplicationContext.Current.RemoveServiceProvider(typeof(IClinicalProtocolRepositoryService));
            }
            catch { }

            ApplicationContext.Current.AddServiceProvider(typeof(FileSystemResolver));
            ApplicationContext.Current.AddServiceProvider(typeof(ServiceManager));
            ApplicationContext.Current.AddServiceProvider(typeof(DebugProtocolRepository));
            Tracer.AddWriter(new ConsoleTraceWriter(EventLevel.LogAlways, "dbg"), EventLevel.LogAlways);
            ApplicationContext.Current.Start();

            if (!String.IsNullOrEmpty(rootPath))
                ApplicationContext.Current.GetService<FileSystemResolver>().RootDirectory = rootPath;
            rootPath = ApplicationContext.Current.GetService<FileSystemResolver>().RootDirectory;

            // Load debug targets
            Console.WriteLine("Loading debuggees...");
            if (sources != null)
                foreach (var rf in sources)
                {
                    var f = rf.Replace("~", rootPath);
                    if (!File.Exists(f))
                        Console.Error.WriteLine("Can't find file {0}", f);
                    else
                        this.Add(f);
                }

        }

        /// <summary>
        /// Loads a script to be debugged
        /// </summary>
        [Command("o", "Adds a protocol file to the planner")]
        public void Add(String file)
        {
            if (!Path.IsPathRooted(file))
                file = Path.Combine(this.m_workingDirectory, file);
            if (!File.Exists(file))
                throw new FileNotFoundException(file);

            // Add
            using (var fs = File.OpenRead(file))
            {
                var protoSource = ProtocolDefinition.Load(fs);
                var proto = new XmlClinicalProtocol(protoSource);
                ApplicationContext.Current.GetService<IClinicalProtocolRepositoryService>().InsertProtocol(proto.GetProtocolData());
            }
        }

        /// <summary>
        /// Clear protocol repository
        /// </summary>
        [Command("c", "Clear the protocol repository")]
        public void Clear()
        {
            ApplicationContext.Current.RemoveServiceProvider(typeof(ICarePlanService));
            ApplicationContext.Current.RemoveServiceProvider(typeof(IClinicalProtocolRepositoryService));
            ApplicationContext.Current.AddServiceProvider(typeof(DebugProtocolRepository));
            ApplicationContext.Current.AddServiceProvider(typeof(SimpleCarePlanService));

        }

        /// <summary>
        /// Loads a script to be debugged
        /// </summary>
        [Command("od", "Adds all files dir to care planner")]
        public void AddDir(String dir)
        {
            if (!Path.IsPathRooted(dir))
                dir = Path.Combine(this.m_workingDirectory, dir);
            if (!Directory.Exists(dir))
                throw new FileNotFoundException(dir);

            // Add
            foreach (var file in Directory.GetFiles(dir))
            {
                Console.WriteLine("Add {0}", Path.GetFileName(file));
                try
                {
                    using (var fs = File.OpenRead(file))
                    {
                        var protoSource = ProtocolDefinition.Load(fs);
                        var proto = new XmlClinicalProtocol(protoSource);
                        
                        ApplicationContext.Current.GetService<IClinicalProtocolRepositoryService>().InsertProtocol(proto.GetProtocolData());
                    }
                }
                catch(Exception e)
                {
                    base.PrintStack(e);
                }
            }
        }


        /// <summary>
        /// List all protocols
        /// </summary>
        [Command("pl", "Displays a list of clinical protocols that have been loaded in this session")]
        public void ListProtocols()
        {
            Console.WriteLine("ID#{0}NAME", new String(' ', 38));
            int t;
            foreach (var itm in ApplicationContext.Current.GetService<ICarePlanService>().Protocols)
                Console.WriteLine("{0}    {1}", itm.Id, itm.Name);
        }

        /// <summary>
        /// Set a breakpoint
        /// </summary>
        [Command("go", "Runs the clinical protocols to construct a care plan")]
        public object Run()
        {

            var cpService = ApplicationContext.Current.GetService<ICarePlanService>();
            if (cpService == null)
                throw new InvalidOperationException("No care plan service is registered");
            else if (this.m_scopeObject is Patient)
            {
                Console.WriteLine("Running care planner...");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var cp = cpService.CreateCarePlan(this.m_scopeObject as Patient);
                sw.Stop();
                Console.WriteLine("Care plan generated in {0}", sw.Elapsed);
                return cp;
            }
            else
                throw new InvalidOperationException("Scope must be a patient object");
        }


        /// <summary>
        /// Set a breakpoint
        /// </summary>
        [Command("go.encounter", "Runs the clinical protocols to construct a care plan as appointments")]
        public object RunEncounter()
        {

            var cpService = ApplicationContext.Current.GetService<ICarePlanService>();
            if (cpService == null)
                throw new InvalidOperationException("No care plan service is registered");
            else if (this.m_scopeObject is Patient)
            {
                Console.WriteLine("Running care planner...");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var cp = cpService.CreateCarePlan(this.m_scopeObject as Patient);
                sw.Stop();
                Console.WriteLine("Care plan generated in {0}", sw.Elapsed);
                return cp;
            }
            else
                throw new InvalidOperationException("Current scope must be a patient");
        }


        /// <summary>
        /// Exit the specified context
        /// </summary>
        public override void Exit()
        {
            base.Exit();
        }
    }

}
