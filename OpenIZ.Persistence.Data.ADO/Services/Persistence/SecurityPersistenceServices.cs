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

using OpenIZ.Core.Model.Security;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model.Security;
using OpenIZ.Persistence.Data.ADO.Exceptions;
using System;
using System.Linq;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
	/// <summary>
	/// Security user persistence
	/// </summary>
	public class SecurityApplicationPersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityApplication, DbSecurityApplication>
	{
		/// <summary>
		/// Insert the specified object
		/// </summary>
		public override Core.Model.Security.SecurityApplication Insert(DataContext context, Core.Model.Security.SecurityApplication data, IPrincipal principal)
		{
			var retVal = base.Insert(context, data, principal);

			if (data.Policies == null)
				return retVal;

			data.Policies.ForEach(o => o.Policy?.EnsureExists(context, principal));
			foreach (var itm in data.Policies.Select(o => new DbSecurityApplicationPolicy()
			{
				PolicyKey = o.PolicyKey.Value,
				GrantType = (int)o.GrantType,
				SourceKey = retVal.Key.Value,
				Key = Guid.NewGuid()
			}))
				context.Insert(itm);

			return retVal;
		}

		/// <summary>
		/// Represent as model instance
		/// </summary>
		public override Core.Model.Security.SecurityApplication ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
		{
			var retVal = base.ToModelInstance(dataInstance, context, principal);
			if (retVal == null) return null;
			var policyQuery = new SqlStatement<DbSecurityApplicationPolicy>().SelectFrom()
				.InnerJoin<DbSecurityPolicy>(o => o.PolicyKey, o => o.Key)
				.Where<DbSecurityApplicationPolicy>(o => o.SourceKey == retVal.Key);

			retVal.Policies = context.Query<CompositeResult<DbSecurityApplicationPolicy, DbSecurityPolicy>>(policyQuery).Select(o => new SecurityPolicyInstance(m_mapper.MapDomainInstance<DbSecurityPolicy, SecurityPolicy>(o.Object2), (PolicyGrantType)o.Object1.GrantType)).ToList();
			return retVal;
		}

		/// <summary>
		/// Update the roles to security user
		/// </summary>
		public override Core.Model.Security.SecurityApplication Update(DataContext context, Core.Model.Security.SecurityApplication data, IPrincipal principal)
		{
			data = base.Update(context, data, principal);

			if (data.Policies != null)
			{
				context.Delete<DbSecurityDevicePolicy>(o => o.SourceKey == data.Key);
				foreach (var pol in data.Policies.Select(o => new DbSecurityApplicationPolicy
				{
					PolicyKey = o.PolicyKey.Value,
					GrantType = (int)o.GrantType,
					SourceKey = data.Key.Value,
					Key = Guid.NewGuid()
				}))
					context.Insert(pol);
			}

			return data;
		}
	}

	/// <summary>
	/// Security user persistence
	/// </summary>
	public class SecurityDevicePersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityDevice, DbSecurityDevice>
	{
		/// <summary>
		/// Insert the specified object
		/// </summary>
		public override Core.Model.Security.SecurityDevice Insert(DataContext context, Core.Model.Security.SecurityDevice data, IPrincipal principal)
		{
			var retVal = base.Insert(context, data, principal);

			if (data.Policies == null)
				return retVal;

			data.Policies.ForEach(o => o.Policy?.EnsureExists(context, principal));
			foreach (var itm in data.Policies.Select(o => new DbSecurityDevicePolicy()
			{
				PolicyKey = o.PolicyKey.Value,
				GrantType = (int)o.GrantType,
				SourceKey = retVal.Key.Value,
				Key = Guid.NewGuid()
			}))
				context.Insert(itm);

			return retVal;
		}

		/// <summary>
		/// Represent as model instance
		/// </summary>
		public override Core.Model.Security.SecurityDevice ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
		{
			var retVal = base.ToModelInstance(dataInstance, context, principal);
			if (retVal == null) return null;
			var policyQuery = new SqlStatement<DbSecurityDevicePolicy>().SelectFrom()
				.InnerJoin<DbSecurityPolicy>(o => o.PolicyKey, o => o.Key)
				.Where<DbSecurityDevicePolicy>(o => o.SourceKey == retVal.Key);

			retVal.Policies = context.Query<CompositeResult<DbSecurityDevicePolicy, DbSecurityPolicy>>(policyQuery).Select(o => new SecurityPolicyInstance(m_mapper.MapDomainInstance<DbSecurityPolicy, SecurityPolicy>(o.Object2), (PolicyGrantType)o.Object1.GrantType)).ToList();
			return retVal;
		}

		/// <summary>
		/// Update the roles to security user
		/// </summary>
		public override Core.Model.Security.SecurityDevice Update(DataContext context, Core.Model.Security.SecurityDevice data, IPrincipal principal)
		{
			data = base.Update(context, data, principal);

			if (data.Policies != null)
			{
				context.Delete<DbSecurityDevicePolicy>(o => o.SourceKey == data.Key);
				foreach (var pol in data.Policies.Select(o => new DbSecurityDevicePolicy
				{
					PolicyKey = o.PolicyKey.Value,
					GrantType = (int)o.GrantType,
					SourceKey = data.Key.Value,
					Key = Guid.NewGuid()
				}))
					context.Insert(pol);
			}

			return data;
		}
	}

	/// <summary>
	/// Represents a persistence service for security policies.
	/// </summary>
	public class SecurityPolicyPersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityPolicy, DbSecurityPolicy>
	{
		/// <summary>
		/// Updating policies is a security risk and not permitted... ever
		/// </summary>
		public override Core.Model.Security.SecurityPolicy Update(DataContext context, Core.Model.Security.SecurityPolicy data, IPrincipal principal)
		{
			throw new AdoFormalConstraintException(AdoFormalConstraintType.UpdatedReadonlyObject);
		}
	}

	/// <summary>
	/// Security user persistence
	/// </summary>
	public class SecurityRolePersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityRole, DbSecurityRole>
	{
		/// <summary>
		/// Insert the specified object
		/// </summary>
		public override Core.Model.Security.SecurityRole Insert(DataContext context, Core.Model.Security.SecurityRole data, IPrincipal principal)
		{
			var retVal = base.Insert(context, data, principal);

			if (data.Policies != null)
			{
				// TODO: Clean this up
				data.Policies.ForEach(o => o.Policy?.EnsureExists(context, principal));
				foreach (var pol in data.Policies.Select(o => new DbSecurityRolePolicy()

				{
					PolicyKey = o.PolicyKey.Value,
					GrantType = (int)o.GrantType,
					SourceKey = retVal.Key.Value,
					Key = Guid.NewGuid()
				}))
					context.Insert(pol);
			}

			return retVal;
		}

		/// <summary>
		/// Represent as model instance
		/// </summary>
		public override Core.Model.Security.SecurityRole ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
		{
			var retVal = base.ToModelInstance(dataInstance, context, principal);
			if (retVal == null) return null;

			var policyQuery = new SqlStatement<DbSecurityRolePolicy>().SelectFrom()
				.InnerJoin<DbSecurityPolicy>(o => o.PolicyKey, o => o.Key)
				.Where<DbSecurityRolePolicy>(o => o.SourceKey == retVal.Key);

			retVal.Policies = context.Query<CompositeResult<DbSecurityRolePolicy, DbSecurityPolicy>>(policyQuery).Select(o => new SecurityPolicyInstance(m_mapper.MapDomainInstance<DbSecurityPolicy, SecurityPolicy>(o.Object2), (PolicyGrantType)o.Object1.GrantType)).ToList();

			var rolesQuery = new SqlStatement<DbSecurityUserRole>().SelectFrom()
				.InnerJoin<DbSecurityUser>(o => o.UserKey, o => o.Key)
				.Where<DbSecurityUserRole>(o => o.RoleKey == retVal.Key);

			retVal.Users = context.Query<DbSecurityUser>(rolesQuery).Select(o => m_mapper.MapDomainInstance<DbSecurityUser, Core.Model.Security.SecurityUser>(o)).ToList();

			return retVal;
		}

		/// <summary>
		/// Update the roles to security user
		/// </summary>
		public override Core.Model.Security.SecurityRole Update(DataContext context, Core.Model.Security.SecurityRole data, IPrincipal principal)
		{
			data = base.Update(context, data, principal);

			if (data.Policies != null)
			{
				context.Delete<DbSecurityRolePolicy>(o => o.SourceKey == data.Key);
				foreach (var pol in data.Policies.Select(o => new DbSecurityRolePolicy

				{
					PolicyKey = o.PolicyKey.Value,
					GrantType = (int)o.GrantType,
					SourceKey = data.Key.Value,
					Key = Guid.NewGuid()
				}))
					context.Insert(pol);
			}

			return data;
		}
	}

	/// <summary>
	/// Security user persistence
	/// </summary>
	public class SecurityUserPersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityUser, DbSecurityUser>
	{
		/// <summary>
		/// Gets a security user.
		/// </summary>
		/// <param name="context">The data context.</param>
		/// <param name="key">The key of the user to retrieve.</param>
		/// <param name="principal">The authentication context.</param>
		/// <returns>Returns a security user or null if no user is found.</returns>
		internal override SecurityUser Get(DataContext context, Guid key, IPrincipal principal)
		{
			var user = base.Get(context, key, principal);
            if (user == null) return null;
			var rolesQuery = new SqlStatement<DbSecurityUserRole>().SelectFrom()
				.InnerJoin<DbSecurityRole>(o => o.RoleKey, o => o.Key)
				.Where<DbSecurityUserRole>(o => o.UserKey == key);

			user.Roles = context.Query<DbSecurityRole>(rolesQuery).Select(o => m_mapper.MapDomainInstance<DbSecurityRole, SecurityRole>(o)).ToList();

			return user;
		}

		/// <summary>
		/// Insert the specified object
		/// </summary>
		public override Core.Model.Security.SecurityUser Insert(DataContext context, Core.Model.Security.SecurityUser data, IPrincipal principal)
		{
			var retVal = base.Insert(context, data, principal);

			// Roles
			if (data.Roles != null)
				foreach (var r in data.Roles)
				{
					r.EnsureExists(context, principal);
					context.Insert(new DbSecurityUserRole()
					{
						UserKey = retVal.Key.Value,
						RoleKey = r.Key.Value
					});
				}

			return retVal;
		}

		public override IEnumerable<SecurityUser> Query(DataContext context, Expression<Func<SecurityUser, bool>> query, int offset, int? count, out int totalResults, IPrincipal principal, bool countResults = true)
		{
			var results = base.Query(context, query, offset, count, out totalResults, principal, countResults);

			var users = new List<SecurityUser>();

			foreach (var user in results)
			{
				var rolesQuery = new SqlStatement<DbSecurityUserRole>().SelectFrom()
					.InnerJoin<DbSecurityRole>(o => o.RoleKey, o => o.Key)
					.Where<DbSecurityUserRole>(o => o.UserKey == user.Key);

				user.Roles = context.Query<DbSecurityRole>(rolesQuery).Select(o => m_mapper.MapDomainInstance<DbSecurityRole, SecurityRole>(o)).ToList();

				users.Add(user);
			}

			return users;
		}

		public override Core.Model.Security.SecurityUser ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
		{
			var dbUser = dataInstance as DbSecurityUser;
			var retVal = base.ToModelInstance(dataInstance, context, principal);
			if (retVal == null) return null;

			var rolesQuery = new SqlStatement<DbSecurityUserRole>().SelectFrom()
				.InnerJoin<DbSecurityRole>(o => o.RoleKey, o => o.Key)
				.Where<DbSecurityUserRole>(o => o.UserKey == dbUser.Key);

			retVal.Roles = context.Query<DbSecurityRole>(rolesQuery).Select(o => m_mapper.MapDomainInstance<DbSecurityRole, Core.Model.Security.SecurityRole>(o)).ToList();
			return retVal;
		}

		/// <summary>
		/// Update the roles to security user
		/// </summary>
		public override Core.Model.Security.SecurityUser Update(DataContext context, Core.Model.Security.SecurityUser data, IPrincipal principal)
		{
			var retVal = base.Update(context, data, principal);

			if (data.Roles == null)
			{
				return retVal;
			}

			context.Delete<DbSecurityUserRole>(o => o.UserKey == retVal.Key);
			foreach (var r in data.Roles)
			{
				r.EnsureExists(context, principal);
				context.Insert(new DbSecurityUserRole()
				{
					UserKey = retVal.Key.Value,
					RoleKey = r.Key.Value
				});
			}

			return retVal;
		}
	}
}