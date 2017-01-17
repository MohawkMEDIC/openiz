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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Core.Model.Interfaces;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Security;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Exceptions;
using OpenIZ.Persistence.Data.ADO.Util;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
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
    public class SecurityUserPersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityUser, DbSecurityUser>
    {
        public override Core.Model.Security.SecurityUser ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
        {
            var dbUser = dataInstance as DbSecurityUser;
            var retVal = base.ToModelInstance(dataInstance, context, principal);

            var rolesQuery = new SqlStatement<DbSecurityRole>().SelectFrom()
                .InnerJoin<DbSecurityUserRole>(o => o.Key, o => o.RoleKey)
                .Where<DbSecurityUserRole>(o => o.UserKey == dbUser.Key);

            retVal.Roles = context.Query<DbSecurityRole>(rolesQuery).Select(o => m_mapper.MapDomainInstance<DbSecurityRole, Core.Model.Security.SecurityRole>(o)).ToList();
            return retVal;
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

    /// <summary>
    /// Security user persistence
    /// </summary>
    public class SecurityRolePersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityRole, DbSecurityRole>
    {
        /// <summary>
        /// Represent as model instance
        /// </summary>
        public override Core.Model.Security.SecurityRole ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
        {
            var retVal = base.ToModelInstance(dataInstance, context, principal);

            var policyQuery = new SqlStatement<DbSecurityPolicy>().SelectFrom()
                .InnerJoin<DbSecurityRolePolicy>(o => o.Key, o => o.PolicyKey)
                .Where<DbSecurityRolePolicy>(o => o.SourceKey == retVal.Key);

            retVal.Policies = context.Query<DbSecurityPolicy>(policyQuery).Select(o => m_mapper.MapDomainInstance<DbSecurityPolicy, SecurityPolicyInstance>(o)).ToList();

            var rolesQuery = new SqlStatement<DbSecurityUser>().SelectFrom()
                .InnerJoin<DbSecurityUserRole>(o => o.Key, o => o.UserKey)
                .Where<DbSecurityUserRole>(o => o.RoleKey == retVal.Key);

            retVal.Users = context.Query<SecurityUser>(rolesQuery).Select(o => m_mapper.MapDomainInstance<SecurityUser, Core.Model.Security.SecurityUser>(o)).ToList();

            return retVal;
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override Core.Model.Security.SecurityRole Insert(DataContext context, Core.Model.Security.SecurityRole data, IPrincipal principal)
        {
            var retVal = base.Insert(context, data, principal);

            if (data.Policies != null)
            {

                // TODO: Clean this up
                data.Policies.ForEach(o => o.EnsureExists(context, principal));
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
    public class SecurityDevicePersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityDevice, DbSecurityDevice>
    {
        /// <summary>
        /// Represent as model instance
        /// </summary>
        public override Core.Model.Security.SecurityDevice ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
        {
            var retVal = base.ToModelInstance(dataInstance, context, principal);
            retVal.Policies = (dataInstance as DbSecurityDevice).SecurityDevicePolicies.Select(o => m_mapper.MapDomainInstance<Data.SecurityDevicePolicy, Core.Model.Security.SecurityPolicyInstance>(o)).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override Core.Model.Security.SecurityDevice Insert(DataContext context, Core.Model.Security.SecurityDevice data, IPrincipal principal)
        {
            var retVal = base.Insert(context, data, principal);

            if (data.Policies == null)
            {
                return retVal;
            }

            DbPolicies.ForEach(o => o.EnsureExists(context, principal));
            context.SecurityDevicePolicies.InsertAllOnSubmit(data.Policies.Select(o => newDbSecurityDevicePolicy()

            {
                PolicyId = o.PolicyKey.Value,
		        PolicyAction = (int)o.GrantType,
		        DeviceId = retVal.Key.Value,
		        SecurityPolicyInstanceId = Guid.NewGuid()

            }));

            return retVal;
        }

        /// <summary>
        /// Update the roles to security user
        /// </summary>
        public override Core.Model.Security.SecurityDevice Update(DataContext context, Core.Model.Security.SecurityDevice data, IPrincipal principal)
        {
            var domainInstance = this.FromModelInstance(data, context, principal) as DbSecurityDevice;

            var currentObject = context.GetTable<Data.SecurityDevice>().FirstOrDefault(ExpressionRewriter.Rewrite<Data.SecurityDevice>(o => o.Id == DbKey));

            if (currentObject == null)
            {
                throw new KeyNotFoundException(data.Key.ToString());
            }

            currentObject.CopyObjectData(domainInstance);

            currentObject.ObsoletedBy = DbObsoletedByKey == Guid.Empty ? null : DbObsoletedByKey;
            currentObject.ObsoletionTime = DbObsoletionTime;

            context.SubmitChanges();

            context.SecurityDevicePolicies.DeleteAllOnSubmit(context.SecurityDevicePolicies.Where(o => o.DeviceId == domainInstance.Id));

            context.SubmitChanges();

            context.SecurityDevicePolicies.InsertAllOnSubmit(data.Policies.Select(o => newDbSecurityDevicePolicy

            {
                PolicyId = o.PolicyKey.Value,
		        PolicyAction = (int)o.GrantType,
		        DeviceId = domainInstance.Id,
		        SecurityPolicyInstanceId = Guid.NewGuid()

            }));

            context.SubmitChanges();

            return data;
        }

        public override object FromModelInstance(Core.Model.Security.SecurityDevice modelInstance, DataContext context, IPrincipal princpal)
        {
            var domainInstance = base.FromModelInstance(modelInstance, context, princpal) as DbSecurityDevice;

            domainInstance.SecurityDevicePolicies.AddRange(modelInstance.Policies.Select(o => new SecurityDevicePolicy
            {
                PolicyId = o.PolicyKey.Value,
                PolicyAction = (int)o.GrantType,
                DeviceId = modelInstance.Key.GetValueOrDefault(Guid.NewGuid()),
                SecurityPolicyInstanceId = Guid.NewGuid()
            }));

            return domainInstance;
        }
    }

    /// <summary>
    /// Security user persistence
    /// </summary>
    public class SecurityApplicationPersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityApplication, DbSecurityApplication>
    {
        /// <summary>
        /// Represent as model instance
        /// </summary>
        public override Core.Model.Security.SecurityApplication ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
        {
            var retVal = base.ToModelInstance(dataInstance, context, principal);
            retVal.Policies = (dataInstance as DbSecurityApplication).SecurityApplicationPolicies.Select(o => m_mapper.MapDomainInstance<Data.SecurityApplicationPolicy, Core.Model.Security.SecurityPolicyInstance>(o)).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override Core.Model.Security.SecurityApplication Insert(DataContext context, Core.Model.Security.SecurityApplication data, IPrincipal principal)
        {
            var retVal = base.Insert(context, data, principal);

            if (data.Policies == null)
            {
                context.SecurityApplicationPolicies.InsertAllOnSubmit(data.Policies.Select(o => newDbSecurityApplicationPolicy()

                {
                    PolicyId = o.PolicyKey.Value,
					PolicyAction = (int)o.GrantType,
					ApplicationId = retVal.Key.Value,
					SecurityPolicyInstanceId = Guid.NewGuid()

                }));
            }

            return retVal;
        }

        /// <summary>
        /// Update the roles to security user
        /// </summary>
        public override Core.Model.Security.SecurityApplication Update(DataContext context, Core.Model.Security.SecurityApplication data, IPrincipal principal)
        {
            var domainInstance = this.FromModelInstance(data, context, principal) as DbSecurityApplication;

            var currentObject = context.GetTable<Data.SecurityApplication>().FirstOrDefault(ExpressionRewriter.Rewrite<Data.SecurityApplication>(o => o.Id == DbKey));

            if (currentObject == null)
            {
                throw new KeyNotFoundException(data.Key.ToString());
            }

            currentObject.CopyObjectData(domainInstance);

            currentObject.ObsoletedBy = DbObsoletedByKey == Guid.Empty ? null : DbObsoletedByKey;
            currentObject.ObsoletionTime = DbObsoletionTime;

            context.SubmitChanges();

            context.SecurityApplicationPolicies.DeleteAllOnSubmit(context.SecurityApplicationPolicies.Where(o => o.ApplicationId == domainInstance.Id));

            context.SubmitChanges();

            context.SecurityApplicationPolicies.InsertAllOnSubmit(data.Policies.Select(o => newDbSecurityApplicationPolicy

            {
                PolicyId = o.PolicyKey.Value,
				PolicyAction = (int)o.GrantType,
				ApplicationId = domainInstance.Id,
				SecurityPolicyInstanceId = Guid.NewGuid()

            }));

            context.SubmitChanges();

            return data;
        }

        public override object FromModelInstance(Core.Model.Security.SecurityApplication modelInstance, DataContext context, IPrincipal princpal)
        {
            var domainInstance = base.FromModelInstance(modelInstance, context, princpal) as DbSecurityApplication;

            domainInstance.SecurityApplicationPolicies.AddRange(modelInstance.Policies.Select(o => new SecurityApplicationPolicy
            {
                PolicyId = o.PolicyKey.Value,
                PolicyAction = (int)o.GrantType,
                ApplicationId = modelInstance.Key.GetValueOrDefault(Guid.NewGuid()),
                SecurityPolicyInstanceId = Guid.NewGuid()
            }));

            return domainInstance;
        }

    }
}
