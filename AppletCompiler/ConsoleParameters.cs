using MohawkCollege.Util.Console.Parameters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppletCompiler
{
    /// <summary>
    /// Console parameters
    /// </summary>
    public class ConsoleParameters
    {

        /// <summary>
        /// Deploy
        /// </summary>
        [Parameter("deploy")]
        public string Deploy { get; set; }
        
        /// <summary>
        /// Language
        /// </summary>
        [Parameter("lang")]
        public string Lang { get; set; }

        /// <summary>
        /// Clean
        /// </summary>
        [Parameter("c")]
        public bool Clean { get; set; }

        /// <summary>
        /// Source files
        /// </summary>
        [Parameter("s")]
        [Parameter("source")]
        [Description("Identifies the source files to include in the applet")]
        public String Source { get; set; }

        /// <summary>
        /// Gets or sets the output
        /// </summary>
        [Description("The output applet file")]
        [Parameter("o")]
        [Parameter("output")]
        public String Output { get; set; }


        /// <summary>
        /// Gets or sets the indicator for showing help
        /// </summary>
        [Parameter("?")]
        [Parameter("help")]
        public bool Help { get; set; }
    }
}
