using MARC.HI.EHRS.SVC.Core.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Exceptions;
using OpenIZ.Core.Security;
using OpenIZ.Persistence.Data.MSSQL.Configuration;
using System.Configuration;
using OpenIZ.Persistence.Data.MSSQL.Data;
using OpenIZ.Persistence.Data.MSSQL.Security;
using System.Security;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.MSSQL.Services
{
    /// <summary>
    /// Local role provider
    /// </summary>
    public class SqlRoleProvider : IRoleProviderService
    {
        // Configuration
        protected SqlConfiguration m_configuration = ConfigurationManager.GetSection("openiz.persistence.data.mssql") as SqlConfiguration;

        /// <summary>
        /// Verify principal
        /// </summary>
        private void VerifyPrincipal(IPrincipal authPrincipal, String policyCheck)
        {
            if (authPrincipal == null)
                throw new ArgumentNullException(nameof(authPrincipal));
            else if (!authPrincipal.Identity.IsAuthenticated)
                throw new SecurityException("Principal must be authenticated");

            if (policyCheck != null)
            {
                var policyService = ApplicationContext.Current.GetService<IPolicyDecisionService>();
                if (policyService != null)
                {
                    var policyDecision = policyService.GetPolicyOutcome(authPrincipal, PolicyIdentifiers.OpenIzCreateRolesPolicy);
                    if (policyDecision != PolicyDecisionOutcomeType.Grant)
                        throw new PolicyViolationException(PolicyIdentifiers.OpenIzCreateRolesPolicy, policyDecision);
                }
            }
            
        }

        /// <summary>
        /// Adds the specified users to the specified roles
        /// </summary>
        public void AddUsersToRoles(string[] users, string[] roles, IPrincipal authPrincipal)
        {
            this.VerifyPrincipal(authPrincipal, PolicyIdentifiers.OpenIzAlterRolePolicy);

            // Add users to role
            using (var dataContext = new Data.ModelDataContext(this.m_configuration.ReadWriteConnectionString))
            {
                foreach(var un in users)
                {
                    SecurityUser user = dataContext.SecurityUsers.SingleOrDefault(u => u.UserName == un);
                    if (user == null)
                        throw new KeyNotFoundException(String.Format("Could not locate user {0}", un));
                    foreach(var rol in roles)
                    {
                        SecurityRole role = dataContext.SecurityRoles.SingleOrDefault(r => r.Name == rol);
                        if (role == null)
                            throw new KeyNotFoundException(String.Format("Could not locate role {0}", rol));
                        if (!user.SecurityUserRoles.Any(o => o.RoleId == role.RoleId))
                            user.SecurityUserRoles.Add(new SecurityUserRole() { UserId = user.UserId, RoleId = role.RoleId });
                    }
                }

                dataContext.SubmitChanges();
            }
        }

        /// <summary>
        /// Create a role
        /// </summary>
        public void CreateRole(string roleName, IPrincipal authPrincipal)
        {

            this.VerifyPrincipal(authPrincipal, PolicyIdentifiers.OpenIzCreateRolesPolicy);

            // Add users to role
            using (var dataContext = new Data.ModelDataContext(this.m_configuration.ReadWriteConnectionString))
            {
                SecurityUser user = dataContext.SecurityUsers.SingleOrDefault(u => u.UserName == authPrincipal.Identity.Name);
                if (user == null)
                    throw new SecurityException(String.Format("Could not verify identity of {0}", authPrincipal.Identity.Name));

                // Insert
                dataContext.SecurityRoles.InsertOnSubmit(new SecurityRole()
                {
                    CreatedByEntity = user,
                    Name = roleName
                });
                dataContext.SubmitChanges();
            }
        }

        /// <summary>
        /// Find all users in a role
        /// </summary>
        public string[] FindUsersInRole(string role)
        {
            using (var dataContext = new ModelDataContext(this.m_configuration.ReadonlyConnectionString))
            {
                var securityRole = dataContext.SecurityRoles.SingleOrDefault(r => r.Name == role);
                if (securityRole == null)
                    throw new KeyNotFoundException(String.Format("Role {0} not found", role));
                return securityRole.SecurityUserRoles.Select(o => o.SecurityUser.UserName).ToArray();
            }
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns></returns>
        public string[] GetAllRoles()
        {
            using (var dataContext = new ModelDataContext(this.m_configuration.ReadonlyConnectionString))
                return dataContext.SecurityRoles.Select(o => o.Name).ToArray();
        }

        /// <summary>
        /// Determine if the user is in the specified role
        /// </summary>
        public bool IsUserInRole(IPrincipal principal, string roleName)
        {
            return this.IsUserInRole(principal.Identity.Name, roleName);
        }

        /// <summary>
        /// Determine if user is in role
        /// </summary>
        public bool IsUserInRole(string userName, string roleName)
        {
            using (var dataContext = new ModelDataContext(this.m_configuration.ReadonlyConnectionString))
                return dataContext.SecurityUserRoles.Any(ur => ur.SecurityRole.Name == roleName && ur.SecurityUser.UserName == userName);
        }

        /// <summary>
        /// Remove users from roles
        /// </summary>
        public void RemoveUsersFromRoles(string[] users, string[] roles, IPrincipal authPrincipal)
        {
            this.VerifyPrincipal(authPrincipal, PolicyIdentifiers.OpenIzAlterRolePolicy);

            // Add users to role
            using (var dataContext = new Data.ModelDataContext(this.m_configuration.ReadWriteConnectionString))
            {
                foreach (var un in users)
                {
                    SecurityUser user = dataContext.SecurityUsers.SingleOrDefault(u => u.UserName == un);
                    if (user == null)
                        throw new KeyNotFoundException(String.Format("Could not locate user {0}", un));
                    foreach (var rol in roles)
                    {
                        SecurityRole role = dataContext.SecurityRoles.SingleOrDefault(r => r.Name == rol);
                        if (role == null)
                            throw new KeyNotFoundException(String.Format("Could not locate role {0}", rol));

                        // Remove
                        dataContext.SecurityUserRoles.DeleteOnSubmit(user.SecurityUserRoles.SingleOrDefault(ur => ur.RoleId == role.RoleId && ur.UserId == user.UserId));
                    }
                }

                dataContext.SubmitChanges();
            }
        }
    }
}
