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
 * Date: 2017-3-31
 */
using System;
using System.Linq;
using System.Security.Principal;
using System.Security;
using OpenIZ.Core.Http;
using System.Net;
using OpenIZ.Core.Services;
using System.Text;
using OpenIZ.Core.Interfaces;
using OpenIZ.Core.Diagnostics;
using MARC.HI.EHRS.SVC.Core.Event;
using System.Security.Claims;
using OpenIZ.Core.Security.Claims;
using OpenIZ.AdminConsole.Shell;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using OpenIZ.Core.Security;

namespace OpenIZ.AdminConsole.Security
{
    /// <summary>
    /// Represents an OAuthIdentity provider
    /// </summary>
    public class OAuthIdentityProvider : IIdentityProviderService, ISecurityAuditEventSource
    {
        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(OAuthIdentityProvider));

        #region IIdentityProviderService implementation
        /// <summary>
        /// Occurs when authenticating.
        /// </summary>
        public event EventHandler<AuthenticatingEventArgs> Authenticating;
        /// <summary>
        /// Occurs when authenticated.
        /// </summary>
        public event EventHandler<AuthenticatedEventArgs> Authenticated;
        public event EventHandler<SecurityAuditDataEventArgs> SecurityAttributesChanged;
        public event EventHandler<AuditDataEventArgs> DataCreated;
        public event EventHandler<AuditDataEventArgs> DataUpdated;
        public event EventHandler<AuditDataEventArgs> DataObsoleted;
        public event EventHandler<AuditDataDisclosureEventArgs> DataDisclosed;
        public event EventHandler<SecurityAuditDataEventArgs> SecurityResourceCreated;
        public event EventHandler<SecurityAuditDataEventArgs> SecurityResourceDeleted;

        /// <summary>
        /// Authenticate the user
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        public System.Security.Principal.IPrincipal Authenticate(string userName, string password)
        {
            if(String.IsNullOrEmpty(userName))
                return this.Authenticate(AuthenticationContext.Current.Principal, password);
            else
                return this.Authenticate(new GenericPrincipal(new GenericIdentity(userName), null), password);
        }

        /// <summary>
        /// Perform authentication with specified password
        /// </summary>
        public System.Security.Principal.IPrincipal Authenticate(System.Security.Principal.IPrincipal principal, string password)
        {
            return this.Authenticate(principal, password, null);
        }

