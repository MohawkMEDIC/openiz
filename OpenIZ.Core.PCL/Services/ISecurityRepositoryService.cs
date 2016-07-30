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
 * Date: 2016-7-18
 */
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Security repository service is responsible for the maintenance of security entities
    /// </summary>
    public interface ISecurityRepositoryService
    {
		/// <summary>
		/// Changes a user's password.
		/// </summary>
		/// <param name="userId">The id of the user.</param>
		/// <param name="password">The new password of the user.</param>
		/// <returns>Returns the updated user.</returns>
		SecurityUser ChangePassword(Guid userId, String password);

		/// <summary>
		/// Creates a device.
		/// </summary>
		/// <param name="device">The security device.</param>
		/// <returns>Returns the newly created device.</returns>
		SecurityDevice CreateDevice(SecurityDevice device);

		/// <summary>
		/// Creates a role.
		/// </summary>
		/// <param name="roleInfo">The security role.</param>
		/// <returns>Returns the newly created security role.</returns>
		SecurityRole CreateRole(SecurityRole roleInfo);

		/// <summary>
		/// Creates a user with a specified password.
		/// </summary>
		/// <param name="userInfo">The security user.</param>
		/// <param name="password">The password.</param>
		/// <returns>Returns the newly created user.</returns>
		SecurityUser CreateUser(SecurityUser userInfo, String password);

		/// <summary>
		/// Gets a list of devices based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the devices.</param>
		/// <returns>Returns a list of devices.</returns>
		IEnumerable<SecurityDevice> FindDevices(Expression<Func<SecurityDevice, bool>> query);

		/// <summary>
		/// Gets a list of devices based on a query.
		/// </summary>
		/// <param name="query">The filter to use to match the devices.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The number of devices.</param>
		/// <param name="totalResults">The total number of devices.</param>
		/// <returns>Returns a list of devices.</returns>
		IEnumerable<SecurityDevice> FindDevices(Expression<Func<SecurityDevice, bool>> query, int offset, int? count, out int totalResults);

		/// <summary>
		/// Gets a list of policies based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the policies.</param>
		/// <returns>Returns a list of policies.</returns>
		IEnumerable<SecurityPolicy> FindPolicies(Expression<Func<SecurityPolicy, bool>> query);

		/// <summary>
		/// Gets a list of policies based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the policies.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The number of policies.</param>
		/// <param name="totalResults">The total number of policies.</param>
		/// <returns>Returns a list of policies.</returns>
		IEnumerable<SecurityPolicy> FindPolicies(Expression<Func<SecurityPolicy, bool>> query, int offset, int? count, out int totalResults);

		/// <summary>
		/// Gets a list of roles based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the roles.</param>
		/// <returns>Returns a list of roles.</returns>
		IEnumerable<SecurityRole> FindRoles(Expression<Func<SecurityRole, bool>> query);

		/// <summary>
		/// Gets a list of roles based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the roles.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The number of roles.</param>
		/// <param name="totalResults">The total number of roles.</param>
		/// <returns>Returns a list of roles.</returns>
		IEnumerable<SecurityRole> FindRoles(Expression<Func<SecurityRole, bool>> query, int offset, int? count, out int totalResults);
        /// <summary>
        /// Creates the specified user entity
        /// </summary>
        /// <returns></returns>
        UserEntity CreateUserEntity(UserEntity userEntity);

        /// <summary>
        /// Gets a list of users based on a query.
        /// </summary>
        /// <param name="query">The query to use to match the users.</param>
        /// <returns>Returns a list of users.</returns>
        IEnumerable<SecurityUser> FindUsers(Expression<Func<SecurityUser, bool>> query);

		/// <summary>
		/// Gets a list of users based on a query.
		/// </summary>
		/// <param name="query">The query to use to match the users.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The number of users.</param>
		/// <param name="totalResults">The total number of users.</param>
		/// <returns>Returns a list of roles.</returns>
		IEnumerable<SecurityUser> FindUsers(Expression<Func<SecurityUser, bool>> query, int offset, int? count, out int totalResults);

		/// <summary>
		/// Gets a specific device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be retrieved.</param>
		/// <returns>Returns the device.</returns>
		SecurityDevice GetDevice(Guid deviceId);
        /// <summary>
        /// Gets the specified user entity
        /// </summary>
        UserEntity GetUserEntity(Guid id, Guid versionId);

        /// <summary>
        /// Gets a specific role.
        /// </summary>
        /// <param name="roleId">The id of the role to retrieve.</param>
        /// <returns>Returns the role.</returns>
        SecurityRole GetRole(Guid roleId);
      
        /// <summary>
        /// Obsoletes the specfied user entity
        /// </summary>
        UserEntity ObsoleteUserEntity(Guid id);


        /// <summary>
        /// Gets a specific user.
        /// </summary>
        /// <param name="userId">The id of the user to retrieve.</param>
        /// <returns>Returns the user.</returns>
        SecurityUser GetUser(Guid userId);

		/// <summary>
		/// Gets the specified security user based on the principal
		/// </summary>
		SecurityUser GetUser(IIdentity identity);

		/// <summary>
		/// Locks a specific user.
		/// </summary>
		/// <param name="userId">The id of the user to lock.</param>
		void LockUser(Guid userId);

		/// <summary>
		/// Obsoletes a device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be obsoleted.</param>
		/// <returns>Returns the obsoleted device.</returns>
		SecurityDevice ObsoleteDevice(Guid deviceId);

		/// <summary>
		/// Obsoletes a role.
		/// </summary>
		/// <param name="roleId">The id of the role to be obsoleted.</param>
		/// <returns>Returns the obsoleted role.</returns>
		SecurityRole ObsoleteRole(Guid roleId);

		/// <summary>
		/// Obsoletes a user.
		/// </summary>
		/// <param name="userId">The id of the user to be obsoleted.</param>
		/// <returns>Returns the obsoleted user.</returns>
		SecurityUser ObsoleteUser(Guid userId);

        /// <summary>
        /// Finds the specified user entity 
        /// </summary>
        IEnumerable<UserEntity> FindUserEntity(Expression<Func<UserEntity, bool>> expression);

        /// <summary>
        /// Finds the specified user entity 
        /// </summary>
        IEnumerable<UserEntity> FindUserEntity(Expression<Func<UserEntity, bool>> expression, int offset, int? count, out int totalCount);

        /// <summary>
        /// Updates a security device.
        /// </summary>
        /// <param name="device">The security device containing the updated information.</param>
        /// <returns>Returns the updated device.</returns>
        SecurityDevice SaveDevice(SecurityDevice device);

		/// <summary>
		/// Updates a security role.
		/// </summary>
		/// <param name="role">The security role containing the updated information.</param>
		/// <returns>Returns the updated role.</returns>
		SecurityRole SaveRole(SecurityRole role);
        /// <summary>
        /// Saves (inserts or updates) the specified user entity
        /// </summary>
        UserEntity SaveUserEntity(UserEntity userEntity);

        /// <summary>
        /// Updates a security user.
        /// </summary>
        /// <param name="user">The security user containing the updated information.</param>
        /// <returns>Returns the updated user.</returns>
        SecurityUser SaveUser(SecurityUser user);

		/// <summary>
		/// Unlocks a specific user.
		/// </summary>
		/// <param name="userId">The id of the user to be unlocked.</param>
		void UnlockUser(Guid userId);
	}
}
