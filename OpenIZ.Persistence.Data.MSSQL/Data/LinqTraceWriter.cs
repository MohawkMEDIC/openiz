using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Data
{
    /// <summary>
    /// Trace writer for LINQ query
    /// </summary>
    internal class LinqTraceWriter : TextWriter
    {
        /// <summary>
        /// Write to trace
        /// </summary>
        public override void Write(char[] buffer, int index, int count)
        {
            Trace.Write(new String(buffer, index, count));
        }

        /// <summary>
        /// Writer string value to trace
        /// </summary>
        public override void Write(string value)
        {
            Trace.Write(value);
        }

        /// <summary>
        /// Get the encoding
        /// </summary>
        public override Encoding Encoding
        {
            get { return System.Text.Encoding.Default; }
        }

    }
}
