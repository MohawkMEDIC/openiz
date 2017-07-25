using MohawkCollege.Util.Console.Parameters;
using OpenIZ.AdminConsole.Parameters;
using OpenIZ.AdminConsole.Shell;
using OpenIZ.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.AdminConsole
{
    class Program
    {
        /// <summary>
        /// Main program entry point
        /// </summary>
        static void Main(string[] args)
        {

            Console.WriteLine("Open Immunize Adminisration & Security Console v{0}", typeof(Program).Assembly.GetName().Version);
            Console.WriteLine("Copyright (C) 2015 - 2017, Mohawk College of Applied Arts and Technology");

            var pp = new ParameterParser<ConsoleParameters>();
            var options = pp.Parse(args);

            if (options.Help)
                pp.WriteHelp(Console.Out);
            else
                try
                {
                    // Add a console trace output
                    if (!String.IsNullOrEmpty(options.Verbosity))
                        Tracer.AddWriter(new ConsoleTraceWriter(options.Verbosity), (EventLevel)Enum.Parse(typeof(EventLevel), options.Verbosity));
                    else
                        Tracer.AddWriter(new ConsoleTraceWriter("Error"), EventLevel.Error);

                    ApplicationContext.Initialize(options);
                    if (ApplicationContext.Current.Start())
                        new InteractiveShell().Exec();
                }
                catch (Exception e)
                {
                    Console.WriteLine("FATAL: {0}", e);

                }
                finally
                {
                    Console.ResetColor();
                }
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
