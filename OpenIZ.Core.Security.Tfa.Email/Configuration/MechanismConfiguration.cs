using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security.Tfa.Email.Configuration
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
            this.Templates = new List<TemplateConfiguration>();
        }

        /// <summary>
        /// SMTP configuration
        /// </summary>
        public SmtpConfiguration Smtp { get; set; }

        /// <summary>
        /// Template configuration
        /// </summary>
        public List<TemplateConfiguration> Templates { get; set; }
    }
}
