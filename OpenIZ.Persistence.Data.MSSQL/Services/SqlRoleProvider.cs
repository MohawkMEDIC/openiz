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
 * Date: 2016-6-14
 */
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
using OpenIZ.Persistence.Data.MSSQL.Configuration;
using System.Configuration;
using OpenIZ.Persistence.Data.MSSQL.Data;
using OpenIZ.Persistence.Data.MSSQL.Security;
using System.Security;
using System.Security.Principal;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;

namespace OpenIZ.Persistence.Data.MSSQL.Services
{
    /// <summary>
    /// Local role provider
    /// </summary>
    public class SqlRoleProvider : IRoleProviderService
    {
        /// <summary>
        /// Configuration 
        /// </summary>
        protected SqlConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.persistence.data.mssql") as SqlConfiguration;

        /// <summary>
        /// Verify principal
        /// </summary>
        private void VerifyPrincipal(IPrincipal authPrincipal, String policyId)
        {
            if (authPrincipal == null)
                throw new ArgumentNullException(nameof(authPrincipal));

            new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, policyId, authPrincipal).Demand();
            
        }

        /// <summary>
        /// Adds the specified users to the specified roles
        /// </summary>
        public void AddUsersToRoles(string[] users, string[] roles, IPrincipal authPrincipal)
        {
            this.VerifyPrincipal(authPrincipal, PermissionPolicyIdentifiers.AlterRoles);

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

            this.VerifyPrincipal(authPrincipal, PermissionPolicyIdentifiers.CreateRoles);

            // Add users to role
            using (var dataContext = new Data.ModelDataContext(this.m_configuration.ReadWriteConnectionString))
            {
                SecurityUser user = dataContext.SecurityUsers.SingleOrDefault(u => u.UserName == authPrincipal.Identity.Name);
                if (user == null)
                    throw new SecurityException(String.Format("Could not verify identity of {0}", authPrincipal.Identity.Name));

                // Insert
                dataContext.SecurityRoles.InsertOnSubmit(new SecurityRole()
                {
                    RoleId = Guid.NewGuid(),
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
        /// Get all rolesfor user
        /// </summary>
        /// <returns></returns>
        public string[] GetAllRoles(String userName)
        {
            using (var dataContext = new ModelDataContext(this.m_configuration.ReadonlyConnectionString))
                return dataContext.SecurityUsers.SingleOrDefault(o => o.UserName == userName)?.SecurityUserRoles.Select(r => r.SecurityRole.Name).ToArray();
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
            this.VerifyPrincipal(authPrincipal, PermissionPolicyIdentifiers.AlterRoles);

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

	                    var securityUserRole = user.SecurityUserRoles.SingleOrDefault(ur => ur.RoleId == role.RoleId && ur.UserId == user.UserId);

	                    if (securityUserRole != null)
	                    {
							// Remove
							dataContext.SecurityUserRoles.DeleteOnSubmit(securityUserRole);
						}
                    }
                }

                dataContext.SubmitChanges();
            }
        }
    }
}
