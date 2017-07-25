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
 * Date: 2017-6-28
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Http;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.AdminConsole.Security
{
    /// <summary>
    /// Represents a basic token crendtial provider
    /// </summary>
    public class HttpBasicTokenCredentialProvider : ICredentialProvider
    {
        #region ICredentialProvider implementation
        /// <summary>
        /// Gets or sets the credentials which are used to authenticate
        /// </summary>
        /// <returns>The credentials.</returns>
        /// <param name="context">Context.</param>
        public Credentials GetCredentials(IRestClient context)
        {
            return this.GetCredentials(AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Authenticate a user - this occurs when reauth is required
        /// </summary>
        /// <param name="context">Context.</param>
        public Credentials Authenticate(IRestClient context)
        {
            if (OpenIZ.AdminConsole.Shell.ApplicationContext.Current.Authenticate(new HttpBasicIdentityProvider(), context))
                return this.GetCredentials(AuthenticationContext.Current.Principal);
            return null;
        }

        /// <summary>
        /// Get credentials from the specified principal
        /// </summary>
        public Credentials GetCredentials(IPrincipal principal)
        {
            if (principal is ClaimsPrincipal)
                return new HttpBasicCredentials(principal, (principal as ClaimsPrincipal)?.FindFirst(o=>o.Type == "passwd")?.Value);
            else
                throw new InvalidOperationException("Cannot create basic principal from non-claims principal");
        }
        #endregion
    }
}
