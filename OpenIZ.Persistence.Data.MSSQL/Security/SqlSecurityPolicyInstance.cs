/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Persistence.Data.MSSQL.Data;
using OpenIZ.Persistence.Data.MSSQL.Services;

namespace OpenIZ.Persistence.Data.MSSQL.Security
{
    /// <summary>
    /// Represents a local security policy instance
    /// </summary>
    public class SqlSecurityPolicyInstance : IPolicyInstance
    {
        // Model mapper instance
        private static ModelMapper s_mapper = SqlServerPersistenceService.GetMapper();

        /// <summary>
        /// Local security policy instance
        /// </summary>
        public SqlSecurityPolicyInstance(Data.SecurityRolePolicy rolePolicy)
        {
            this.Policy = new SqlSecurityPolicy(rolePolicy.Policy);
            this.Rule = (PolicyDecisionOutcomeType)rolePolicy.PolicyAction;
            this.Securable = s_mapper.MapDomainInstance<Data.SecurityRole, Core.Model.Security.SecurityRole>(rolePolicy.SecurityRole);
        }

        /// <summary>
        /// Local security policy instance
        /// </summary>
        public SqlSecurityPolicyInstance(Data.SecurityDevicePolicy devicePolicy)
        {
            this.Policy = new SqlSecurityPolicy(devicePolicy.Policy);
            this.Rule = (PolicyDecisionOutcomeType)devicePolicy.PolicyAction;
            this.Securable = s_mapper.MapDomainInstance<Data.SecurityDevice, Core.Model.Security.SecurityDevice>(devicePolicy.SecurityDevice);
        }

        /// <summary>
        /// Local security policy instance
        /// </summary>
        public SqlSecurityPolicyInstance(Data.SecurityApplicationPolicy applicationPolicy)
        {
            this.Policy = new SqlSecurityPolicy(applicationPolicy.Policy);
            this.Rule = (PolicyDecisionOutcomeType)applicationPolicy.PolicyAction;
            this.Securable = s_mapper.MapDomainInstance<Data.SecurityApplication, Core.Model.Security.SecurityApplication>(applicationPolicy.SecurityApplication);

        }

        /// <summary>
        /// Local security policy instance
        /// </summary>
        public SqlSecurityPolicyInstance(Data.ActPolicy actPolicy)
        {
            this.Policy = new SqlSecurityPolicy(actPolicy.Policy);
            // TODO: Configuration of the policy as opt-in / opt-out
            this.Rule = PolicyDecisionOutcomeType.Grant;
            this.Securable = s_mapper.MapDomainInstance<Data.ActVersion, Core.Model.Acts.Act>(actPolicy.Act.ActVersions.CurrentVersion());
        }

        /// <summary>
        /// The policy 
        /// </summary>
        public IPolicy Policy { get; private set; }

        /// <summary>
        /// Policy outcome
        /// </summary>
        public PolicyDecisionOutcomeType  Rule { get; private set;}

        /// <summary>
        /// Securable
        /// </summary>
        public object Securable { get; private set; }
    }
}
