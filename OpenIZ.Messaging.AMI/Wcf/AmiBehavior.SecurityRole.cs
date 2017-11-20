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
 * User: khannan
 * Date: 2016-11-30
 */

using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Services;
using System;
using System.Data;
using System.Linq;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.AMI.Wcf
{
	/// <summary>
	/// Represents the administrative contract interface.
	/// </summary>
	public partial class AmiBehavior
	{
		/// <summary>
		/// Creates a security role.
		/// </summary>
		/// <param name="role">The security role to be created.</param>
		/// <returns>Returns the newly created security role.</returns>
		public SecurityRoleInfo CreateRole(SecurityRoleInfo role)
		{
			var roleRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			var roleToCreate = new SecurityRole
			{
				Name = role.Name,
				Description = role.Role.Description
			};

            if (role.Policies != null)
                roleToCreate.Policies.AddRange(role.Policies.Select(p => new SecurityPolicyInstance(p.Policy, p.Grant)));
            else
			    roleToCreate.Policies.AddRange(role.Role.Policies.Select(p => new SecurityPolicyInstance(p.Policy, p.GrantType)));

			return new SecurityRoleInfo(roleRepository.CreateRole(roleToCreate));
		}

		/// <summary>
		/// Deletes a security role.
		/// </summary>
		/// <param name="rawRoleId">The id of the role to be deleted.</param>
		/// <returns>Returns the deleted role.</returns>
		public SecurityRoleInfo DeleteRole(string rawRoleId)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(rawRoleId, out key))
			{
				throw new ArgumentException($"{nameof(rawRoleId)} must be a valid GUID");
			}

			var securityRepositoryService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			return new SecurityRoleInfo(securityRepositoryService.ObsoleteRole(key));
		}

		/// <summary>
		/// Gets a specific security role.
		/// </summary>
		/// <param name="roleId">The id of the security role to be retrieved.</param>
		/// <returns>Returns the security role.</returns>
		public SecurityRoleInfo GetRole(string rawRoleId)
		{
			Guid roleId = Guid.Empty;
			if (!Guid.TryParse(rawRoleId, out roleId))
				throw new ArgumentException(nameof(rawRoleId));
			var roleRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			return new SecurityRoleInfo(roleRepository.GetRole(roleId));
		}

		/// <summary>
		/// Gets a list of security roles.
		/// </summary>
		/// <returns>Returns a list of security roles.</returns>
		public AmiCollection<SecurityRoleInfo> GetRoles()
		{
			var expression = QueryExpressionParser.BuildLinqExpression<SecurityRole>(this.CreateQuery(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters));
			var userRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			return new AmiCollection<SecurityRoleInfo> { CollectionItem = userRepository.FindRoles(expression).Select(o => new SecurityRoleInfo(o)).ToList() };
		}

		/// <summary>
		/// Updates a role.
		/// </summary>
		/// <param name="roleId">The id of the role to be updated.</param>
		/// <param name="roleInfo">The role containing the updated information.</param>
		/// <returns>Returns the updated role.</returns>
		public SecurityRoleInfo UpdateRole(string roleId, SecurityRoleInfo roleInfo)
		{
			Guid id = Guid.Empty;

			if (!Guid.TryParse(roleId, out id))
			{
				throw new ArgumentException($"{nameof(roleId)} must be a valid GUID");
			}

			if (id != roleInfo.Id)
			{
				throw new ArgumentException($"Unable to update role using id: {id}, and id: {roleInfo.Id}");
			}

			var roleRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (roleRepository == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			roleInfo.Role?.Policies?.AddRange(roleInfo.Policies.Select(r => new SecurityPolicyInstance(r.Policy, r.Grant)));

			var updatedRole = roleRepository.SaveRole(roleInfo.Role);

			return new SecurityRoleInfo(updatedRole);
		}
	}
}