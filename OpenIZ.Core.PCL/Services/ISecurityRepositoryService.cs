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

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Security repository service is responsible for the maintenance of security entities
    /// </summary>
    public interface ISecurityRepositoryService
    {
		/// <summary>
		/// Change user's password
		/// </summary>
		SecurityUser ChangePassword(Guid userId, String password);

		/// <summary>
		/// Create the specified Role with specified password
		/// </summary>
		SecurityRole CreateRole(SecurityRole roleInfo);

		/// <summary>
		/// Create the specified user with specified password
		/// </summary>
		SecurityUser CreateUser(SecurityUser userInfo, String password);

		/// <summary>
		/// Gets a list of devices based on a filter.
		/// </summary>
		/// <param name="filter">The filter to use to match the devices.</param>
		/// <returns>Returns a list of devices.</returns>
		IEnumerable<SecurityDevice> FindDevices(Expression<Func<SecurityDevice, bool>> filter);

		/// <summary>
		/// Gets a list of devices based on a filter.
		/// </summary>
		/// <param name="filter">The filter to use to match the devices.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The number of devices.</param>
		/// <param name="totalResults">The total number of devices.</param>
		/// <returns>Returns a list of devices.</returns>
		IEnumerable<SecurityDevice> FindDevices(Expression<Func<SecurityDevice, bool>> filter, int offset, int? count, out int totalResults);

		/// <summary>
		/// Gets the specified policies
		/// </summary>
		IEnumerable<SecurityPolicy> FindPolicies(Expression<Func<SecurityPolicy, bool>> filter);

		/// <summary>
		/// Find the specified policies
		/// </summary>
		IEnumerable<SecurityPolicy> FindPolicies(Expression<Func<SecurityPolicy, bool>> filter, int offset, int? count, out int totalResults);

		/// <summary>
		/// Finds the specified Roles
		/// </summary>
		IEnumerable<SecurityRole> FindRoles(Expression<Func<SecurityRole, bool>> query);

		/// <summary>
		/// Finds the specified Roles matching the query 
		/// </summary>
		IEnumerable<SecurityRole> FindRoles(Expression<Func<SecurityRole, bool>> query, int offset, int? count, out int total);

		/// <summary>
		/// Finds the specified users
		/// </summary>
		IEnumerable<SecurityUser> FindUsers(Expression<Func<SecurityUser, bool>> query);

		/// <summary>
		/// Finds the specified users matching the query 
		/// </summary>
		IEnumerable<SecurityUser> FindUsers(Expression<Func<SecurityUser, bool>> query, int offset, int? count, out int total);

		/// <summary>
		/// Get the specified Role
		/// </summary>
		SecurityRole GetRole(Guid roleId);

		/// <summary>
		/// Get the specified user
		/// </summary>
		SecurityUser GetUser(Guid userId);

		/// <summary>
		/// Gets the specified security user based on the principal
		/// </summary>
		SecurityUser GetUser(IIdentity identity);

		/// <summary>
		/// Lock a user
		/// </summary>
		void LockUser(Guid userId);

		/// <summary>
		/// Obsolete a Role
		/// </summary>
		SecurityRole ObsoleteRole(Guid roleId);

		/// <summary>
		/// Obsolete a user
		/// </summary>
		SecurityUser ObsoleteUser(Guid userId);

		/// <summary>
		/// Save the specified security Role
		/// </summary>
		SecurityRole SaveRole(SecurityRole role);

		/// <summary>
		/// Save the specified security user
		/// </summary>
		SecurityUser SaveUser(SecurityUser user);

		/// <summary>
		/// Unlock a user
		/// </summary>
		void UnlockUser(Guid userId);

	}
}
