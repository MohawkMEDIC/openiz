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
 * Date: 2017-1-15
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
using OpenIZ.Persistence.Data.ADO.Configuration;
using System.Configuration;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Exceptions;
using MARC.HI.EHRS.SVC.Core.Event;
using System.Diagnostics;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Security.Claims;
using OpenIZ.Core.Services;
using OpenIZ.Persistence.Data.ADO.Security;
using OpenIZ.Persistence.Data.ADO.Data.Model.Security;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Data.ADO.Services.Persistence;

namespace OpenIZ.Persistence.Data.ADO.Services
{
    /// <summary>
    /// Identity provider service
    /// </summary>
    public sealed class AdoIdentityProvider : IIdentityRefreshProviderService
    {

        // Sync lock
        private Object m_syncLock = new object();

        // Trace source
        private TraceSource m_traceSource = new TraceSource(AdoDataConstants.IdentityTraceSourceName);

        // Configuration
        private AdoConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(AdoDataConstants.ConfigurationSectionName) as AdoConfiguration;

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
                var principal = AdoClaimsIdentity.Create(userName, password).CreateClaimsPrincipal();
                this.Authenticated?.Invoke(this, new AuthenticatedEventArgs(userName, principal, true));
                return principal;
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
        /// Gets an un-authenticated identity
        /// </summary>
        public IIdentity GetIdentity(string userName)
        {
            return AdoClaimsIdentity.Create(userName);
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
                using (var dataContext = this.m_configuration.Provider.GetWriteConnection())
                {
                    dataContext.Open();

                    using (var tx = dataContext.BeginTransaction())
                    {
                        try
                        {
                            var user = dataContext.FirstOrDefault<DbSecurityUser>(o => o.UserName.ToLower() == userName.ToLower());
                            if (user == null)
                                throw new KeyNotFoundException(userName);

                            var claims = dataContext.Query<DbUserClaim>(o => o.SourceKey == user.Key && (o.ClaimType == OpenIzClaimTypes.OpenIZTfaSecretClaim || o.ClaimType == OpenIzClaimTypes.OpenIZTfaSecretExpiry || o.ClaimType == OpenIzClaimTypes.OpenIZPasswordlessAuth));
                            DbUserClaim tfaClaim = claims.FirstOrDefault(o => o.ClaimType == OpenIzClaimTypes.OpenIZTfaSecretClaim),
                                tfaExpiry = claims.FirstOrDefault(o => o.ClaimType == OpenIzClaimTypes.OpenIZTfaSecretExpiry),
                                noPassword = claims.FirstOrDefault(o => o.ClaimType == OpenIzClaimTypes.OpenIZPasswordlessAuth);

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
                                retVal = AdoClaimsIdentity.Create(user, true, "Tfa+LastPasswordHash").CreateClaimsPrincipal();
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

                            tx.Commit();
                            return retVal;
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
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

            // Password failed validation
            if (ApplicationContext.Current.GetService<IPasswordValidatorService>()?.Validate(newPassword) == false)
                throw new SecurityException("Password failed validation");

            try
            {
                // Create the hasher and load the user
                using (var dataContext = this.m_configuration.Provider.GetWriteConnection())
                {
                    dataContext.Open();

                    using (var tx = dataContext.BeginTransaction())
                        try
                        {
                            var user = dataContext.SingleOrDefault<DbSecurityUser>(u => u.UserName.ToLower() == userName.ToLower() && !u.ObsoletionTime.HasValue);
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

                            user.PasswordHash = passwordHashingService.EncodePassword(newPassword);
                            user.SecurityHash = Guid.NewGuid().ToString();
                            user.UpdatedByKey = principal.GetUserKey(dataContext);

                            dataContext.Update(user);
                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }

                }
            }
            catch (Exception e)
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
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.CreateIdentity)]
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
                using (var dataContext = this.m_configuration.Provider.GetWriteConnection())
                {
                    dataContext.Open();

                    using (var tx = dataContext.BeginTransaction())
                        try
                        {
                            var hashingService = ApplicationContext.Current.GetService<IPasswordHashingService>();
                            var pdpService = ApplicationContext.Current.GetService<IPolicyDecisionService>();

                            // Demand create identity
                            new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PermissionPolicyIdentifiers.CreateIdentity, authContext).Demand();

                            // Does this principal have the ability to 
                            DbSecurityUser newIdentityUser = new DbSecurityUser()
                            {
                                UserName = userName,
                                PasswordHash = hashingService.EncodePassword(password),
                                SecurityHash = Guid.NewGuid().ToString(),
                                UserClass = UserClassKeys.HumanUser
                            };
                            if (authContext != null)
                                newIdentityUser.CreatedByKey = authContext.GetUserKey(dataContext).Value;

                            dataContext.Insert(newIdentityUser);
                            var retVal = AdoClaimsIdentity.Create(newIdentityUser);
                            tx.Commit();
                            return retVal;
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                }
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }

        }

