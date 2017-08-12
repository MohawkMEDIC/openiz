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
 * User: khannan
 * Date: 2017-7-25
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using MARC.HI.EHRS.SVC.Core.Event;
using System.Security.Claims;
using OpenIZ.Core.Security.Claims;

namespace OpenIZ.AdminConsole.Security
{
    /// <summary>
    /// Represents an HTTP BASIC identity provider
    /// </summary>
    public class HttpBasicIdentityProvider : IIdentityProviderService
    {
        public event EventHandler<AuthenticatedEventArgs> Authenticated;
        public event EventHandler<AuthenticatingEventArgs> Authenticating;

        public void AddClaim(string userName, Claim claim)
        {
            throw new NotImplementedException();
        }

        public IPrincipal Authenticate(string userName, string password)
        {
            return this.Authenticate(userName, password);
        }

        public IPrincipal Authenticate(string userName, string password, string tfaSecret)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new GenericIdentity(userName), new Claim[] { new Claim("passwd", password), new Claim(OpenIzClaimTypes.OpenIZTfaSecretClaim, tfaSecret) }));
        }

        public void ChangePassword(string userName, string newPassword, IPrincipal authContext)
        {
            throw new NotImplementedException();
        }

        public IIdentity CreateIdentity(string userName, string password, IPrincipal authContext)
        {
            throw new NotImplementedException();
        }

        public void DeleteIdentity(string userName, IPrincipal authContext)
        {
            throw new NotImplementedException();
        }

        public string GenerateTfaSecret(string userName)
        {
            throw new NotImplementedException();
        }

        public IIdentity GetIdentity(string userName)
        {
            throw new NotImplementedException();
        }

        public void RemoveClaim(string userName, string claimType)
        {
            throw new NotImplementedException();
        }

        public void SetLockout(string userName, bool lockout, IPrincipal authContext)
        {
            throw new NotImplementedException();
        }
    }
}
