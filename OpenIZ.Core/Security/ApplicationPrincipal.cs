/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-2-17
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{

    /// <summary>
    /// Application identity
    /// </summary>
    public class ApplicationIdentity : IIdentity
    {

        /// <summary>
        /// Application identity ctor
        /// </summary>
        public ApplicationIdentity(Guid name, Boolean isAuthenticated)
        {
            this.Name = name.ToString();
            this.IsAuthenticated = isAuthenticated;
        }

        /// <summary>
        /// Identity for an application
        /// </summary>
        public string AuthenticationType
        {
            get
            {
                return "SYSTEM";
            }
        }

        /// <summary>
        /// True if is authenticated
        /// </summary>
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; private set; }
    }
    /// <summary>
    /// Represents an IPrincipal related to an application
    /// </summary>
    public class ApplicationPrincipal : IPrincipal
    {

        /// <summary>
        /// Application principal
        /// </summary>
        public ApplicationPrincipal(IIdentity identity)
        {
            this.Identity = identity;
        }

        /// <summary>
        /// Gets the identity
        /// </summary>
        public IIdentity Identity { get; private set; }

        /// <summary>
        /// Is in role?
        /// </summary>
        public bool IsInRole(string role)
        {
            return false;
        }
    }
}
