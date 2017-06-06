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

namespace OizDevTool.Debugger
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
        public class DebugParameters
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
        [ParameterClass(typeof(DebugParameters))]
        [Interactive]
        [Example("Debug file samplerule.js", "--source=samplerule.js")]
        public static void BusinessRule(String[] args)
        {
            DebugParameters parms = new ParameterParser<DebugParameters>().Parse(args);

            BreDebugger debugger = new BreDebugger(parms.Sources, parms.WorkingDirectory);
            debugger.Debug();
        }

        /// <summary>
        /// Business rule
        /// </summary>
        [Description("Initiates the debugging of a clinical protocol")]
        [ParameterClass(typeof(DebugParameters))]
        [Interactive]
        [Example("Debug file sampleprotocol.xml", "--source=sampleprotocol.xml")]
        public static void CarePlan(String[] args)
        {
            DebugParameters parms = new ParameterParser<DebugParameters>().Parse(args);

            ProtoDebugger debugger = new ProtoDebugger(parms.Sources, parms.WorkingDirectory);
            debugger.Debug();
        }
    }
}
