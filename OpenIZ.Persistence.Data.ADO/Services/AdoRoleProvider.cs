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
using System.Security.Claims;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Exceptions;
using OpenIZ.Persistence.Data.ADO.Configuration;
using System.Configuration;
using OpenIZ.Persistence.Data.ADO.Data;
using System.Security;
using System.Security.Principal;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Persistence.Data.ADO.Data.Model.Security;
using System.Diagnostics;
using OpenIZ.OrmLite;

namespace OpenIZ.Persistence.Data.ADO.Services
{
    /// <summary>
    /// Local role provider
    /// </summary>
    public class AdoRoleProvider : IRoleProviderService
    {

        // Tracer
        private TraceSource m_tracer = new TraceSource(AdoDataConstants.IdentityTraceSourceName);

        /// <summary>
        /// Configuration 
        /// </summary>
        protected AdoConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(AdoDataConstants.ConfigurationSectionName) as AdoConfiguration;

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
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
        public void AddUsersToRoles(string[] users, string[] roles, IPrincipal authPrincipal)
        {
            this.VerifyPrincipal(authPrincipal, PermissionPolicyIdentifiers.AlterRoles);

            // Add users to role
            using (DataContext dataContext = this.m_configuration.Provider.GetWriteConnection())
            {
                try
                {
                    dataContext.Open();
                    using (var tx = dataContext.BeginTransaction())
                    {
                        try
                        {
                            foreach (var un in users)
                            {
                                DbSecurityUser user = dataContext.SingleOrDefault<DbSecurityUser>(u => u.UserName.ToLower() == un.ToLower());
                                if (user == null)
                                    throw new KeyNotFoundException(String.Format("Could not locate user {0}", un));
                                foreach (var rol in roles)
                                {
                                    DbSecurityRole role = dataContext.SingleOrDefault<DbSecurityRole>(r => r.Name == rol);
                                    if (role == null)
                                        throw new KeyNotFoundException(String.Format("Could not locate role {0}", rol));
                                    if (!dataContext.Any<DbSecurityUserRole>(o => o.RoleKey == role.Key && o.UserKey == user.Key))
                                    {
                                        // Insert
                                        dataContext.Insert(new DbSecurityUserRole() { UserKey = user.Key, RoleKey = role.Key });
                                    }
                                }
                            }
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
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error adding {0} to {1} : {2}", String.Join(",", users), String.Join(",", roles), e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Create a role
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.CreateRoles)]
        public void CreateRole(string roleName, IPrincipal authPrincipal)
        {

            this.VerifyPrincipal(authPrincipal, PermissionPolicyIdentifiers.CreateRoles);

            // Add users to role
            using (DataContext dataContext = this.m_configuration.Provider.GetWriteConnection())
            {
                try
                {
                    dataContext.Open();
                    using (var tx = dataContext.BeginTransaction())
                    {
                        try
                        {
                            DbSecurityUser user = dataContext.SingleOrDefault<DbSecurityUser>(u => u.UserName.ToLower() == authPrincipal.Identity.Name.ToLower());
                            if (user == null)
                                throw new SecurityException(String.Format("Could not verify identity of {0}", authPrincipal.Identity.Name));

                            // Insert
                            dataContext.Insert(new DbSecurityRole()
                            {
                                CreatedByKey = user.Key,
                                Name = roleName
                            });
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
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error creating role {0} : {1}", roleName, e);
                    throw;
                }
            }

        }

        /// <summary>
        /// Find all users in a role
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterRoles)]
        public string[] FindUsersInRole(string role)
        {
            using (DataContext dataContext = this.m_configuration.Provider.GetReadonlyConnection())
            {
                try
                {
                    dataContext.Open();
                    var securityRole = dataContext.SingleOrDefault<DbSecurityRole>(r => r.Name == role);
                    if (securityRole == null)
                        throw new KeyNotFoundException(String.Format("Role {0} not found", role));

                    var query = dataContext.CreateSqlStatement<DbSecurityUserRole>().SelectFrom()
                        .InnerJoin<DbSecurityUser, DbSecurityUserRole>()
                        .Where(o => o.RoleKey == securityRole.Key);

                    return dataContext.Query<DbSecurityUser>(query).Select(o => o.UserName).ToArray();
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error finding users for role {0} : {1}", role, e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns></returns>
        public string[] GetAllRoles()
        {
            using (var dataContext = this.m_configuration.Provider.GetReadonlyConnection())
                try
                {
                    dataContext.Open();
                    return dataContext.Query<DbSecurityRole>(o => o.ObsoletionTime != null).Select(o => o.Name).ToArray();
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error executing GetAllRoles() : {0}", e);
                    throw;
                }
        }

        /// <summary>
        /// Get all rolesfor user
        /// </summary>
        /// <returns></returns>
        public string[] GetAllRoles(String userName)
        {
            using (var dataContext = this.m_configuration.Provider.GetReadonlyConnection())
            {
                try
                {
                    dataContext.Open();
                    var securityUser = dataContext.SingleOrDefault<DbSecurityUser>(u => u.UserName.ToLower() == userName.ToLower());
                    if (securityUser == null)
                        throw new KeyNotFoundException(String.Format("User {0} not found", userName));

                    var query = dataContext.CreateSqlStatement<DbSecurityUserRole>().SelectFrom()
                        .InnerJoin<DbSecurityRole, DbSecurityUserRole>()
                        .Where(o => o.UserKey == securityUser.Key);

                    return dataContext.Query<DbSecurityRole>(query).Select(o => o.Name).ToArray();
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error executing getting roles for {0} : {1}", userName, e);
                    throw;
                }
            }
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
            using (var dataContext = this.m_configuration.Provider.GetReadonlyConnection())
            {
                try
                {
                    dataContext.Open();
                    DbSecurityUser user = dataContext.SingleOrDefault<DbSecurityUser>(u => u.UserName.ToLower() == userName.ToLower());
                    if (user == null)
                        throw new KeyNotFoundException(String.Format("Could not locate user {0}", userName));
                    DbSecurityRole role = dataContext.SingleOrDefault<DbSecurityRole>(r => r.Name == roleName);
                    if (role == null)
                        throw new KeyNotFoundException(String.Format("Could not locate role {0}", roleName));

                    // Select
                    return dataContext.Any<DbSecurityUserRole>(o => o.UserKey == user.Key && o.RoleKey == role.Key);
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error determining role membership of user {0} in {1} : {2}", userName, roleName, e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Remove users from roles
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
        public void RemoveUsersFromRoles(string[] users, string[] roles, IPrincipal authPrincipal)
        {
            this.VerifyPrincipal(authPrincipal, PermissionPolicyIdentifiers.AlterRoles);

            using (DataContext dataContext = this.m_configuration.Provider.GetWriteConnection())
                try
                {
                    dataContext.Open();
                    using (var tx = dataContext.BeginTransaction())
                    {
                        try
                        {
                            foreach (var un in users)
                            {
                                DbSecurityUser user = dataContext.SingleOrDefault<DbSecurityUser>(u => u.UserName.ToLower() == un.ToLower());
                                if (user == null)
                                    throw new KeyNotFoundException(String.Format("Could not locate user {0}", un));
                                foreach (var rol in roles)
                                {
                                    DbSecurityRole role = dataContext.SingleOrDefault<DbSecurityRole>(r => r.Name == rol);
                                    if (role == null)
                                        throw new KeyNotFoundException(String.Format("Could not locate role {0}", rol));

                                    // Insert
                                    dataContext.Delete<DbSecurityUserRole>(o => o.UserKey == user.Key && o.RoleKey == role.Key);
                                }
                            }
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
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error removing {0} from {1} : {2}", String.Join(",", users), String.Join(",", roles), e);
                    throw;
                }
        }
    }
}
