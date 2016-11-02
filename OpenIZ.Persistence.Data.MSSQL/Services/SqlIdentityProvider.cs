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
using MARC.HI.EHRS.SVC.Core.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Security;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Security;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Persistence.Data.MSSQL.Configuration;
using System.Configuration;
using OpenIZ.Persistence.Data.MSSQL.Data;
using OpenIZ.Core.Security;
using OpenIZ.Persistence.Data.MSSQL.Security;
using MARC.HI.EHRS.SVC.Core.Exceptions;
using MARC.HI.EHRS.SVC.Core.Event;
using System.Diagnostics;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Security.Claims;

namespace OpenIZ.Persistence.Data.MSSQL.Services
{
    /// <summary>
    /// Identity provider service
    /// </summary>
    public sealed class SqlIdentityProvider : IIdentityProviderService
    {

        // Trace source
        private TraceSource m_traceSource = new TraceSource(SqlServerConstants.IdentityTraceSourceName);

        // Configuration
        private SqlConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(SqlServerConstants.ConfigurationSectionName) as SqlConfiguration;

        /// <summary>
        /// Fired prior to an authentication request being made
        /// </summary>
        public event EventHandler<AuthenticatingEventArgs> Authenticating;

        /// <summary>
        /// Fired after an authentication request has been made
        /// </summary>
        public event EventHandler<AuthenticatedEventArgs> Authenticated;

        /// <summary>
        /// Authenticate the user
        /// </summary>
        public IPrincipal Authenticate(string userName, string password)
        {
            var evt = new AuthenticatingEventArgs(userName);
            this.Authenticating?.Invoke(this, evt);
            if (evt.Cancel)
                throw new SecurityException("Authentication cancelled");

            try
            {
                var principal = SqlClaimsIdentity.Create(userName, password).CreateClaimsPrincipal();
                this.Authenticated?.Invoke(this, new AuthenticatedEventArgs(userName, principal, true));
                return principal;
            }
            catch(SecurityException e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Verbose, e.HResult, "Invalid credentials : {0}/{1}", userName, password);

                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                this.Authenticated?.Invoke(this, new AuthenticatedEventArgs(userName, null, false));
                throw;
            }
        }

        /// <summary>
        /// Gets an un-authenticated identity
        /// </summary>
        public IIdentity GetIdentity(string userName)
        {
            return SqlClaimsIdentity.Create(userName);
        }

