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
 * Date: 2016-6-14
 */
using MARC.HI.EHRS.SVC.Core;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Model.EntityLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core;
using OpenIZ.Core.Services.Impl;
using OpenIZ.Core.Security;
using OpenIZ.AdminConsole.Shell;

namespace OpenIZ
{
    /// <summary>
    /// Guid for the service
    /// </summary>
    [Guid("21F35B18-E417-4F8E-B9C7-73E98B7C71B8")]
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(String[] args)
        {

            AppDomain.CurrentDomain.SetData(
               "DataDirectory",
               Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "Data"));

            // Handle Unahndled exception
            AppDomain.CurrentDomain.UnhandledException += (o, e) => {
                var emergencyString = MARC.HI.EHRS.SVC.Core.ApplicationContext.Current?.GetLocaleString("01189998819991197253");
                Trace.TraceError(emergencyString ?? "FATAL ERROR", e.ExceptionObject);
                Environment.Exit(999);
            };

            // Parser
            ParameterParser<ConsoleParameters> parser = new ParameterParser<ConsoleParameters>();

            // Trace copyright information
            Assembly entryAsm = Assembly.GetEntryAssembly();

            bool hasConsole = true;

            // Dump some info
            Trace.TraceInformation("OpenIZ Startup : v{0}", entryAsm.GetName().Version);
            Trace.TraceInformation("OpenIZ Working Directory : {0}", entryAsm.Location);
            Trace.TraceInformation("Operating System: {0} {1}", Environment.OSVersion.Platform, Environment.OSVersion.VersionString);
            Trace.TraceInformation("CLI Version: {0}", Environment.Version);

            try
            {
                var parameters = parser.Parse(args);
                EntitySource.Current = new EntitySource(new PersistenceServiceEntitySource());

                // What to do?
                if (parameters.ShowHelp)
                    parser.WriteHelp(Console.Out);
                else if(parameters.ConsoleMode)
                {
#if DEBUG
                    Core.Diagnostics.Tracer.AddWriter(new Core.Diagnostics.LogTraceWriter(System.Diagnostics.Tracing.EventLevel.LogAlways, "OpenIZ.data"), System.Diagnostics.Tracing.EventLevel.LogAlways);
#else
                    Core.Diagnostics.Tracer.AddWriter(new Core.Diagnostics.LogTraceWriter(System.Diagnostics.Tracing.EventLevel.LogAlways, "OpenIZ.data"), System.Diagnostics.Tracing.EventLevel.Warning);
#endif

                    Console.WriteLine("Open Immunize (OpenIZ) {0} ({1})", entryAsm.GetName().Version, entryAsm.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
                    Console.WriteLine("{0}", entryAsm.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright);
                    Console.WriteLine("Complete Copyright information available at http://openiz.codeplex.com/wikipage?title=Contributions");
                    ServiceUtil.Start(typeof(Program).GUID);
                    ApplicationServiceContext.Current = MARC.HI.EHRS.SVC.Core.ApplicationContext.Current;
                    MARC.HI.EHRS.SVC.Core.ApplicationContext.Current.AddServiceProvider(typeof(FileConfigurationService));
                    ApplicationServiceContext.HostType = OpenIZHostType.Server;
                    if (!parameters.StartupTest)
                    {
                        Console.WriteLine("Press [ENTER] to stop...");
                        var cInput = Console.ReadLine();
                        if (new MD5PasswordHashingService().EncodePassword(cInput) == "ff0a95a168055e0156f9200658e58a4c")
                        {
                            AdminConsole.Shell.ApplicationContext.Initialize(new AdminConsole.Parameters.ConsoleParameters()
                            {
                                AppId = "org.openiz.administration",
                                AppSecret = "Mohawk123"
                            });
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Blue;

                            if (AdminConsole.Shell.ApplicationContext.Current.Start())
                            {
                                new InteractiveShell().Exec();
                            }
                            else
                            {
                                Console.WriteLine("Cannot start admin console... Press [ENTER] to terminate...");
                                Console.ReadLine();
                            }
                        }
                    }
                    Console.WriteLine("Shutting down service...");
                    ServiceUtil.Stop();
                }
                else
                {
                    hasConsole = false;
                    ServiceBase[] servicesToRun = new ServiceBase[] { new OpenIZ() };
                    ServiceBase.Run(servicesToRun);
                }
            }
            catch(Exception e)
            {
#if DEBUG
                Trace.TraceError("011 899 981 199 911 9725 3!!! {0}", e.ToString());
                if (hasConsole)
                    Console.WriteLine("011 899 981 199 911 9725 3!!! {0}", e.ToString());
#else
                Trace.TraceError("Error encountered: {0}. Will terminate", e.Message);
#endif
                Environment.Exit(911);

            }

        }
    }
}
