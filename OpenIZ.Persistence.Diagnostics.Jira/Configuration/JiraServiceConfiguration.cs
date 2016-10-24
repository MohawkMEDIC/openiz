using OpenIZ.Core.Http;
using OpenIZ.Core.Http.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Diagnostics.Jira.Configuration
{
    /// <summary>
    /// JIRA Service configuration
    /// </summary>
    public class JiraServiceConfiguration : IRestClientDescription
    {
        /// <summary>
        /// Creates a new jira service configuration
        /// </summary>
        public JiraServiceConfiguration(String jiraUri, String projectKey, String userName, String password)
        {
            this.Endpoint = new List<IRestClientEndpointDescription>()
            {
                new ServiceClientEndpointDescription(jiraUri)
            };
            this.Binding = new JiraRestClientBindingDescription();
            this.Project = projectKey;
            this.UserName = userName;
            this.Password = password;
        }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// Gets or sets the project
        /// </summary>
        public String Project { get; set; }

        /// <summary>
        /// Gets the binding of the client description
        /// </summary>
        public IRestClientBindingDescription Binding { get; set; }

        /// <summary>
        /// Gets or sets the endpoint information
        /// </summary>
        public List<IRestClientEndpointDescription> Endpoint { get; private set; }
    }
}
