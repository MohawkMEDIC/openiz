using MohawkCollege.Util.Console.Parameters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool
{
    /// <summary>
    /// Console parameters
    /// </summary>
    public class ConsoleParameters
    {

        /// <summary>
        /// List deployment options
        /// </summary>
        [Parameter("l")]
        [Parameter("list")]
        public bool ListDeploy { get; set; }

        /// <summary>
        /// Deployment scripts to run
        /// </summary>
        [Parameter("d")]
        [Parameter("deploy")]
        public StringCollection Deploy { get; set; }


        /// <summary>
        /// Deployment options
        /// </summary>
        [ParameterExtension()]
        public Dictionary<String, StringCollection> Options { get; set; }
    }
}
