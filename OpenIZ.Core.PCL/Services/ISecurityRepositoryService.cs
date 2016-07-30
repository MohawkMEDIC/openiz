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
        /// Create the specified user with specified password
        /// </summary>
        SecurityUser CreateUser(SecurityUser userInfo, String password);

        /// <summary>
        /// Obsolete a user
        /// </summary>
        SecurityUser ObsoleteUser(Guid userId);

        /// <summary>
        /// Change user's password
        /// </summary>
        SecurityUser ChangePassword(Guid userId, String password);

        /// <summary>
        /// Lock a user
        /// </summary>
        void LockUser(Guid userId);

        /// <summary>
        /// Unlock a user
        /// </summary>
        void UnlockUser(Guid userId);

        /// <summary>
        /// Gets the specified security user based on the principal
        /// </summary>
        SecurityUser GetUser(IIdentity identity);

        /// <summary>
        /// Get the specified user
        /// </summary>
        SecurityUser GetUser(Guid userId);

        /// <summary>
        /// Creates the specified user entity
        /// </summary>
        /// <returns></returns>
        UserEntity CreateUserEntity(UserEntity userEntity);

        /// <summary>
        /// Finds the specified users
        /// </summary>
        IEnumerable<SecurityUser> FindUsers(Expression<Func<SecurityUser, bool>> query);

        /// <summary>
        /// Finds the specified users matching the query 
        /// </summary>
        IEnumerable<SecurityUser> FindUsers(Expression<Func<SecurityUser, bool>> query, int offset, int? count, out int total);


        /// <summary>
        /// Gets the specified user entity
        /// </summary>
        UserEntity GetUserEntity(Guid id, Guid versionId);

        /// <summary>
        /// Save the specified security user
        /// </summary>
        SecurityUser SaveUser(SecurityUser user);

        /// <summary>
        /// Obsoletes the specfied user entity
        /// </summary>
        UserEntity ObsoleteUserEntity(Guid id);


        /// <summary>
        /// Create the specified Role with specified password
        /// </summary>
        SecurityRole CreateRole(SecurityRole roleInfo);

        /// <summary>
        /// Finds the specified user entity 
        /// </summary>
        IEnumerable<UserEntity> FindUserEntity(Expression<Func<UserEntity, bool>> expression);

        /// <summary>
        /// Obsolete a Role
        /// </summary>
        SecurityRole ObsoleteRole(Guid roleId);

        /// <summary>
        /// Get the specified Role
        /// </summary>
        SecurityRole GetRole(Guid roleId);

        /// <summary>
        /// Finds the specified user entity with the specified query restrictions
        /// </summary>
        IEnumerable<UserEntity> FindUserEntity(Expression<Func<UserEntity, bool>> expression, int offset, int count, out int totalCount);

        /// <summary>
        /// Finds the specified Roles
        /// </summary>
        IEnumerable<SecurityRole> FindRoles(Expression<Func<SecurityRole, bool>> query);

        /// <summary>
        /// Finds the specified Roles matching the query 
        /// </summary>
        IEnumerable<SecurityRole> FindRoles(Expression<Func<SecurityRole, bool>> query, int offset, int? count, out int total);

        /// <summary>
        /// Save the specified security Role
        /// </summary>
        SecurityRole SaveRole(SecurityRole role);

        /// <summary>
        /// Saves (inserts or updates) the specified user entity
        /// </summary>
        UserEntity SaveUserEntity(UserEntity userEntity);

        /// <summary>
        /// Gets the specified policies
        /// </summary>
        IEnumerable<SecurityPolicy> FindPolicies(Expression<Func<SecurityPolicy, bool>> filter);

        /// <summary>
        /// Find the specified policies
        /// </summary>
        IEnumerable<SecurityPolicy> FindPolicies(Expression<Func<SecurityPolicy, bool>> filter, int offset, int? count, out int totalResults);
    }
}
