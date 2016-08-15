using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Alerting
{
    /// <summary>
    /// Alert event arguments
    /// </summary>
    public class AlertEventArgs : EventArgs
    {

        /// <summary>
        /// Construct a new alert event 
        /// </summary>
        /// <param name="msg"></param>
        public AlertEventArgs(AlertMessage msg)
        {
            this.Message = msg;
        }

        /// <summary>
        /// Allows the handler to instruct the alert engine to ignore (not to persist) the 
        /// alert
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Gets the alert message
        /// </summary>
        public AlertMessage Message { get; internal set; }
    }
}