        /// <summary>
        /// Authenticate the user using a TwoFactorAuthentication secret
        /// </summary>
        public IPrincipal Authenticate(string userName, string password, string tfaSecret)
        {
            // First, let's verify the TFA
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            else if (String.IsNullOrEmpty(tfaSecret))
                throw new ArgumentNullException(nameof(tfaSecret));

            // Authentication event args
            var evt = new AuthenticatingEventArgs(userName);
            this.Authenticating?.Invoke(this, evt);
            if (evt.Cancel)
                throw new SecurityException("Authentication cancelled");

            // Password hasher
            var hashingService = ApplicationContext.Current.GetService<IPasswordHashingService>();
            tfaSecret = hashingService.EncodePassword(tfaSecret);

            // Try to authenticate
            try
            {
                using (var dataContext = new ModelDataContext(this.m_configuration.ReadWriteConnectionString))
                {
                    var user = dataContext.SecurityUsers.FirstOrDefault(o => o.UserName == userName);
                    if (user == null)
                        throw new KeyNotFoundException(userName);
                    SecurityUserClaim tfaClaim = dataContext.SecurityUserClaims.FirstOrDefault(o => o.UserId == user.Id && o.ClaimType == OpenIzClaimTypes.OpenIZTfaSecretClaim),
                        tfaExpiry = dataContext.SecurityUserClaims.FirstOrDefault(o => o.UserId == user.Id && o.ClaimType == OpenIzClaimTypes.OpenIZTfaSecretExpiry),
                        noPassword = dataContext.SecurityUserClaims.FirstOrDefault(o => o.UserId == user.Id && o.ClaimType == OpenIzClaimTypes.OpenIZPasswordlessAuth);

                    if (tfaClaim == null || tfaExpiry == null)
                        throw new InvalidOperationException("Cannot find appropriate claims for TFA");

                    // Expiry check
                    ClaimsPrincipal retVal = null;
                    DateTime expiryDate = DateTime.Parse(tfaExpiry.ClaimValue);
                    if (expiryDate < DateTime.Now)
                        throw new SecurityException("TFA secret expired");
                    else if (String.IsNullOrEmpty(password) &&
                        Boolean.Parse(noPassword?.ClaimValue ?? "false") &&
                        tfaSecret == tfaClaim.ClaimValue) // Last known password hash sent as password, this is a password reset token - It will be set to expire ASAP
                    {
                        retVal = SqlClaimsIdentity.Create(user, true, "Tfa+LastPasswordHash").CreateClaimsPrincipal();
                        (retVal.Identity as ClaimsIdentity).AddClaim(new Claim(OpenIzClaimTypes.OpenIzGrantedPolicyClaim, PermissionPolicyIdentifiers.ChangePassword));
                        (retVal.Identity as ClaimsIdentity).RemoveClaim(retVal.FindFirst(ClaimTypes.Expiration));
                        // TODO: Add to configuration
                        (retVal.Identity as ClaimsIdentity).AddClaim(new Claim(ClaimTypes.Expiration, DateTime.Now.AddMinutes(5).ToString("o")));
                    }
                    else if (!String.IsNullOrEmpty(password))
                        retVal = this.Authenticate(userName, password) as ClaimsPrincipal;
                    else
                        throw new PolicyViolationException(PermissionPolicyIdentifiers.Login, PolicyDecisionOutcomeType.Deny);

                    // Now we want to fire authenticated
                    this.Authenticated?.Invoke(this, new AuthenticatedEventArgs(userName, retVal, true));
                    return retVal;

                }
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Verbose, e.HResult, "Invalid credentials : {0}/{1}", userName, password);
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                this.Authenticated?.Invoke(this, new AuthenticatedEventArgs(userName, null, false));
                throw;
            }
        }

        /// <summary>
        /// Change the user's password
        /// </summary>
        public void ChangePassword(string userName, string newPassword, IPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (!principal.Identity.IsAuthenticated)
                throw new SecurityException("Authorization context must be authenticated");

            this.m_traceSource.TraceInformation("Change userpassword for {0} to {1} ({2})", userName, newPassword, principal);

            try
            {
                // Create the hasher and load the user
                using (var dataContext = new ModelDataContext(this.m_configuration.ReadWriteConnectionString))
                {
                    var user = dataContext.SecurityUsers.Where(u => u.UserName == userName && !u.ObsoletionTime.HasValue).SingleOrDefault();
                    if (user == null)
                        throw new InvalidOperationException(String.Format("Cannot locate user {0}", userName));

                    // Security check
                    var policyDecisionService = ApplicationContext.Current.GetService<IPolicyDecisionService>();
                    var passwordHashingService = ApplicationContext.Current.GetService<IPasswordHashingService>();

                    var pdpOutcome = policyDecisionService?.GetPolicyOutcome(principal, PermissionPolicyIdentifiers.ChangePassword);
                    if (userName != principal.Identity.Name &&
                        pdpOutcome.HasValue &&
                        pdpOutcome != PolicyDecisionOutcomeType.Grant)
                        throw new PolicyViolationException(PermissionPolicyIdentifiers.ChangePassword, pdpOutcome.Value);

                    user.UserPassword = passwordHashingService.EncodePassword(newPassword);
                    dataContext.SubmitChanges();
                }
            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Generate and store the TFA secret
        /// </summary>
        public string GenerateTfaSecret(string userName)
        {
            // This is a simple TFA generator
            var secret = ApplicationContext.Current.GetService<ITwoFactorSecretGenerator>().GenerateTfaSecret();
            var hashingService = ApplicationContext.Current.GetService<IPasswordHashingService>();

            this.AddClaim(userName, new Claim(OpenIzClaimTypes.OpenIZTfaSecretClaim, hashingService.EncodePassword(secret)));
            this.AddClaim(userName, new Claim(OpenIzClaimTypes.OpenIZTfaSecretExpiry, DateTime.Now.AddMinutes(5).ToString("o")));

            return secret;
        }

        /// <summary>
        /// Create a basic user
        /// </summary>
        public IIdentity CreateIdentity(string userName, string password, IPrincipal authContext)
        {

            if (String.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            else if (String.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            else if (authContext == null)
                throw new ArgumentNullException(nameof(authContext));

            this.m_traceSource.TraceInformation("Creating identity {0} ({1})", userName, authContext);

            try
            {
                using (var dataContext = new ModelDataContext(this.m_configuration.ReadWriteConnectionString))
                {
                    var hashingService = ApplicationContext.Current.GetService<IPasswordHashingService>();
                    var pdpService = ApplicationContext.Current.GetService<IPolicyDecisionService>();

                    // Demand create identity
                    new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PermissionPolicyIdentifiers.CreateIdentity, authContext).Demand();

                    // Does this principal have the ability to 
                    Data.SecurityUser newIdentityUser = new Data.SecurityUser()
                    {
                        UserId = Guid.NewGuid(),
                        UserName = userName,
                        UserPassword = hashingService.EncodePassword(password),
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserClass = UserClassKeys.HumanUser
                    };

                    if (authContext != null)
                        newIdentityUser.CreatedByEntity = dataContext.SecurityUsers.Single(u => u.UserName == authContext.Identity.Name);

                    dataContext.SecurityUsers.InsertOnSubmit(newIdentityUser);
                    dataContext.SubmitChanges();

                    return SqlClaimsIdentity.Create(newIdentityUser);

                }
            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }

        }

        /// <summary>
        /// Delete the specified identity
        /// </summary>
        public void DeleteIdentity(string userName, IPrincipal authContext)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            this.m_traceSource.TraceInformation("Delete identity {0}", userName);
            try
            {
                // submit the changes
                using (var dataContext = new ModelDataContext(this.m_configuration.ReadWriteConnectionString))
                {
                    new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PermissionPolicyIdentifiers.UnrestrictedAdministration, authContext).Demand();

                    var user = dataContext.SecurityUsers.FirstOrDefault(o => o.UserName == userName);
                    if (user == null)
                        throw new KeyNotFoundException("Specified user does not exist!");

                    // Obsolete
                    user.ObsoletionTime = DateTimeOffset.Now;
                    user.ObsoletedByEntity = authContext.GetUser(dataContext);
                    user.SecurityStamp = Guid.NewGuid().ToString();

                    dataContext.SubmitChanges();
                }

            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Set the lockout status
        /// </summary>
        public void SetLockout(string userName, bool lockout, IPrincipal authContext)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            this.m_traceSource.TraceInformation("Lockout identity {0} = {1}", userName, lockout);
            try
            {
                // submit the changes
                using (var dataContext = new ModelDataContext(this.m_configuration.ReadWriteConnectionString))
                {
                    new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PermissionPolicyIdentifiers.UnrestrictedAdministration, authContext).Demand();

                    var user = dataContext.SecurityUsers.FirstOrDefault(o => o.UserName == userName);
                    if (user == null)
                        throw new KeyNotFoundException("Specified user does not exist!");

                    // Obsolete
					if (lockout)
					{
						user.Lockout = DateTimeOffset.Now;
					}
                    user.ObsoletionTime = null;
                    user.ObsoletedBy = null;
                    user.UpdatedByEntity = authContext.GetUser(dataContext);
                    user.UpdatedTime = DateTimeOffset.Now;
                    user.SecurityStamp = Guid.NewGuid().ToString();

                    dataContext.SubmitChanges();
                }

            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Add a claim to the specified user
        /// </summary>
        public void AddClaim(string userName, Claim claim)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));
            else if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            try
            {
                using (var dataContext = new ModelDataContext(this.m_configuration.ReadWriteConnectionString))
                {
                    var user = dataContext.SecurityUsers.FirstOrDefault(o => o.UserName == userName);
                    if (user == null)
                        throw new KeyNotFoundException(userName);

                    var existingClaim = dataContext.SecurityUserClaims.FirstOrDefault(o => o.ClaimType == claim.Type && o.UserId == user.Id);

                    // Set the secret
                    if (existingClaim == null)
                        dataContext.SecurityUserClaims.InsertOnSubmit(new SecurityUserClaim()
                        {
                            ClaimId = Guid.NewGuid(),
                            ClaimType = claim.Type,
                            ClaimValue = claim.Value,
                            SecurityUser = user
                        });
                    else
                        existingClaim.ClaimValue = claim.Value;

                    dataContext.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Remove the specified claim
        /// </summary>
        public void RemoveClaim(string userName, string claimType)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));
            else if (claimType == null)
                throw new ArgumentNullException(nameof(claimType));

            try
            {
                using (var dataContext = new ModelDataContext(this.m_configuration.ReadWriteConnectionString))
                {
                    var user = dataContext.SecurityUsers.FirstOrDefault(o => o.UserName == userName);
                    if (user == null)
                        throw new KeyNotFoundException(userName);

                    var existingClaim = dataContext.SecurityUserClaims.FirstOrDefault(o => o.ClaimType == claimType && o.UserId == user.Id);

                    // Set the secret
                    if (existingClaim != null)
                        dataContext.SecurityUserClaims.DeleteOnSubmit(existingClaim);

                    dataContext.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }
    }
}
