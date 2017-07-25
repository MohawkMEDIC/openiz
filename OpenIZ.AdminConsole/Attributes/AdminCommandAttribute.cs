using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.AdminConsole.Attributes
{
    /// <summary>
    /// Identifies the method can be invoked from the admin console
    /// </summary>
    public class AdminCommandAttribute : System.Attribute
    {



        /// <summary>
        /// Administrative command attribute
        /// </summary>
        public AdminCommandAttribute(String command, String description) 
        {
            this.Description = description;
            this.Command = command;
        }

        /// <summary>
        /// Commands
        /// </summary>
        public String Command { get; set; }

        /// <summary>
        /// Description for commands
        /// </summary>
        public String Description { get; set; }


    }
}
