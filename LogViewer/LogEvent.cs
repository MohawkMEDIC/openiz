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
 * Date: 2017-3-31
 */
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogViewer
{
    /// <summary>
    /// Represents a log event
    /// </summary>
    public class LogEvent
    {
        /// <summary>
        /// Gets or sets the log source
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// Get or sets the level
        /// </summary>
        public EventLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the thread
        /// </summary>
        public String Thread { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// Log sequence
        /// </summary>
        public int Sequence { get; internal set; }

        /// <summary>
        /// Load gzipped stream
        /// </summary>
        public static List<LogEvent> LoadGz(String filename)
        {
            using (var strm = new GZipStream(File.OpenRead(filename), CompressionMode.Decompress))
            using (var sw = new StreamReader(strm))
                return LogEvent.Load(sw);
        }

        /// <summary>
        /// Load plain text
        /// </summary>
        public static List<LogEvent> Load(String filename)
        {
            using (var sw = File.OpenText(filename))
                return LogEvent.Load(sw);
        }

        /// <summary>
        /// Load the specified file
        /// </summary>
        public static List<LogEvent> Load(StreamReader stream)
        {
            Regex v1Regex = new Regex(@"^(.*)?(\s)?[\s][\[\<](.*?)[\]\>]\s\[(.*?)\]\s?:(.*)$"),
             v2Regex = new Regex(@"^(.*)?@(.*)?\s[\[\<](.*)?[\>\]]\s\[(.*?)\]\:\s(.*)$"),
             serverOld = new Regex(@"^([0-9\-\s\:APM\/]*?)?\s:\s(.*)\s(Information|Warning|Error|Fatal|Verbose):\s-?\d{1,10}?\s:(.*)$"),
             server = new Regex(@"^([0-9\-\s\:APM\/]*?)\[@(\d*)\]?\s:\s(.*)\s(Information|Warning|Error|Fatal|Verbose):\s-?\d{1,10}?\s:(.*)$"),
             logCat = new Regex(@"^(\d{2}\-\d{2}\s\d{2}\:\d{2}\:\d{2}\.\d{3})\s*(\d*)?\s*(\d*)?\s([IVDEW])\s([\w\-\s]*):\s(.*)$");

            List<LogEvent> retVal = new List<LogEvent>();
            LogEvent current = null;

            while (!stream.EndOfStream)
            { 
                var line = stream.ReadLine();
                var match = v2Regex.Match(line);
                if (!match.Success)
                    match = v1Regex.Match(line);
                if (match.Success)
                {
                    if (current != null) retVal.Add(current);
                    current = new LogEvent()
                    {
                        Sequence = current?.Sequence + 1 ?? 0,
                        Source = match.Groups[1].Value,
                        Thread = match.Groups[2].Value,
                        Level = (EventLevel)Enum.Parse(typeof(EventLevel), match.Groups[3].Value),
                        Date = DateTime.Parse(match.Groups[4].Value),
                        Message = match.Groups[5].Value
                    };
                }
                else if(server.IsMatch(line))
                {
                    if (current != null) retVal.Add(current);
                    match = server.Match(line);
                    current = new LogEvent()
                    {
                        Sequence = current?.Sequence + 1 ?? 0,
                        Source = match.Groups[3].Value,
                        Level = match.Groups[4].Value == "Information" ? EventLevel.Informational :
                        match.Groups[4].Value == "Warning" ? EventLevel.Warning :
                        match.Groups[4].Value == "Error" ? EventLevel.Error : EventLevel.Verbose,
                        Date = DateTime.Parse(match.Groups[1].Value),
                        Message = match.Groups[5].Value,
                        Thread = match.Groups[2].Value
                    };
                }
                else if (serverOld.IsMatch(line))
                {
                    if (current != null) retVal.Add(current);
                    match = serverOld.Match(line);
                    current = new LogEvent()
                    {
                        Sequence = current?.Sequence + 1 ?? 0,
                        Source = match.Groups[2].Value,
                        Level = match.Groups[3].Value == "Information" ? EventLevel.Informational :
                        match.Groups[3].Value == "Warning" ? EventLevel.Warning :
                        match.Groups[3].Value == "Error" ? EventLevel.Error : EventLevel.Verbose,
                        Date = DateTime.Parse(match.Groups[1].Value),
                        Message = match.Groups[4].Value,
                        Thread = "NA"
                    };
                }
                else if(logCat.IsMatch(line))
                {
                    if (current != null) retVal.Add(current);
                    match = logCat.Match(line);
                    current = new LogEvent()
                    {
                        Sequence = current?.Sequence + 1 ?? 0,
                        Source = match.Groups[5].Value,
                        Level = match.Groups[4].Value == "I" ? EventLevel.Informational :
                        match.Groups[4].Value == "V" ? EventLevel.Verbose :
                        match.Groups[4].Value == "E" ? EventLevel.Error :
                        match.Groups[4].Value == "D" ? EventLevel.LogAlways : EventLevel.Warning,
                        Date = DateTime.ParseExact(match.Groups[1].Value, "MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
                        Message = match.Groups[6].Value,
                        Thread = match.Groups[3].Value
                    };
                }
                else if(current != null)
                    current.Message += "\r\n" + line;
            }
            if(current != null)
                retVal.Add(current);

            return retVal;
        }
    }
}
