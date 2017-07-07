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
 * Date: 2017-4-22
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Diagnostics
{
    /// <summary>
    /// Diagnostic extensions class
    /// </summary>
    public static class DiagnosticExtensions
    {

        /// <summary>
        /// Trace error
        /// </summary>
        public static void TraceError(this TraceSource me, String format, params object[] args) => me.TraceEvent(TraceEventType.Error, 0, format, args);

        /// <summary>
        /// Trace error
        /// </summary>
        public static void TraceWarning(this TraceSource me, String format, params object[] args) => me.TraceEvent(TraceEventType.Warning, 0, format, args);

        /// <summary>
        /// Trace error
        /// </summary>
        public static void TraceInfo(this TraceSource me, String format, params object[] args) => me.TraceEvent(TraceEventType.Information, 0, format, args);
        
        /// <summary>
        /// Trace error
        /// </summary>
        public static void TraceVerbose(this TraceSource me, String format, params object[] args) => me.TraceEvent(TraceEventType.Verbose, 0, format, args);

    }
}
