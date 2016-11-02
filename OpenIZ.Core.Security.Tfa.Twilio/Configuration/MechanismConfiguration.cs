using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security.Tfa.Twilio.Configuration
{
    /// <summary>
    /// Represents the configuration for the TFA mecahnism
    /// </summary>
    public class MechanismConfiguration
    {
        /// <summary>
        /// Creates a new template mechanism configuration
        /// </summary>
        public MechanismConfiguration()
        {
        }

        /// <summary>
        /// SID configuration
        /// </summary>
        public String Sid { get; set; }

        /// <summary>
        /// From number
        /// </summary>
        public String From { get; set; }

        /// <summary>
        /// Authentication token
        /// </summary>
        public String Auth { get; set; }
    }
}