        /// <summary>
        /// Delete the specified identity
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
        public void DeleteIdentity(string userName, IPrincipal authContext)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            this.m_traceSource.TraceInformation("Delete identity {0}", userName);
            try
            {
                // submit the changes
                using (var dataContext = this.m_configuration.Provider.GetWriteConnection())
                {
                    dataContext.Open();

                    new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PermissionPolicyIdentifiers.UnrestrictedAdministration, authContext).Demand();

                    var user = dataContext.FirstOrDefault<DbSecurityUser>(o => o.UserName.ToLower() == userName.ToLower());
                    if (user == null)
                        throw new KeyNotFoundException("Specified user does not exist!");

                    // Obsolete
                    user.ObsoletionTime = DateTimeOffset.Now;
                    user.ObsoletedByKey = authContext.GetUserKey(dataContext);
                    user.SecurityHash = Guid.NewGuid().ToString();

                    dataContext.Update(user);
                }

            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Set the lockout status
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
        public void SetLockout(string userName, bool lockout, IPrincipal authContext)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            this.m_traceSource.TraceInformation("Lockout identity {0} = {1}", userName, lockout);
            try
            {
                // submit the changes
                using (var dataContext = this.m_configuration.Provider.GetWriteConnection())
                {
                    dataContext.Open();
                    new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PermissionPolicyIdentifiers.UnrestrictedAdministration, authContext).Demand();

                    var user = dataContext.FirstOrDefault<DbSecurityUser>(o => o.UserName.ToLower() == userName.ToLower());
                    if (user == null)
                        throw new KeyNotFoundException("Specified user does not exist!");

                    // Obsolete
	                if (lockout)
		                user.Lockout = DateTime.MaxValue.AddDays(-10);
	                else
		                user.Lockout = null;

                    user.ObsoletionTime = null;
                    user.ObsoletedByKey = null;
                    user.UpdatedByKey = authContext.GetUserKey(dataContext);
                    user.UpdatedTime = DateTimeOffset.Now;
                    user.SecurityHash = Guid.NewGuid().ToString();

                    var updatedUser = dataContext.Update(user);

	                var securityUser = new SecurityUserPersistenceService().ToModelInstance(updatedUser, dataContext, authContext);
					ApplicationContext.Current.GetService<IDataCachingService>()?.Add(securityUser);
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
                using (var dataContext = this.m_configuration.Provider.GetWriteConnection())
                {
                    dataContext.Open();

                    var user = dataContext.FirstOrDefault<DbSecurityUser>(o => o.UserName.ToLower() == userName.ToLower());
                    if (user == null)
                        throw new KeyNotFoundException(userName);

                    lock (this.m_syncLock)
                    {
                        var existingClaim = dataContext.FirstOrDefault<DbUserClaim>(o => o.ClaimType == claim.Type && o.SourceKey == user.Key);

                        // Set the secret
                        if (existingClaim == null)
                        {
                            existingClaim = new DbUserClaim()
                            {
                                ClaimType = claim.Type,
                                ClaimValue = claim.Value,
                                SourceKey = user.Key
                            };
                            dataContext.Insert(existingClaim);
                        }
                        else
                        {
                            existingClaim.ClaimValue = claim.Value;
                            dataContext.Update(existingClaim);
                        }

                    }
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
                using (var dataContext = this.m_configuration.Provider.GetWriteConnection())
                {
                    dataContext.Open();

                    var user = dataContext.FirstOrDefault<DbSecurityUser>(o => o.UserName.ToLower() == userName.ToLower());
                    if (user == null)
                        throw new KeyNotFoundException(userName);

                    dataContext.Delete<DbUserClaim>(o => o.ClaimType == claimType && o.SourceKey == user.Key);
                }
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create and register a refresh token for the specified principal
        /// </summary>
        public byte[] CreateRefreshToken(IPrincipal principal, DateTimeOffset expiry)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (!principal.Identity.IsAuthenticated)
                throw new SecurityException("Can only generate refresh tokens for authenticated principals");

            var sid = (principal as ClaimsPrincipal).FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (sid == null)
                throw new InvalidOperationException("Cannot generate refresh claim for this principal");

            byte[] secret = new byte[32];

            // First we shall set the refresh claim
            byte[] refreshSecret = Guid.NewGuid().ToByteArray(),
                userSid = Guid.Parse(sid).ToByteArray();

            // Now interlace them
            for (var i = 0; i < refreshSecret.Length; i++)
            {
                secret[i * 2] = refreshSecret[i];
                secret[(i * 2) + 1] = userSid[i];
            }
            this.AddClaim(principal.Identity.Name, new Claim(AdoDataConstants.RefreshSecretClaimType, BitConverter.ToString(secret).Replace("-", "")));
            this.AddClaim(principal.Identity.Name, new Claim(AdoDataConstants.RefreshExpiryClaimType, expiry.ToString("o")));
            return secret;
        }

        /// <summary>
        /// Authenticate using a refresh token
        /// </summary>
        public IPrincipal Authenticate(byte[] refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));
            else if (refreshToken.Length != 32)
                throw new ArgumentOutOfRangeException(nameof(refreshToken), "Invalid refresh token");


            string trokenName = BitConverter.ToString(refreshToken).Replace("-", "");
            var evt = new AuthenticatingEventArgs(trokenName);
            this.Authenticating?.Invoke(this, evt);
            if (evt.Cancel)
                throw new SecurityException("Authentication cancelled");

            try
            {
                var principal = AdoClaimsIdentity.Create(refreshToken).CreateClaimsPrincipal();
                this.Authenticated?.Invoke(this, new AuthenticatedEventArgs(trokenName, principal, true));
                return principal;
            }
            catch (SecurityException e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Verbose, e.HResult, "Invalid credentials : {0}", refreshToken);

                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                this.Authenticated?.Invoke(this, new AuthenticatedEventArgs(trokenName, null, false));
                throw;
            }
        }
    }
}
