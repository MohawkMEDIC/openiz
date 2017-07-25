using OpenIZ.Core.Diagnostics;
using System;
using System.Diagnostics.Tracing;

namespace OpenIZ.AdminConsole.Shell
{
    /// <summary>
    /// Represents the console trace writer
    /// </summary>
    internal class ConsoleTraceWriter : TraceWriter
    {
        /// <summary>
        /// Console trace writer
        /// </summary>
        public ConsoleTraceWriter(string filter) : base((EventLevel)Enum.Parse(typeof(EventLevel), filter), null)
        {
        }

        /// <summary>
        /// Write trace to the console
        /// </summary>
        protected override void WriteTrace(EventLevel level, string source, string format, params object[] args)
        {
            switch (level)
            {
                case EventLevel.Critical:
                case EventLevel.Error:
                    Console.ForegroundColor = Console.BackgroundColor == ConsoleColor.Black ? ConsoleColor.Red : ConsoleColor.DarkRed;
                    Console.WriteLine($"E:{format}", args);
                    Console.ResetColor();
                    break;
                case EventLevel.Informational:
                    Console.ForegroundColor = Console.BackgroundColor == ConsoleColor.Black ? ConsoleColor.Cyan : ConsoleColor.DarkCyan;
                    Console.WriteLine($"I:{format}", args);
                    Console.ResetColor();
                    break;
                case EventLevel.Warning:
                    Console.ForegroundColor = Console.BackgroundColor == ConsoleColor.Black ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;
                    Console.WriteLine($"W:{format}", args);
                    Console.ResetColor();
                    break;
                case EventLevel.Verbose:
                    Console.ForegroundColor = Console.BackgroundColor == ConsoleColor.Black ? ConsoleColor.Magenta : ConsoleColor.DarkMagenta;
                    Console.WriteLine($"V:{format}", args);
                    Console.ResetColor();
                    break;

            }
        }
    }
}