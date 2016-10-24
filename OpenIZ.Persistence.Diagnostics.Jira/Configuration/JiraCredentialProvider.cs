using System;
using System.Security.Principal;
using OpenIZ.Core.Http;
using System.Configuration;
using System.Collections.Generic;
using OpenIZ.Persistence.Diagnostics.Jira.Model;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;

namespace OpenIZ.Persistence.Diagnostics.Jira.Configuration
{
   
    /// <summary>
    /// Represents a JIRA session credential
    /// </summary>
    internal class JiraCredentials : Credentials
    {

        // JIRA service configuration
        private JiraServiceConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.persistence.diagnostics.jira") as JiraServiceConfiguration;

        // Authentication
        private JiraAuthenticationResponse m_authentication;

        /// <summary>
        /// Create JIRA credentials
        /// </summary>
        public JiraCredentials(JiraAuthenticationResponse response) : base(null)
        {
            this.m_authentication = response;
        }

        /// <summary>
        /// Get the HTTP headers
        /// </summary>
        public override Dictionary<string, string> GetHttpHeaders()
        {
            return new Dictionary<string, string>()
            {
                { "Cookie", String.Format("{0}={1}", this.m_authentication.Session.Name, this.m_authentication.Session.Value) }
            };
        }
    }
}