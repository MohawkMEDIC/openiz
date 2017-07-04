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
 * Date: 2016-8-2
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Http
{
    /// <summary>
    /// Represents a credential provider which does basic http
    /// </summary>
    public class HttpBasicCredentials : Credentials
    {
        // Password
        private String m_password;
        private string m_userName;

        /// <summary>
        /// Creates the basic credential 
        /// </summary>
        public HttpBasicCredentials(String userName, string password) : base(null)
        {
            this.m_userName = userName;
            this.m_password = password;

        }
        
        /// <summary>
        /// Creates the basic credential 
        /// </summary>
        public HttpBasicCredentials(IPrincipal principal, string password) : base(principal)
        {
            if (!principal.Identity.IsAuthenticated)
                throw new InvalidOperationException("Principal must be authenticated");
            this.m_userName = principal.Identity.Name;
            this.m_password = password;

        }

        /// <summary>
        /// Gets the HTTP headers
        /// </summary>
        public override Dictionary<string, string> GetHttpHeaders()
        {
            var authString = String.Format("{0}:{1}", this.m_userName, this.m_password);
            return new Dictionary<string, string>()
            {
                {  "Authorization", String.Format("BASIC {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(authString))) }
            };
        }
    }
}
