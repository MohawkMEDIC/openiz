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
 * Date: 2016-11-30
 */
using OpenIZ.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.BusinessRules.JavaScript.JNI
{
    /// <summary>
    /// Supplies console
    /// </summary>
    public class JsConsoleProvider 
    {
        // Tracker
        private Tracer m_tracer = Tracer.GetTracer(typeof(JsConsoleProvider));

        /// <summary>
        /// Log informational
        /// </summary>
        public void info(string log)
        {
            this.m_tracer.TraceInfo("JS> {0}", log);
        }

        /// <summary>
        /// Log informational
        /// </summary>
        public void warn(string log)
        {
            this.m_tracer.TraceWarning("JS> {0}", log);
        }

        /// <summary>
        /// Log informational
        /// </summary>
        public void error(string log)
        {
            this.m_tracer.TraceError("JS> {0}", log);
        }

        /// <summary>
        /// Assert 
        /// </summary>
        public void assert(bool comparison, string message)
        {
            if(!comparison)
                throw new ArgumentOutOfRangeException(nameof(comparison), message);
        }
    }
}