        /// <summary>
        /// Authenticate the user
        /// </summary>
        /// <param name="principal">Principal.</param>
        /// <param name="password">Password.</param>
        public System.Security.Principal.IPrincipal Authenticate(System.Security.Principal.IPrincipal principal, string password, String tfaSecret)
        {

            AuthenticatingEventArgs e = new AuthenticatingEventArgs(principal.Identity.Name);
            this.Authenticating?.Invoke(this, e);
            if (e.Cancel)
            {
                this.m_tracer.TraceWarning("Pre-Event ordered cancel of auth {0}", principal);
                return null;
            }

            // Get the scope being requested
            String scope = "*";
            if (principal is ClaimsPrincipal)
                scope = (principal as ClaimsPrincipal).Claims.FirstOrDefault(o => o.Type == OpenIzClaimTypes.OpenIzScopeClaim)?.Value ?? scope;
            else
                scope = ApplicationContext.Current.GetRestClient(Core.Interop.ServiceEndpointType.ImmunizationIntegrationService)?.Description.Endpoint[0].Address ??
                    ApplicationContext.Current.GetRestClient(Core.Interop.ServiceEndpointType.AdministrationIntegrationService)?.Description.Endpoint[0].Address ??
                    "*";

            // Authenticate
            IPrincipal retVal = null;

            try
            {
                using (IRestClient restClient = ApplicationContext.Current.GetRestClient(Core.Interop.ServiceEndpointType.AuthenticationService))
                {

                    // Set credentials
                    restClient.Credentials = new OAuthTokenServiceCredentials(principal);

                    // Create grant information
                    OAuthTokenRequest request = null;
                    if (!String.IsNullOrEmpty(password))
                        request = new OAuthTokenRequest(principal.Identity.Name, password, scope);
                    else if (principal is TokenClaimsPrincipal)
                        request = new OAuthTokenRequest(principal as TokenClaimsPrincipal, scope);
                    else
                        request = new OAuthTokenRequest(principal.Identity.Name, null, scope);

                    try
                    {
                        restClient.Requesting += (o, p) =>
                        {
                            p.AdditionalHeaders.Add("X-OpenIZClient-Claim", Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}={1}", OpenIzClaimTypes.OpenIzScopeClaim, scope))));
                            if (!String.IsNullOrEmpty(tfaSecret))
                                p.AdditionalHeaders.Add("X-OpenIZ-TfaSecret", tfaSecret);
                        };

                        OAuthTokenResponse response = restClient.Post<OAuthTokenRequest, OAuthTokenResponse>("oauth2_token", "application/x-www-urlform-encoded", request);
                        retVal = new TokenClaimsPrincipal(response.AccessToken, response.TokenType, response.RefreshToken);
                        this.Authenticated?.Invoke(this, new AuthenticatedEventArgs(principal.Identity.Name, retVal, true));

                    }
                    catch(RestClientException<OAuthTokenResponse> ex)
                    {
                        this.m_tracer.TraceWarning("OAUTH Server Responded: {0}", ex.Result.ErrorDescription);
                    }
                    catch (WebException ex) // Raw level web exception
                    {

                        this.m_tracer.TraceError("Error authenticating: {0}", ex.Message);

                    }
                    catch (SecurityException ex)
                    {
                        this.m_tracer.TraceError("Server was contacted however the token is invalid: {0}", ex.Message);
                        throw;
                    }
                    catch (Exception ex) // fallback to local
                    {
                        this.m_tracer.TraceError("General Authentication Error: {0}", ex.Message);
                    }
                }
            }
            catch
            {
                this.Authenticated?.Invoke(this, new AuthenticatedEventArgs(principal.Identity.Name, retVal, false));
                throw;
            }

            return retVal;
        }

  

        /// <summary>
        /// Gets the specified identity
        /// </summary>
        public System.Security.Principal.IIdentity GetIdentity(string userName)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Authenticates the specified user
        /// </summary>
		public System.Security.Principal.IPrincipal Authenticate(string userName, string password, string tfaSecret)
        {
            return this.Authenticate(new GenericPrincipal(new GenericIdentity(userName), null), password, tfaSecret);
        }

        /// <summary>
        /// Changes the users password.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="newPassword">The new password of the user.</param>
        /// <param name="principal">The authentication principal (the user that is changing the password).</param>
        public void ChangePassword(string userName, string newPassword, System.Security.Principal.IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Changes the users password.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="password">The new password of the user.</param>
        public void ChangePassword(string userName, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an identity
        /// </summary>
        public IIdentity CreateIdentity(string userName, string password)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sets the user's lockout status
        /// </summary>
        public void SetLockout(string userName, bool v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the specified identity
        /// </summary>
        public void DeleteIdentity(string userName)
        {
            throw new NotImplementedException();
        }

        public IIdentity CreateIdentity(Guid sid, string userName, string password)
        {
            throw new NotImplementedException();
        }

        public IIdentity CreateIdentity(string userName, string password, IPrincipal authContext)
        {
            throw new NotImplementedException();
        }

        public string GenerateTfaSecret(string userName)
        {
            throw new NotImplementedException();
        }

        public void DeleteIdentity(string userName, IPrincipal authContext)
        {
            throw new NotImplementedException();
        }

        public void SetLockout(string userName, bool lockout, IPrincipal authContext)
        {
            throw new NotImplementedException();
        }

        public void AddClaim(string userName, Claim claim)
        {
            throw new NotImplementedException();
        }

        public void RemoveClaim(string userName, string claimType)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

