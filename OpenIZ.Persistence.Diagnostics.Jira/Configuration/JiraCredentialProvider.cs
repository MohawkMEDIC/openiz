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