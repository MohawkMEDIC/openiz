using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Diagnostics.Email.Configuration
{
    /// <summary>
    /// Represents diagnostic email configuration
    /// </summary>
    public class DiagnosticEmailServiceConfiguration
    {

        /// <summary>
        /// Gets or sets the SMTP configuration
        /// </summary>
        public SmtpConfiguration Smtp { get; set; }

        /// <summary>
        /// Gets or sets the recipient
        /// </summary>
        public List<String> Recipients { get; set; }

    }
}
