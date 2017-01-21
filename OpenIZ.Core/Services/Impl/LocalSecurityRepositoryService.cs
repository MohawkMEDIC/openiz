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
 * Date: 2016-8-2
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents a security repository service that uses the direct local services
	/// </summary>
	public class LocalSecurityRepositoryService : ISecurityRepositoryService
	{
		private TraceSource m_traceSource = new TraceSource(OpenIzConstants.ServiceTraceSourceName);

		/// <summary>
		/// Changes a user's password.
		/// </summary>
		/// <param name="userId">The id of the user.</param>
		/// <param name="password">The new password of the user.</param>
		/// <returns>Returns the updated user.</returns>
		public SecurityUser ChangePassword(Guid userId, string password)
		{
			this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Changing user password");
			var securityUser = this.GetUser(userId);
			if (securityUser == null)
				throw new KeyNotFoundException("Cannot locate security user");
			var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
			if (iids == null) throw new InvalidOperationException("Cannot find identity provider service");
			iids.ChangePassword(securityUser.UserName, password, AuthenticationContext.Current.Principal);
			return securityUser;
		}

		/// <summary>
		/// Creates a security application.
		/// </summary>
		/// <param name="application">The security application.</param>
		/// <returns>Returns the newly created application.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.CreateApplication)]
		public SecurityApplication CreateApplication(SecurityApplication application)
		{
			this.m_traceSource.TraceEvent(TraceEventType.Information, 0, "Creating application {0}", application);

			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityApplication>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<SecurityApplication>)} not found");
			}

			application.ApplicationSecret = this.Encode(application.ApplicationSecret);

			var createdApplication = persistenceService.Insert(application, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			var applicationEntityPersistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ApplicationEntity>>();

			if (applicationEntityPersistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ApplicationEntity>)} not found");
			}

			applicationEntityPersistenceService.Insert(new ApplicationEntity
			{
				SecurityApplication = createdApplication,
				SoftwareName = application.Name,
				StatusConceptKey = StatusKeys.Active
			}, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			return createdApplication;
		}

		/// <summary>
		/// Creates a device.
		/// </summary>
		/// <param name="device">The security device.</param>
		/// <returns>Returns the newly created device.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.CreateDevice)]
		public SecurityDevice CreateDevice(SecurityDevice device)
		{
			this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Creating device {0}", device);

			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityDevice>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<SecurityDevice>)} not found");
			}

			device.DeviceSecret = this.Encode(device.DeviceSecret);

			var createdDevice = persistenceService.Insert(device, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			var deviceEntityPersistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<DeviceEntity>>();

			if (deviceEntityPersistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<DeviceEntity>)} not found");
			}

			deviceEntityPersistenceService.Insert(new DeviceEntity
			{
				ManufacturerModelName = device.Name,
				SecurityDevice = createdDevice,
				StatusConceptKey = StatusKeys.Active
			}, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			return createdDevice;
		}

		/// <summary>
		/// Creates a security policy.
		/// </summary>
		/// <param name="policy">The security policy.</param>
		/// <returns>Returns the newly created policy.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterPolicy)]
		public SecurityPolicy CreatePolicy(SecurityPolicy policy)
		{
			this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Creating policy {0}", policy);

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
			this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Creating role {0}", roleInfo);

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
			this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Creating user {0}", userInfo);

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
                retVal.UserClass = userInfo.UserClass;
				pers.Update(retVal, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}

			this.CreateUserEntity(new UserEntity
			{
				SecurityUser = retVal
			});

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
		/// Gets a list of applications based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the application.</param>
		/// <returns>Returns a list of applications.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
		public IEnumerable<SecurityApplication> FindApplications(Expression<Func<SecurityApplication, bool>> query)
		{
			int totalCount = 0;
			return this.FindApplications(query, 0, null, out totalCount);
		}

		/// <summary>
		/// Gets a list of applications based on a query.
		/// </summary>
		/// <param name="query">The filter to use to match the applications.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The number of applications.</param>
		/// <param name="totalResults">The total number of applications.</param>
		/// <returns>Returns a list of applications.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
		public IEnumerable<SecurityApplication> FindApplications(Expression<Func<SecurityApplication, bool>> query, int offset, int? count, out int totalResults)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityApplication>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<SecurityApplication>)} not found");
			}

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
		}

		/// <summary>
		/// Gets a list of devices based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the devices.</param>
		/// <returns>Returns a list of devices.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
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
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.Login)]
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
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.Login)]
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
		/// Gets a specific application.
		/// </summary>
		/// <param name="applicationId">The id of the application to be retrieved.</param>
		/// <returns>Returns a application.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
		public SecurityApplication GetApplication(Guid applicationId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityApplication>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<SecurityApplication>)} not found");
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(applicationId), AuthenticationContext.Current.Principal, false);
		}

		/// <summary>
		/// Gets a specific device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be retrieved.</param>
		/// <returns>Returns the device.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
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
		/// Gets a specific policy.
		/// </summary>
		/// <param name="policyId">The id of the policy to be retrieved.</param>
		/// <returns>Returns the policy.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
		public SecurityPolicy GetPolicy(Guid policyId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityPolicy>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityPolicy>)));
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(policyId), AuthenticationContext.Current.Principal, false);
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
        /// Gets a specific user.
        /// </summary>
        /// <param name="userId">The id of the user to retrieve.</param>
        /// <returns>Returns the user.</returns>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public SecurityUser GetUser(String userName)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityUser>)));
            }

            return persistenceService.Query(u => u.UserName == userName, AuthenticationContext.Current.Principal).FirstOrDefault();
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
			return pers.Query(o => o.UserName == identity.Name && o.ObsoletionTime == null, AuthenticationContext.Current.Principal).FirstOrDefault();
		}

		/// <summary>
		/// Get user entity from identity
		/// </summary>
		public UserEntity GetUserEntity(IIdentity identity)
		{
			var pers = ApplicationContext.Current.GetService<IDataPersistenceService<UserEntity>>();
			if (pers == null)
				throw new InvalidOperationException("Missing persistence service");
			int t = 0;
			return pers.Query(o => o.SecurityUser.UserName == identity.Name, 0, 1, AuthenticationContext.Current.Principal, out t).FirstOrDefault();
		}

		/// <summary>
		/// Encodes a string using SHA-256.
		/// </summary>
		/// <param name="value">The string to be encoded.</param>
		/// <returns>Returns the encoded string.</returns>
		private string Encode(string value)
		{
			var hasher = SHA256.Create();
			return BitConverter.ToString(hasher.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", "").ToLower();
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
			this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Locking user {0}", userId);

			var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
			if (iids == null)
				throw new InvalidOperationException("Missing identity provider service");

			var securityUser = this.GetUser(userId);
			iids.SetLockout(securityUser.UserName, true, AuthenticationContext.Current.Principal);
		}

		/// <summary>
		/// Obsoletes an application.
		/// </summary>
		/// <param name="applicationId">The id of the application to be obsoleted.</param>
		/// <returns>Returns the obsoleted application.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
		public SecurityApplication ObsoleteApplication(Guid applicationId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityApplication>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<SecurityApplication>)} not found");
			}

			return persistenceService.Obsolete(this.GetApplication(applicationId), AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsoletes a device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be obsoleted.</param>
		/// <returns>Returns the obsoleted device.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
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
		/// Obsoletes a policy.
		/// </summary>
		/// <param name="policyId">THe id of the policy to be obsoleted.</param>
		/// <returns>Returns the obsoleted policy.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterPolicy)]
		public SecurityPolicy ObsoletePolicy(Guid policyId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityPolicy>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityPolicy>)));
			}

			return persistenceService.Obsolete(this.GetPolicy(policyId), AuthenticationContext.Current.Principal, TransactionMode.Commit);
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
		/// Updates a security application.
		/// </summary>
		/// <param name="application">The security application containing the updated information.</param>
		/// <returns>Returns the updated application.</returns>
		public SecurityApplication SaveApplication(SecurityApplication application)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityApplication>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<SecurityApplication>)} not found");
			}

			return persistenceService.Update(application, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Updates a security device.
		/// </summary>
		/// <param name="device">The security device containing the updated information.</param>
		/// <returns>Returns the updated device.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedMetadata)]
		public SecurityDevice SaveDevice(SecurityDevice device)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityDevice>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<SecurityDevice>)} not found");
			}

			return persistenceService.Update(device, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Updates a security policy.
		/// </summary>
		/// <param name="policy">The security policy containing the updated information.</param>
		/// <returns>Returns the updated policy.</returns>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedMetadata)]
		public SecurityPolicy SavePolicy(SecurityPolicy policy)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityPolicy>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<SecurityPolicy>)));
			}

			return persistenceService.Update(policy, AuthenticationContext.Current.Principal, TransactionMode.Commit);
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
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AlterIdentity)]
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
			this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Unlocking user {0}", userId);

			var iids = ApplicationContext.Current.GetService<IIdentityProviderService>();
			if (iids == null)
				throw new InvalidOperationException("Missing identity provider service");

			var securityUser = this.GetUser(userId);
			iids.SetLockout(securityUser.UserName, false, AuthenticationContext.Current.Principal);
		}
	}
}