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
 * Date: 2016-1-19
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Exceptions;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Persistence.Data.MSSQL.Configuration;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Security
{
    /// <summary>
    /// Represents a user prinicpal based on a SecurityUser domain model 
    /// </summary>
    public class SqlClaimsIdentity : IIdentity
    {
        // Trace source
        private static TraceSource s_traceSource = new TraceSource(SqlServerConstants.IdentityTraceSourceName);
        
        // Whether the user is authenticated
        private bool m_isAuthenticated;
        // The security user
        private SecurityUser m_securityUser;
        // The authentication type
        private String m_authenticationType;
        // Issued on
        private DateTimeOffset m_issuedOn = DateTimeOffset.Now;
        // Roles
        private List<SecurityRole> m_roles = null;

        // Configuration
        private static SqlConfiguration s_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(SqlServerConstants.ConfigurationSectionName) as SqlConfiguration;

        /// <summary>
        /// Creates a principal based on username and password
        /// </summary>
        internal static SqlClaimsIdentity Create(String userName, String password)
        {
            try
            {
                using (var dataContext = new Data.ModelDataContext(s_configuration.ReadWriteConnectionString))
                {
                    // Attempt to get a user
                    var hashingService = ApplicationContext.Current.GetService<IPasswordHashingService>();

                    var passwordHash = hashingService.EncodePassword(password);
                    var user = dataContext.SecurityUsers.SingleOrDefault(u => u.UserName == userName && u.ObsoletionTime == null);
                    if (user?.UserPassword == passwordHash && (bool)!user?.TwoFactorEnabled &&
                        (bool)!user?.LockoutEnabled)
                    {
                        var userIdentity = new SqlClaimsIdentity(user, true) { m_authenticationType = "Password" };

                        // Is user allowed to login?
                        try
                        {
                            new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PermissionPolicyIdentifiers.Login, new GenericPrincipal(userIdentity, null)).Demand();
                            user.LastSuccessfulLogin = DateTimeOffset.Now;
                            user.FailedLoginAttempts = 0;
                            dataContext.SubmitChanges();
                            return userIdentity;
                        }
                        catch (PolicyViolationException e)
                        {
                            user.FailedLoginAttempts++;
                            dataContext.SubmitChanges();
                            throw;
                        }
                    }
                    else if (user == null)
                        throw new SecurityException("Invalid username/password");
                    else if ((bool)user?.LockoutEnabled)
                    {
                        user.FailedLoginAttempts++;
                        dataContext.SubmitChanges();
                        throw new SecurityException("Account is locked");
                    }
                    else if (user != null)
                    {
                        user.FailedLoginAttempts++;
                        if (user.FailedLoginAttempts > 3) // TODO: Add this to configuration
                            user.LockoutEnabled = true;
                        dataContext.SubmitChanges();
                        throw new SecurityException("Invalid username/password");
                    }
                    else
                        throw new InvalidOperationException("Shouldn't be here");

                }
            }
            catch(SecurityException)
            {
                throw;
            }
            catch(Exception e)
            {
                s_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw new Exception("Creating identity failed", e);
            }
        }

        /// <summary>
        /// Create a identity from certificate file
        /// </summary>
        internal static SqlClaimsIdentity Create(X509Certificate2 userCertificate)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Create a identity from certificate + password = TFA (ex: Smart Card)
        /// </summary>
        internal static SqlClaimsIdentity Create(String userName, String password, X509Certificate2 userCertificate)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Create a claims identity from a data context user
        /// </summary>
        internal static SqlClaimsIdentity Create(SecurityUser user)
        {
            return new SqlClaimsIdentity(user, false);
        }

        /// <summary>
        /// Creates an identity from a hash
        /// </summary>
        internal static SqlClaimsIdentity Create(String userName)
        {
            try
            {
                using (var dataContext = new Data.ModelDataContext(s_configuration.ReadonlyConnectionString))
                {
                    var user = dataContext.SecurityUsers.SingleOrDefault(u => !u.ObsoletionTime.HasValue && u.UserName == userName);
                    if (user == null)
                        return null;
                    return new SqlClaimsIdentity(user, false);
                }
            }
            catch (Exception e)
            {
                s_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw new Exception("Creating unauthorized identity failed", e);
            }
        }

        /// <summary>
        /// Private ctor
        /// </summary>
        private SqlClaimsIdentity(SecurityUser user, bool isAuthenticated)
        {
            this.m_isAuthenticated = isAuthenticated;
            this.m_securityUser = user;
            this.m_roles = user.SecurityUserRoles.Select(o => o.SecurityRole).ToList(); 
        }

        /// <summary>
        /// Gets the authentication type
        /// </summary>
        public string AuthenticationType
        {
            get
            {
                return this.m_authenticationType;
            }
        }

        /// <summary>
        /// Whether the principal is autheticated
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return this.m_isAuthenticated;
            }
        }

        /// <summary>
        /// Gets or sets the name of the user
        /// </summary>
        public string Name
        {
            get
            {
                return this.m_securityUser.UserName;
            }
        }

        /// <summary>
        /// Gets the original user upon which this principal is based
        /// </summary>
        public SecurityUser User
        {
            get
            {
                return this.m_securityUser;
            }
        }

        /// <summary>
        /// Create an authorization context
        /// </summary>
        public ClaimsPrincipal CreateClaimsPrincipal()
        {

            if (!this.m_isAuthenticated)
                throw new SecurityException("Principal is not authenticated");

            try
            {

                // System claims
                List<Claim> claims = new List<Claim>(
                    this.m_roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r.Name))
                )
                {
                    new Claim(ClaimTypes.Authentication, this.m_isAuthenticated.ToString()),
                    new Claim(ClaimTypes.AuthenticationInstant, this.m_issuedOn.ToString("o")), // TODO: Fix this
                    new Claim(ClaimTypes.AuthenticationMethod, this.m_authenticationType),
                    new Claim(ClaimTypes.Expiration, this.m_issuedOn.AddMinutes(30).ToString("o")), // TODO: Move this to configuration
                    new Claim(ClaimTypes.Name, this.m_securityUser.UserName),
                    new Claim(ClaimTypes.Sid, this.m_securityUser.UserId.ToString()),
                    new Claim(OpenIzClaimTypes.XspaUserIdentifierClaim, this.m_securityUser.UserId.ToString())
                };

                if (this.m_securityUser.Email != null && this.m_securityUser.EmailConfirmed)
                    claims.Add(new Claim(ClaimTypes.Email, this.m_securityUser.Email));

                var retVal = new ClaimsPrincipal(
                        new ClaimsIdentity[] { new ClaimsIdentity(this, claims.AsReadOnly()) }
                    );
                s_traceSource.TraceInformation("Created security principal from identity {0} > {1}", this, SqlClaimsIdentity.PrincipalToString(retVal));
                return retVal;
            }
            catch (Exception e)
            {
                s_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw new Exception("Creating principal from identity failed", e);
            }
        }

        /// <summary>
        /// Return string representation of the identity
        /// </summary>
        public override string ToString()
        {
            return String.Format("SqlClaimsIdentity(name={0}, auth={1}, mode={2})", this.Name, this.IsAuthenticated, this.AuthenticationType);
        }

        /// <summary>
        /// Represent principal as a string
        /// </summary>
        private static String PrincipalToString(ClaimsPrincipal retVal)
        {
            using (StringWriter sw = new StringWriter())
            {
                sw.Write("{{ Identity = {0}, Claims = [", retVal.Identity);
                foreach(var itm in retVal.Claims)
                {
                    sw.Write("{{ Type = {0}, Value = {1} }}", itm.Type, itm.Value);
                    if (itm != retVal.Claims.Last()) sw.Write(",");
                }
                sw.Write("] }");
                return sw.ToString();
            }
            
        }
    }
}
