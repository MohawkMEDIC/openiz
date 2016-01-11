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

namespace OpenIZ.Persistence.Data.MSSQL
{
    /// <summary>
    /// Identity provider service
    /// </summary>
    public class SqlIdentityProvider : IIdentityProviderService
    {

        // Configuration
        protected SqlConfiguration m_configuration = ConfigurationManager.GetSection("openiz.persistence.data.mssql") as SqlConfiguration;

        /// <summary>
        /// Gets or sets the two factor secret generator
        /// </summary>
        public ITwoFactorSecretGenerator TwoFactorSecretGenerator { get; set; }

        /// <summary>
        /// Authenticate the user
        /// </summary>
        public ClaimsPrincipal Authenticate(string userName, string password)
        {
            return SqlClaimsIdentity.Create(userName, password).CreateClaimsPrincipal();
        }

        /// <summary>
        /// Authenticate the user using a TwoFactorAuthentication secret
        /// </summary>
        public ClaimsPrincipal Authenticate(string userName, string password, string tfaSecret)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Change the user's password
        /// </summary>
        public void ChangePassword(string userName, string newPassword, ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (!principal.Identity.IsAuthenticated)
                throw new SecurityException("Authorization context must be authenticated");

            // Create the hasher and load the user
            SHA256 hasher = SHA256.Create();
            using (var dataContext = new ModelDataContext(this.m_configuration.ReadWriteConnectionString))
            {
                var user = dataContext.SecurityUsers.Where(u => u.UserName == userName && !u.ObsoletionTime.HasValue).FirstOrDefault();
                if (user == null)
                    throw new InvalidOperationException(String.Format("Cannot locate user {0}", userName));

                // Security check
                var policyDecisionService = ApplicationContext.Current.GetService<IPolicyDecisionService>();

                var pdpOutcome = policyDecisionService?.GetPolicyOutcome(principal, PolicyIdentifiers.OpenIzChangePasswordPolicy);
                if (userName == principal.Identity.Name ||
                    pdpOutcome != PolicyDecisionOutcomeType.Grant)
                    throw new PolicyViolationException(PolicyIdentifiers.OpenIzChangePasswordPolicy, pdpOutcome.Value);

                user.UserPassword = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(newPassword)));
                dataContext.SubmitChanges();
            }
        }

        /// <summary>
        /// Generate and store the TFA secret
        /// </summary>
        public string GenerateTfaSecret(string userName)
        {
            throw new NotImplementedException();
        }
    }
}
