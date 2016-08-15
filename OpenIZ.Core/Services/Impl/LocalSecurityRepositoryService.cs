﻿/*
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
 * Date: 2016-6-22
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Security;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Represents a security repository service that uses the direct local services
    /// </summary>
    public class LocalSecurityRepositoryService : ISecurityRepositoryService
    {
		/// <summary>
		/// Changes a user's password.
		/// </summary>
		/// <param name="userId">The id of the user.</param>
		/// <param name="password">The new password of the user.</param>
		/// <returns>Returns the updated user.</returns>
		public SecurityUser ChangePassword(Guid userId, string password)
        {
            var securityUser = this.GetUser(userId);
            var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
            iids.ChangePassword(securityUser.UserName, password, AuthenticationContext.Current.Principal);
            return securityUser;
        }

		/// <summary>
		/// Creates a device.
		/// </summary>
		/// <param name="device">The security device.</param>
		/// <returns>Returns the newly created device.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.CreateDevice)]
		public SecurityDevice CreateDevice(SecurityDevice device)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityDevice>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityDevice>)));
			}

			return persistenceService.Insert(device, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Creates a security policy.
		/// </summary>
		/// <param name="policy">The security policy.</param>
		/// <returns>Returns the newly created policy.</returns>
		public SecurityPolicy CreatePolicy(SecurityPolicy policy)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityPolicy>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityPolicy>)));
			}

			return persistenceService.Insert(policy, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Creates a role.
		/// </summary>
		/// <param name="roleInfo">The security role.</param>
		/// <returns>Returns the newly created security role.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.CreateRoles)]
        public SecurityRole CreateRole(SecurityRole roleInfo)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();
            if (pers == null)
                throw new InvalidOperationException("Misisng role provider service");
            return pers.Insert(roleInfo, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

		/// <summary>
		/// Creates a user with a specified password.
		/// </summary>
		/// <param name="userInfo">The security user.</param>
		/// <param name="password">The password.</param>
		/// <returns>Returns the newly created user.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.CreateIdentity)]
        public SecurityUser CreateUser(SecurityUser userInfo, string password)
        {
            var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();

            if (pers == null)
                throw new InvalidOperationException("Missing persistence service");
            else if (iids == null)
                throw new InvalidOperationException("Missing identity provider service");

            // Create the identity
            var id = iids.CreateIdentity(userInfo.UserName, password, AuthenticationContext.Current.Principal);
            // Now ensure local db record exists
            var retVal = this.GetUser(id);
            if (retVal == null)
                retVal = pers.Insert(userInfo, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            else
            {
                retVal.Email = userInfo.Email;
                retVal.EmailConfirmed = userInfo.EmailConfirmed;
                retVal.InvalidLoginAttempts = userInfo.InvalidLoginAttempts;
                retVal.LastLoginTime = userInfo.LastLoginTime;
				retVal.Lockout = userInfo.Lockout;
                retVal.PhoneNumber = userInfo.PhoneNumber;
                retVal.PhoneNumberConfirmed = userInfo.PhoneNumberConfirmed;
                retVal.SecurityHash = userInfo.SecurityHash;
                retVal.TwoFactorEnabled = userInfo.TwoFactorEnabled;
                retVal.UserPhoto = userInfo.UserPhoto;
                pers.Update(retVal, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
            return retVal;
        }
        /// <summary>
        /// Creates the specified user entity
        /// </summary>
        public UserEntity CreateUserEntity(UserEntity userEntity)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            if (persistence == null)
                throw new InvalidOperationException("Persistence service missing");
            return persistence.Insert(userEntity, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Gets a list of devices based on a query.
        /// </summary>
        /// <param name="query">The query to use to match the devices.</param>
        /// <returns>Returns a list of devices.</returns>
        public IEnumerable<SecurityDevice> FindDevices(Expression<Func<SecurityDevice, bool>> query)
		{
			int totalCount = 0;
			return this.FindDevices(query, 0, null, out totalCount);
		}

		/// <summary>
		/// Gets a list of devices based on a query.
		/// </summary>
		/// <param name="query">The filter to use to match the devices.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The number of devices.</param>
		/// <param name="totalResults">The total number of devices.</param>
		/// <returns>Returns a list of devices.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
		public IEnumerable<SecurityDevice> FindDevices(Expression<Func<SecurityDevice, bool>> query, int offset, int? count, out int totalResults)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityDevice>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityDevice>)));
			}

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
		}

		/// <summary>
		/// Gets a list of policies based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the policies.</param>
		/// <returns>Returns a list of policies.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<SecurityPolicy> FindPolicies(Expression<Func<SecurityPolicy, bool>> query)
        {
            int totalResults = 0;
            return this.FindPolicies(query, 0, null, out totalResults);
        }

		/// <summary>
		/// Gets a list of policies based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the policies.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The number of policies.</param>
		/// <param name="totalResults">The total number of policies.</param>
		/// <returns>Returns a list of policies.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<SecurityPolicy> FindPolicies(Expression<Func<SecurityPolicy, bool>> query, int offset, int? count, out int totalResults)
        {
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityPolicy>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityPolicy>)));
			}

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
		}

		/// <summary>
		/// Gets a list of roles based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the roles.</param>
		/// <returns>Returns a list of roles.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<SecurityRole> FindRoles(Expression<Func<SecurityRole, bool>> query)
        {
			int totalResults = 0;
			return this.FindRoles(query, 0, null, out totalResults);
		}

		/// <summary>
		/// Gets a list of roles based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the roles.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The number of roles.</param>
		/// <param name="totalResults">The total number of roles.</param>
		/// <returns>Returns a list of roles.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<SecurityRole> FindRoles(Expression<Func<SecurityRole, bool>> query, int offset, int? count, out int totalResults)
        {
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityRole>)));
			}

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
		}

        /// <summary>
        /// Find the specified user entity data
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IEnumerable<UserEntity> FindUserEntity(Expression<Func<UserEntity, bool>> expression)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            if (persistence == null)
                throw new InvalidOperationException("Persistence service missing");
            return persistence.Query(expression, AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Find the specified user entity with constraints
        /// </summary>
        public IEnumerable<UserEntity> FindUserEntity(Expression<Func<UserEntity, bool>> expression, int offset, int? count, out int totalCount)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            if (persistence == null)
                throw new InvalidOperationException("Persistence service missing");
            return persistence.Query(expression, offset, count, AuthenticationContext.Current.Principal, out totalCount);
        }

        /// <summary>
        /// Gets a list of users based on a query.
        /// </summary>
        /// <param name="query">The query to use to match the users.</param>
        /// <returns>Returns a list of users.</returns>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
        public IEnumerable<SecurityUser> FindUsers(Expression<Func<SecurityUser, bool>> query)
        {
			int totalResults = 0;
			return this.FindUsers(query, 0, null, out totalResults);
		}

		/// <summary>
		/// Gets a list of users based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the users.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The number of users.</param>
		/// <param name="totalResults">The total number of users.</param>
		/// <returns>Returns a list of roles.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
        public IEnumerable<SecurityUser> FindUsers(Expression<Func<SecurityUser, bool>> query, int offset, int? count, out int totalResults)
        {
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityUser>)));
			}

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
		}

		/// <summary>
		/// Gets a specific device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be retrieved.</param>
		/// <returns>Returns the device.</returns>
		public SecurityDevice GetDevice(Guid deviceId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityDevice>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityDevice>)));
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(deviceId), AuthenticationContext.Current.Principal, false);
		}

		/// <summary>
		/// Gets a specific role.
		/// </summary>
		/// <param name="roleId">The id of the role to retrieve.</param>
		/// <returns>Returns the role.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public SecurityRole GetRole(Guid roleId)
        {
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityRole>)));
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(roleId), AuthenticationContext.Current.Principal, false);
		}

		/// <summary>
		/// Gets a specific user.
		/// </summary>
		/// <param name="userId">The id of the user to retrieve.</param>
		/// <returns>Returns the user.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public SecurityUser GetUser(Guid userId)
        {
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityUser>)));
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(userId), AuthenticationContext.Current.Principal, false);
		}

        /// <summary>
        /// Get the specified user based on identity
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public SecurityUser GetUser(IIdentity identity)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            if (pers == null)
                throw new InvalidOperationException("Missing persistence service");
            return pers.Query(o=>o.UserName == identity.Name && o.ObsoletionTime == null, AuthenticationContext.Current.Principal).FirstOrDefault();
        }

        /// <summary>
        /// Gets the specified user entity
        /// </summary>
        public UserEntity GetUserEntity(Guid id, Guid versionId)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            if (persistence == null)
                throw new InvalidOperationException("Persistence service missing");
            return persistence.Get<Guid>(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
        }

        /// <summary>
        /// Locks a specific user.
        /// </summary>
        /// <param name="userId">The id of the user to lock.</param>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
        public void LockUser(Guid userId)
        {
            var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
            if (iids == null)
                throw new InvalidOperationException("Missing identity provider service");

            var securityUser = this.GetUser(userId);
            iids.SetLockout(securityUser.UserName, true, AuthenticationContext.Current.Principal);
            
        }

		/// <summary>
		/// Obsoletes a device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be obsoleted.</param>
		/// <returns>Returns the obsoleted device.</returns>
		public SecurityDevice ObsoleteDevice(Guid deviceId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityDevice>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityDevice>)));
			}

			return persistenceService.Obsolete(this.GetDevice(deviceId), AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsoletes a role.
		/// </summary>
		/// <param name="roleId">The id of the role to be obsoleted.</param>
		/// <returns>Returns the obsoleted role.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterRoles)]
        public SecurityRole ObsoleteRole(Guid roleId)
        {
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityRole>)));
			}

			return persistenceService.Obsolete(this.GetRole(roleId), AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsoletes a user.
		/// </summary>
		/// <param name="userId">The id of the user to be obsoleted.</param>
		/// <returns>Returns the obsoleted user.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
        public SecurityUser ObsoleteUser(Guid userId)
        {
            var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();

            if (pers == null)
                throw new InvalidOperationException("Missing persistence service");
            else if (iids == null)
                throw new InvalidOperationException("Missing identity provider service");

            var retVal = pers.Obsolete(this.GetUser(userId), AuthenticationContext.Current.Principal, TransactionMode.Commit);
            iids.DeleteIdentity(retVal.UserName, AuthenticationContext.Current.Principal);
            return retVal;
        }
        /// <summary>
        /// Obsoletes the specified user entity
        /// </summary>
        public UserEntity ObsoleteUserEntity(Guid id)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            if (persistence == null)
                throw new InvalidOperationException("Persistence service not found");
            return persistence.Obsolete(new UserEntity() { Key = id }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Updates a security device.
        /// </summary>
        /// <param name="device">The security device containing the updated information.</param>
        /// <returns>Returns the updated device.</returns>
        public SecurityDevice SaveDevice(SecurityDevice device)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityDevice>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityDevice>)));
			}

			return persistenceService.Update(device, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Updates a security role.
		/// </summary>
		/// <param name="role">The security role containing the updated information.</param>
		/// <returns>Returns the updated role.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterRoles)]
        public SecurityRole SaveRole(SecurityRole role)
        {
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityRole>)));
			}

			return persistenceService.Update(role, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Updates a security user.
		/// </summary>
		/// <param name="user">The security user containing the updated information.</param>
		/// <returns>Returns the updated user.</returns>
		public SecurityUser SaveUser(SecurityUser user)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            user.PasswordHash = null; // Don't update the password hash here
            if (pers == null)
                throw new InvalidOperationException("Missing persistence service");

            // Demand permission do this operation
            if (AuthenticationContext.Current.Principal.Identity.Name != user.UserName) // Users can update their own info
                new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PermissionPolicyIdentifiers.AlterIdentity).Demand(); // Otherwise demand to be an administrator
            return pers.Update(user, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Saves the specified user entity
        /// </summary>
        public UserEntity SaveUserEntity(UserEntity userEntity)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
            if (persistence == null)
                throw new InvalidOperationException("Persistence service not found");
            try
            {
                return persistence.Update(userEntity, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
            catch
            {
                return persistence.Insert(userEntity, AuthenticationContext.Current.Principal, TransactionMode.Commit);

            }
        }

        /// <summary>
		/// Unlocks a specific user.
		/// </summary>
		/// <param name="userId">The id of the user to be unlocked.</param>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
        public void UnlockUser(Guid userId)
        {
            var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
            if (iids == null)
                throw new InvalidOperationException("Missing identity provider service");

            var securityUser = this.GetUser(userId);
            iids.SetLockout(securityUser.UserName, false, AuthenticationContext.Current.Principal);
        }
	}
}