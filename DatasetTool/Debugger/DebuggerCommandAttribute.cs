using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OizDevTool.Debugger
{
    /// <summary>
    /// Represents a debugger command 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DebuggerCommandAttribute : Attribute
    {

        /// <summary>
        /// Ctore for debugger command
        /// </summary>
        public DebuggerCommandAttribute(String cmd, String desc)
        {
            this.Command = cmd;
            this.Description = desc;
        }

        /// <summary>
        /// The command to be run
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// The command to be run
        /// </summary>
        public string Description { get; set; }

    }
}
