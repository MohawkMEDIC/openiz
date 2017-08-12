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
 * User: khannan
 * Date: 2017-7-26
 */
using OpenIZ.AdminConsole.Attributes;
using OpenIZ.Messaging.AMI.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.AdminConsole.Shell.CmdLets
{
    /// <summary>
    /// Represents the server information commandlet
    /// </summary>
    [AdminCommandlet]
    public static class ServerInfoCmdlet
    {

        // Ami client
        private static AmiServiceClient m_client = new AmiServiceClient(ApplicationContext.Current.GetRestClient(Core.Interop.ServiceEndpointType.AdministrationIntegrationService));

        /// <summary>
        /// Get server information
        /// </summary>
        public static void Init()
        {
            try
            {
                var diagReport = m_client.GetServerDiagnoticReport().ApplicationInfo;
                Console.WriteLine("* {0} -> v.{1} ({2})", m_client.Client.Description.Endpoint[0].Address, diagReport.Version, diagReport.InformationalVersion);
            }
            catch { }
        }


        /// <summary>
        /// Get diagnostic info from server
        /// </summary>
        [AdminCommand("sinfo", "Gets diagnostic information from the server")]
        public static void ServerVersionQuery()
        {
            var diagReport = m_client.GetServerDiagnoticReport().ApplicationInfo;

            Console.WriteLine("Diagnostic Report for {0}", m_client.Client.Description.Endpoint[0].Address);
            Console.WriteLine("Server Reports As:\r\n {0} v. {2} ({3}) \r\n {4}", diagReport.Name, diagReport.Product, diagReport.Version, diagReport.InformationalVersion, diagReport.Copyright);
        }

        /// <summary>
        /// Get assembly info from server
        /// </summary>
        [AdminCommand("sasm", "Shows the server assembly information")]
        public static void ServerAssemblyQuery()
        {
            var diagReport = m_client.GetServerDiagnoticReport().ApplicationInfo;

            // Loaded assemblies
            Console.WriteLine("Assemblies:\r\nAssembly{0}Version    Information", new String(' ', 22));
            foreach(var itm in diagReport.Assemblies)
            {
                if (itm.Name == "Microsoft.GeneratedCode") continue;
                Console.WriteLine("{0}{1}{2}{3}{4}",
                    itm.Name.Length > 28 ? itm.Name.Substring(0, 28) : itm.Name,
                    itm.Name.Length > 28 ? "  " : new string(' ', 30 - itm.Name.Length),
                    itm.Version.Length > 10 ? itm.Version.Substring(0, 10) : itm.Version,
                    itm.Version.Length > 10 ? " " : new string(' ', 11 - itm.Version.Length),
                    itm.Info?.Length > 50 ? itm.Info?.Substring(0, 50) : itm.Info);
            }
        }

        /// <summary>
        /// Get assembly info from server
        /// </summary>
        [AdminCommand("svci", "Shows the server service information")]
        [Description("This command will show the running daemon services in the connected IMS instance")]
        public static void ServiceInformation()
        {
            var diagReport = m_client.GetServerDiagnoticReport().ApplicationInfo;

            // Loaded assemblies
            Console.WriteLine("Services:\r\nService{0}Status", new String(' ', 35));
            foreach (var itm in diagReport.ServiceInfo)
            {
                string name = itm.Description ?? itm.Type;
                Console.WriteLine("{0}{1}{2}",
                    name.Length > 37 ? name.Substring(0, 37) + "..." : name,
                    name.Length > 37 ? "  " : new string(' ', 42 - name.Length),
                    itm.IsRunning ? "Running" : "Stopped");
            }
        }

    }
}
