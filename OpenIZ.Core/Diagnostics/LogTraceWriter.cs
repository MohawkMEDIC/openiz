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
using System.Diagnostics.Tracing;
using System.Diagnostics;
using OpenIZ.Core.Diagnostics;
using System.Collections.Generic;

namespace OpenIZ.Core.Diagnostics
{
    /// <summary>
    /// Represents a trace writer which writes to the android log
    /// </summary>
    public class LogTraceWriter : TraceWriter
    {
        private Dictionary<String, TraceSource> m_traceSources = new Dictionary<string, TraceSource>();

        /// <summary>
        /// Initialize the trace writer
        /// </summary>
        public LogTraceWriter(EventLevel filter, String initializationData) : base(filter, null)
        {
        }


        #region implemented abstract members of TraceWriter
        /// <summary>
        /// Write trace
        /// </summary>
        protected override void WriteTrace(EventLevel level, string source, string format, params object[] args)
        {
            TraceSource tsource = null;
            if (!this.m_traceSources.TryGetValue(source, out tsource))
                lock (this.m_traceSources)
                    if (!this.m_traceSources.ContainsKey(source))
                    {
                        tsource = new TraceSource(source);
                        this.m_traceSources.Add(source, tsource);
                    }

            switch (level)
            {
                case EventLevel.Error:
                    tsource?.TraceEvent(TraceEventType.Error, 0, format, args);
                    break;
                case EventLevel.Informational:
	                tsource?.TraceEvent(TraceEventType.Information, 0, format, args);
                    break;
                case EventLevel.Critical:
	                tsource?.TraceEvent(TraceEventType.Critical, 0, format, args);
                    break;
                case EventLevel.Verbose:
	                tsource?.TraceEvent(TraceEventType.Verbose, 0, format, args);
                    break;
                case EventLevel.Warning:
	                tsource?.TraceEvent(TraceEventType.Warning, 0, format, args);
                    break;

            }
        }
        #endregion
    }
}

