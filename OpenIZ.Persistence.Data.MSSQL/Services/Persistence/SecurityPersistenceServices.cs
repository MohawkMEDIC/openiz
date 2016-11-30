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
using OpenIZ.Core.Model.Security;
using OpenIZ.Persistence.Data.MSSQL.Data;
using OpenIZ.Core.Model.Interfaces;
using System.Data.Linq;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Security user persistence
    /// </summary>
    public class SecurityUserPersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityUser, Data.SecurityUser>
    {
        public override Core.Model.Security.SecurityUser ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var dbUser = dataInstance as Data.SecurityUser;
            var retVal = base.ToModelInstance(dataInstance, context, principal);
            retVal.Roles = dbUser.SecurityUserRoles.Select(o => m_mapper.MapDomainInstance<Data.SecurityRole, Core.Model.Security.SecurityRole>(o.SecurityRole)).ToList();
            return retVal;
        }
        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override Core.Model.Security.SecurityUser Insert(ModelDataContext context, Core.Model.Security.SecurityUser data, IPrincipal principal)
        {
            var retVal = base.Insert(context, data, principal);

            // Roles
            if (data.Roles != null)
                foreach(var r in data.Roles)
                {
                    r.EnsureExists(context, principal);
                    context.SecurityUserRoles.InsertOnSubmit(new SecurityUserRole()
                    {
                        UserId = retVal.Key.Value,
                        RoleId = r.Key.Value
                    });
                }

            return retVal;
        }

        /// <summary>
        /// Update the roles to security user
        /// </summary>
        public override Core.Model.Security.SecurityUser Update(ModelDataContext context, Core.Model.Security.SecurityUser data, IPrincipal principal)
        {
            var retVal = base.Update(context, data, principal);

	        if (data.Roles == null)
	        {
		        return retVal;
	        }

	        context.SecurityUserRoles.DeleteAllOnSubmit(context.SecurityUserRoles.Where(o => o.UserId == retVal.Key));
	        foreach (var r in data.Roles)
	        {
		        r.EnsureExists(context, principal);
		        context.SecurityUserRoles.InsertOnSubmit(new SecurityUserRole()
		        {
			        UserId = retVal.Key.Value,
			        RoleId = r.Key.Value
		        });
	        }

	        return retVal;
        }

        /// <summary>
        /// Data load options
        /// </summary>
        /// <returns></returns>
        internal override DataLoadOptions GetDataLoadOptions()
        {
            var baseOptions = base.GetDataLoadOptions();
            baseOptions.LoadWith<Data.SecurityUser>(o => o.SecurityUserRoles);
            return baseOptions;
        }
    }

    /// <summary>
    /// Security user persistence
    /// </summary>
    public class SecurityRolePersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityRole, Data.SecurityRole>
    {
        /// <summary>
        /// Represent as model instance
        /// </summary>
        public override Core.Model.Security.SecurityRole ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var retVal = base.ToModelInstance(dataInstance, context, principal);
            retVal.Policies = (dataInstance as Data.SecurityRole).SecurityRolePolicies.Select(o => m_mapper.MapDomainInstance<Data.SecurityRolePolicy, Core.Model.Security.SecurityPolicyInstance>(o)).ToList();
            retVal.Users = (dataInstance as Data.SecurityRole).SecurityUserRoles.Select(o => m_mapper.MapDomainInstance<Data.SecurityUser, Core.Model.Security.SecurityUser>(o.SecurityUser)).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override Core.Model.Security.SecurityRole Insert(ModelDataContext context, Core.Model.Security.SecurityRole data, IPrincipal principal)
        {
            var retVal = base.Insert(context, data, principal);

			if (data.Policies == null)
			{
		        return retVal;
	        }

	        data.Policies.ForEach(o => o.EnsureExists(context, principal));
	        context.SecurityRolePolicies.InsertAllOnSubmit(data.Policies.Select(o => new Data.SecurityRolePolicy()
	        {
		        PolicyId = o.PolicyKey.Value,
		        PolicyAction = (int)o.GrantType,
		        RoleId = retVal.Key.Value,
		        SecurityPolicyInstanceId = Guid.NewGuid()
	        }));

	        return retVal;
        }

        /// <summary>
        /// Update the roles to security user
        /// </summary>
        public override Core.Model.Security.SecurityRole Update(ModelDataContext context, Core.Model.Security.SecurityRole data, IPrincipal principal)
        {
			var domainInstance = this.FromModelInstance(data, context, principal) as Data.SecurityRole;

			var currentObject = context.GetTable<Data.SecurityRole>().FirstOrDefault(ExpressionRewriter.Rewrite<Data.SecurityRole>(o => o.Id == data.Key));

			if (currentObject == null)
			{
				throw new KeyNotFoundException(data.Key.ToString());
			}

			currentObject.CopyObjectData(domainInstance);

			currentObject.ObsoletedBy = data.ObsoletedByKey == Guid.Empty ? null : data.ObsoletedByKey;
			currentObject.ObsoletionTime = data.ObsoletionTime;

			context.SubmitChanges();

			context.SecurityRolePolicies.DeleteAllOnSubmit(context.SecurityRolePolicies.Where(o => o.RoleId == domainInstance.Id));

			context.SubmitChanges();

			context.SecurityRolePolicies.InsertAllOnSubmit(data.Policies.Select(o => new Data.SecurityRolePolicy
			{
				PolicyId = o.PolicyKey.Value,
				PolicyAction = (int)o.GrantType,
				RoleId = domainInstance.Id,
				SecurityPolicyInstanceId = Guid.NewGuid()
			}));

			context.SubmitChanges();

			return data;
		}

		public override object FromModelInstance(Core.Model.Security.SecurityRole modelInstance, ModelDataContext context, IPrincipal princpal)
		{
			var domainInstance = base.FromModelInstance(modelInstance, context, princpal) as Data.SecurityRole;

			domainInstance.SecurityRolePolicies.AddRange(modelInstance.Policies.Select(o => new SecurityRolePolicy
			{
				PolicyId = o.PolicyKey.Value,
				PolicyAction = (int)o.GrantType,
				RoleId = modelInstance.Key.GetValueOrDefault(Guid.NewGuid()),
				SecurityPolicyInstanceId = Guid.NewGuid()
			}));

			return domainInstance;
		}

	}

    /// <summary>
    /// Security user persistence
    /// </summary>
    public class SecurityDevicePersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityDevice, Data.SecurityDevice>
    {
        /// <summary>
        /// Represent as model instance
        /// </summary>
        public override Core.Model.Security.SecurityDevice ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var retVal = base.ToModelInstance(dataInstance, context, principal);
            retVal.Policies = (dataInstance as Data.SecurityDevice).SecurityDevicePolicies.Select(o => m_mapper.MapDomainInstance<Data.SecurityDevicePolicy, Core.Model.Security.SecurityPolicyInstance>(o)).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override Core.Model.Security.SecurityDevice Insert(ModelDataContext context, Core.Model.Security.SecurityDevice data, IPrincipal principal)
        {
            var retVal = base.Insert(context, data, principal);

			if (data.Policies == null)
			{
		        return retVal;
	        }

	        data.Policies.ForEach(o => o.EnsureExists(context, principal));
	        context.SecurityDevicePolicies.InsertAllOnSubmit(data.Policies.Select(o => new Data.SecurityDevicePolicy()
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
        public override Core.Model.Security.SecurityDevice Update(ModelDataContext context, Core.Model.Security.SecurityDevice data, IPrincipal principal)
        {
			var domainInstance = this.FromModelInstance(data, context, principal) as Data.SecurityDevice;

			var currentObject = context.GetTable<Data.SecurityDevice>().FirstOrDefault(ExpressionRewriter.Rewrite<Data.SecurityDevice>(o => o.Id == data.Key));

	        if (currentObject == null)
	        {
				throw new KeyNotFoundException(data.Key.ToString());
			}

			currentObject.CopyObjectData(domainInstance);

			currentObject.ObsoletedBy = data.ObsoletedByKey == Guid.Empty ? null : data.ObsoletedByKey;
			currentObject.ObsoletionTime = data.ObsoletionTime;

			context.SubmitChanges();

			context.SecurityDevicePolicies.DeleteAllOnSubmit(context.SecurityDevicePolicies.Where(o => o.DeviceId == domainInstance.Id));

			context.SubmitChanges();

			context.SecurityDevicePolicies.InsertAllOnSubmit(data.Policies.Select(o => new Data.SecurityDevicePolicy
	        {
		        PolicyId = o.PolicyKey.Value,
		        PolicyAction = (int)o.GrantType,
		        DeviceId = domainInstance.Id,
		        SecurityPolicyInstanceId = Guid.NewGuid()
	        }));

			context.SubmitChanges();

			return data;
        }

		public override object FromModelInstance(Core.Model.Security.SecurityDevice modelInstance, ModelDataContext context, IPrincipal princpal)
		{
			var domainInstance = base.FromModelInstance(modelInstance, context, princpal) as Data.SecurityDevice;

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
    public class SecurityApplicationPersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityApplication, Data.SecurityApplication>
    {
        /// <summary>
        /// Represent as model instance
        /// </summary>
        public override Core.Model.Security.SecurityApplication ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var retVal = base.ToModelInstance(dataInstance, context, principal);
            retVal.Policies = (dataInstance as Data.SecurityApplication).SecurityApplicationPolicies.Select(o => m_mapper.MapDomainInstance<Data.SecurityApplicationPolicy, Core.Model.Security.SecurityPolicyInstance>(o)).ToList();
            return retVal;
        }

	    /// <summary>
	    /// Insert the specified object
	    /// </summary>
	    public override Core.Model.Security.SecurityApplication Insert(ModelDataContext context, Core.Model.Security.SecurityApplication data, IPrincipal principal)
	    {
		    var retVal = base.Insert(context, data, principal);

			if (data.Policies == null)
			{
				context.SecurityApplicationPolicies.InsertAllOnSubmit(data.Policies.Select(o => new Data.SecurityApplicationPolicy()
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
        public override Core.Model.Security.SecurityApplication Update(ModelDataContext context, Core.Model.Security.SecurityApplication data, IPrincipal principal)
        {
			var domainInstance = this.FromModelInstance(data, context, principal) as Data.SecurityApplication;

			var currentObject = context.GetTable<Data.SecurityApplication>().FirstOrDefault(ExpressionRewriter.Rewrite<Data.SecurityApplication>(o => o.Id == data.Key));

			if (currentObject == null)
			{
				throw new KeyNotFoundException(data.Key.ToString());
			}

			currentObject.CopyObjectData(domainInstance);

			currentObject.ObsoletedBy = data.ObsoletedByKey == Guid.Empty ? null : data.ObsoletedByKey;
			currentObject.ObsoletionTime = data.ObsoletionTime;

			context.SubmitChanges();

			context.SecurityApplicationPolicies.DeleteAllOnSubmit(context.SecurityApplicationPolicies.Where(o => o.ApplicationId == domainInstance.Id));

			context.SubmitChanges();

			context.SecurityApplicationPolicies.InsertAllOnSubmit(data.Policies.Select(o => new Data.SecurityApplicationPolicy
			{
				PolicyId = o.PolicyKey.Value,
				PolicyAction = (int)o.GrantType,
				ApplicationId = domainInstance.Id,
				SecurityPolicyInstanceId = Guid.NewGuid()
			}));

			context.SubmitChanges();

			return data;
        }

		public override object FromModelInstance(Core.Model.Security.SecurityApplication modelInstance, ModelDataContext context, IPrincipal princpal)
		{
			var domainInstance = base.FromModelInstance(modelInstance, context, princpal) as Data.SecurityApplication;

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
