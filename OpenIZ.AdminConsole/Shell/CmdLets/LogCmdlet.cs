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
 * Date: 2017-8-8
 */
using LogViewer;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.AdminConsole.Attributes;
using OpenIZ.Messaging.AMI.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenIZ.AdminConsole.Shell.CmdLets
{
    /// <summary>
    /// Represents a commandlet that deals with log files
    /// </summary>
    [AdminCommandlet]
    public static class LogCmdlet
    {

        /// <summary>
        /// Display options
        /// </summary>
        public class DisplayOptions
        {
            /// <summary>
            /// Gets or sets the number of lines
            /// </summary>
            [Parameter("n")]
            [Description("Limit output to n lines")]
            public string NumLines { get; set; }

            /// <summary>
            /// Search in the string
            /// </summary>
            [Parameter("g")]
            [Description("Greps the log for the matching pattern")]
            public string Grep { get; set; }

            /// <summary>
            /// Tails 
            /// </summary>
            [Parameter("t")]
            [Description("Starts from the end of the file")]
            public bool Tail { get; set; }

            /// <summary>
            /// Heads
            /// </summary>
            [Parameter("h")]
            [Description("Starts from the start of the file")]
            public bool Head { get; set; }

            /// <summary>
            /// Pages the output
            /// </summary>
            [Parameter("p")]
            [Description("Pagination")]
            public bool Page { get; set; }

            /// <summary>
            /// Fiter on event type
            /// </summary>
            [Parameter("f")]
            [Description("Filters based on event type")]
            public string Filter { get; set; }

            /// <summary>
            /// Raw print
            /// </summary>
            [Parameter("r")]
            [Description("Output RAW log data (don't parse)")]
            public bool RawPrint { get; set; }

            /// <summary>
            /// Output the entire log file
            /// </summary>
            [Parameter("a")]
            [Description("Output the entire log file")]
            public bool All { get; set; }
        }

        // Ami client
        private static AmiServiceClient m_client = new AmiServiceClient(ApplicationContext.Current.GetRestClient(Core.Interop.ServiceEndpointType.AdministrationIntegrationService));

        /// <summary>
        /// List logs
        /// </summary>
        [AdminCommand("dmesg", "Outputs the most recent log file")]
        public static void Dmesg(DisplayOptions logOptions)
        {

            var loginfo = m_client.GetLogs().CollectionItem.OrderByDescending(o => o.LastWrite).FirstOrDefault();
            var log = m_client.GetLog(loginfo.Name);

            using (var ms = new MemoryStream(log.Contents))
            using (var sr = new StreamReader(ms))
                if (!logOptions.RawPrint)
                    PrintFiltered(LogEvent.Load(sr), logOptions);
                else
                    PrintRaw(sr, logOptions);
        }

        /// <summary>
        /// List logs
        /// </summary>
        [AdminCommand("loglist", "Lists the available log files")]
        public static void LogList()
        {

            var loginfo = m_client.GetLogs();
            foreach(var it in loginfo.CollectionItem)
            {
                string strSize = (it.Size / 1024).ToString("0");
                Console.WriteLine("{0}\t\t{1} kb{2}{3:o}", it.Name, strSize, new String(' ',10 - strSize.Length), it.LastWrite);
            }
        }

        /// <summary>
        /// Gets or sets the log name
        /// </summary>
        public class LogCatDisplayOptions : DisplayOptions
        {

            /// <summary>
            /// Gets or sets the log name
            /// </summary>
            [Parameter("*")]
            [Description("The name of the log to display")]
            public StringCollection LogName { get; set; }
        }

        /// <summary>
        /// List logs
        /// </summary>
        [AdminCommand("logcat", "Lists the contents of the specified file")]
        public static void LogCat(LogCatDisplayOptions logOptions)
        {

            foreach (var l in logOptions.LogName)
            {
                var log = m_client.GetLog(l);

                using (var ms = new MemoryStream(log.Contents))
                using (var sr = new StreamReader(ms))
                    if (!logOptions.RawPrint)
                        PrintFiltered(LogEvent.Load(sr), logOptions);
                    else
                        PrintRaw(sr, logOptions);
            }
        }


        /// <summary>
        /// Print raw
        /// </summary>
        private static void PrintRaw(StreamReader sr, DisplayOptions logOptions)
        {
            var nLines = Int32.Parse(logOptions.NumLines ?? "25");
            var buffer = new Queue<String>(nLines);
            var searchRegex = new Regex(logOptions.Grep ?? ".*");

            var ln = 0;
            var line = sr;
            while(!sr.EndOfStream)
            {

                var lineData = sr.ReadLine();
                // Grep?
                ln++;

                // Search?
                if (searchRegex.IsMatch(lineData))
                {
                    // Add
                    buffer.Enqueue(lineData);
                    if (buffer.Count > nLines && !logOptions.All)
                        buffer.Dequeue();

                    // Head?
                    if (logOptions.Head && buffer.Count >= nLines)
                        break;
                }
            }

            // Output the content of the lines
            var ol = 0;
            while(buffer.Count > 0)
            {
                Console.WriteLine(buffer.Dequeue());
                if(ol++ > Console.WindowHeight && logOptions.Page)
                {
                    ol = 0;
                    Console.WriteLine("Press [Enter]...");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Print filtered object
        /// </summary>
        private static void PrintFiltered(List<LogEvent> list, DisplayOptions logOptions)
        {
            var nLines = Int32.Parse(logOptions.NumLines ?? "25");
            var searchRegex = new Regex(logOptions.Grep ?? ".*");
            var typeFilter = (EventLevel)Enum.Parse(typeof(EventLevel), logOptions.Filter ?? "Verbose");
            var filteredEvents = list.Where(o =>  searchRegex.IsMatch(o.Message) && o.Level <= typeFilter).ToList();
            int ofs = logOptions.Head || logOptions.All ? 0 : filteredEvents.Count - nLines;
            if (ofs < 0) ofs = 0;

            if (logOptions.Head && !logOptions.All)
                filteredEvents = filteredEvents.Take(nLines).ToList();

            // Output the content of the lines
            for(int i = ofs; i < filteredEvents.Count; i++)
            {
                Console.WriteLine("{0:o}\t{1}\tTHD#:{2}\t{3}", filteredEvents[i].Date, filteredEvents[i].Level, filteredEvents[i].Thread, filteredEvents[i].Message);
                if(logOptions.Page && i % Console.WindowHeight == 0)
                {
                    Console.WriteLine("Press [Enter]...");
                    Console.ReadKey();
                }
            }
        }
    }
}
