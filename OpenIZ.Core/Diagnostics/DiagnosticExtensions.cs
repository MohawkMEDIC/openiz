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

    }
}
