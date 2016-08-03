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
 * Date: 2016-6-14
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Claims;
using OpenIZ.Persistence.Data.MSSQL.Configuration;
using OpenIZ.Persistence.Data.MSSQL.Data;
using OpenIZ.Persistence.Data.MSSQL.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Services
{
    /// <summary>
    /// Represents a PIP fed from SQL Server tables
    /// </summary>
    public class SqlPolicyInformationService : IPolicyInformationService
    {
        // Get the SQL configuration
        private SqlConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.persistence.data.mssql") as SqlConfiguration;
        
        /// <summary>
        /// Get active policies for the specified securable type
        /// </summary>
        public IEnumerable<IPolicyInstance> GetActivePolicies(object securable)
        {
            using (ModelDataContext context = new ModelDataContext(this.m_configuration.ReadonlyConnectionString))
                // Security device
                if (securable is Core.Model.Security.SecurityDevice)
                    return context.SecurityDevicePolicies.Where(o => o.DeviceId == (securable as IdentifiedData).Key && o.Policy.ObsoletionTime == null).Select(o => new SqlSecurityPolicyInstance(o)).ToList();
                else if (securable is Core.Model.Security.SecurityRole)
                    return context.SecurityRolePolicies.Where(o => o.RoleId == (securable as IdentifiedData).Key && o.Policy.ObsoletionTime == null).Select(o => new SqlSecurityPolicyInstance(o)).ToList();
                else if (securable is Core.Model.Security.SecurityApplication)
                    return context.SecurityApplicationPolicies.Where(o => o.ApplicationId == (securable as IdentifiedData).Key && o.Policy.ObsoletionTime == null).Select(o => new SqlSecurityPolicyInstance(o)).ToList();
                else if (securable is ApplicationPrincipal)
                {
                    var sid = (securable as ApplicationPrincipal).FindFirst(ClaimTypes.Sid);
                    Guid appId = Guid.Parse(sid.Value);
                    return context.SecurityApplicationPolicies.Where(o => o.ApplicationId == appId && o.Policy.ObsoletionTime == null).Select(o => new SqlSecurityPolicyInstance(o)).ToList();
                }
                else if (securable is IPrincipal || securable is IIdentity)
                {
                    var identity = (securable as IPrincipal)?.Identity ?? securable as IIdentity;
                    var user = context.SecurityUsers.SingleOrDefault(u => u.UserName == identity.Name);
                    if (user == null)
                        throw new KeyNotFoundException("Identity not found");

                    List<IPolicyInstance> retVal = new List<IPolicyInstance>();

                    // Role policies
                    var roleIds = user.SecurityUserRoles.Select(o => o.RoleId).ToList();
                    retVal.AddRange(context.SecurityRolePolicies.Where(o => roleIds.Contains(o.RoleId)).Select(o => new SqlSecurityPolicyInstance(o)));

                    // Claims principal, then we want device and app SID
                    if (securable is ClaimsPrincipal)
                    {
                        var cp = securable as ClaimsPrincipal;
                        var appClaim = cp.FindAll(OpenIzClaimTypes.OpenIzApplicationIdentifierClaim).SingleOrDefault();
                        var devClaim = cp.FindAll(OpenIzClaimTypes.OpenIzDeviceIdentifierClaim).SingleOrDefault();

                        // There is an application claim so we want to add the application policies - most restrictive
                        if (appClaim != null)
                            retVal.AddRange(context.SecurityApplicationPolicies.Where(o => o.SecurityApplication.ApplicationId == Guid.Parse(appClaim.Value)).Select(o => new SqlSecurityPolicyInstance(o)));
                        // There is an device claim so we want to add the device policies - most restrictive
                        if (devClaim != null)
                            retVal.AddRange(context.SecurityDevicePolicies.Where(o => o.SecurityDevice.DeviceId == Guid.Parse(devClaim.Value)).Select(o => new SqlSecurityPolicyInstance(o)));
                    }

                    // TODO: Most restrictive
                    return retVal;
                }
                else if (securable is Core.Model.Acts.Act)
                {
                    var pAct = securable as Core.Model.Acts.Act;
                    return context.ActPolicies.Where(o => o.ActId == (securable as IdentifiedData).Key && o.Policy.ObsoletionTime == null && pAct.VersionSequence >= o.EffectiveVersionSequenceId && (pAct.VersionSequence < o.ObsoleteVersionSequenceId || o.ObsoleteVersionSequenceId == null)).Select(o => new SqlSecurityPolicyInstance(o));
                }
                else if (securable is Core.Model.Entities.Entity)
                {
                    throw new NotImplementedException();
                }
                else
                    return new List<IPolicyInstance>();
        }

        /// <summary>
        /// Get all policies on the system
        /// </summary>
        public IEnumerable<IPolicy> GetPolicies()
        {
            using (var dataContext = new ModelDataContext(this.m_configuration.ReadonlyConnectionString))
                return dataContext.Policies.Where(o => o.ObsoletionTime == null).Select(o => new SqlSecurityPolicy(o));
        }

        /// <summary>
        /// Get a specific policy
        /// </summary>
        public IPolicy GetPolicy(string policyOid)
        {
            using (var dataContext = new ModelDataContext(this.m_configuration.ReadonlyConnectionString))
            {
                var policy = dataContext.Policies.SingleOrDefault(o => o.PolicyOid == policyOid);
                if(policy != null)
                    return new SqlSecurityPolicy(policy);
                return null;
            }
        }
    }
}
