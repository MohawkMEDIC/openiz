/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-6-14
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{

    /// <summary>
    /// Application identity
    /// </summary>
    public class ApplicationIdentity : ClaimsIdentity
    {
        // Member variables
        private string m_name;
        private bool m_isAuthenticated;

        /// <summary>
        /// Application identity ctor
        /// </summary>
        public ApplicationIdentity(Guid sid, String name, Boolean isAuthenticated)
        {
            this.m_name = name.ToString();
            this.m_isAuthenticated = isAuthenticated;
            this.AddClaim(new Claim(ClaimTypes.Sid, sid.ToString()));
        }

        /// <summary>
        /// Identity for an application
        /// </summary>
        public override string AuthenticationType
        {
            get
            {
                return "SYSTEM";
            }
        }

        /// <summary>
        /// True if is authenticated
        /// </summary>
        public override bool IsAuthenticated { get { return this.m_isAuthenticated; }  }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public override string Name { get { return this.m_name; } }
    }
    /// <summary>
    /// Represents an IPrincipal related to an application
    /// </summary>
    public class ApplicationPrincipal : ClaimsPrincipal
    {

        /// <summary>
        /// Application principal
        /// </summary>
        public ApplicationPrincipal(IIdentity identity) : base(identity)
        {
        }

    }
}
