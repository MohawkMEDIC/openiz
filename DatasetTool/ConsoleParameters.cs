using MohawkCollege.Util.Console.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OizDevTool
{
    /// <summary>
    /// Console parameters
    /// </summary>
    public class ConsoleParameters
    {

        /// <summary>
        /// The The name of the tool
        /// </summary>
        [Parameter("tool")]
        [Parameter("t")]
        public String ToolName { get; set; }

        /// <summary>
        /// Operation name
        /// </summary>
        [Parameter("operation")]
        [Parameter("o")]
        public String OperationName { get; set; }

    }
}
