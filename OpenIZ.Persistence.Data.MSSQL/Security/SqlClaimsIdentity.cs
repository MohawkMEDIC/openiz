using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Persistence.Data.MSSQL.Configuration;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static SqlConfiguration s_configuration = ConfigurationManager.GetSection("openiz.persistence.data.mssql") as SqlConfiguration;

        /// <summary>
        /// Creates a principal based on username and password
        /// </summary>
        internal static SqlClaimsIdentity Create(String userName, String password)
        {
            using (var dataContext = new Data.ModelDataContext(s_configuration.ReadonlyConnectionString))
            {
                // Attempt to get a user
                var hasher = SHA256.Create();
                var passwordHash = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(password)));
                var user = dataContext.SecurityUsers.FirstOrDefault(u => u.UserName == userName && u.ObsoletionTime == null);
                if (user?.UserPassword == passwordHash && (bool)!user?.TwoFactorEnabled &&
                    (bool)!user?.LockoutEnabled)
                {
                    user.LastSuccessfulLogin = DateTimeOffset.Now;
                    user.FailedLoginAttempts = 0;
                    dataContext.SubmitChanges();
                    return new SqlClaimsIdentity(user, true) { m_authenticationType = "Password" };
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
        /// Creates an identity from a hash
        /// </summary>
        internal static SqlClaimsIdentity Create(String userName)
        {
            using (var dataContext = new Data.ModelDataContext(s_configuration.ReadonlyConnectionString))
            {
                var user = dataContext.SecurityUsers.FirstOrDefault(u => !u.ObsoletionTime.HasValue && u.UserName == userName);
                if (user == null)
                    return null;
                return new SqlClaimsIdentity(user, false);
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

            // System claims
            List<Claim> claims = new List<Claim>(
                this.m_roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r.Name, "xs:string", ApplicationContext.Current.Configuration.Custodianship.Name))
            )
            {
                new Claim(ClaimTypes.Authentication, this.m_isAuthenticated.ToString()),
                new Claim(ClaimTypes.AuthenticationInstant, this.m_issuedOn.ToString()), // TODO: Fix this
                new Claim(ClaimTypes.AuthenticationMethod, this.m_authenticationType),
                new Claim(ClaimTypes.Email, this.m_securityUser.Email),
                new Claim(ClaimTypes.Expiration, this.m_issuedOn.AddMinutes(30).ToString()), // TODO: Move this to configuration
                new Claim(ClaimTypes.Name, this.m_securityUser.UserName),
                new Claim(ClaimTypes.Sid, this.m_securityUser.UserId.ToString())
            };

            return new ClaimsPrincipal(
                    new ClaimsIdentity[] { new ClaimsIdentity(this, claims.AsReadOnly()) }
                );
        }

    }
}
