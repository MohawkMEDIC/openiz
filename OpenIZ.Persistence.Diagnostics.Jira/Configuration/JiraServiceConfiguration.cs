/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-11-30
 */
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
	    /// Gets whether a tracing is enabled.
	    /// </summary>
	    public bool Trace { get; }

	    /// <summary>
        /// Gets or sets the endpoint information
        /// </summary>
        public List<IRestClientEndpointDescription> Endpoint { get; private set; }
    }
}
