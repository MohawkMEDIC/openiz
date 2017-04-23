using MohawkCollege.Util.Console.Parameters;
using OpenIZ.BusinessRules.JavaScript;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OizDevTool.BreDebugger
{
    /// <summary>
    /// Represents a business rules debugger
    /// </summary>
    [Description("Tools for debugging dynamic parts of OpenIZ IMS")]
    public static class Debug
    {

        /// <summary>
        /// BRE debugger tools
        /// </summary>
        public class BreDebugParameters
        {

            /// <summary>
            /// Gets or sets the source of the debugger
            /// </summary>
            [Parameter("s")]
            [Parameter("source")]
            [Description("Identifies the source of the debugger")]
            public StringCollection Sources { get; set; }

            /// <summary>
            /// Gets the working directory of the debugger
            /// </summary>
            [Description("Sets the working directory of the debugger")]
            [Parameter("workDir")]
            public String WorkingDirectory { get; set; }
        }


        /// <summary>
        /// Business rule
        /// </summary>
        [Description("Initiates the debugging of a business rule using the JINT engine")]
        [ParameterClass(typeof(BreDebugParameters))]
        [Interactive]
        [Example("Debug file samplerule.js using specified openiz.js implementation", "--source=samplerule.js --source=openiz.js")]
        public static void BusinessRule(String[] args)
        {
            BreDebugParameters parms = new ParameterParser<BreDebugParameters>().Parse(args);

            BreDebugger debugger = new BreDebugger(parms.Sources, parms.WorkingDirectory);
            debugger.Debug();
        }


    }
}
