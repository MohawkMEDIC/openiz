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
