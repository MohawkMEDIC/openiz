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
 * User: justi
 * Date: 2017-1-16
 */
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Persistence.Data.ADO.Data.Model.Security;
using OpenIZ.Persistence.Data.ADO.Data.Model.Acts;
using OpenIZ.OrmLite;

namespace OpenIZ.Persistence.Data.PSQL.Security
{
    /// <summary>
    /// Represents a local security policy instance
    /// </summary>
    public class AdoSecurityPolicyInstance : IPolicyInstance
    {

        /// <summary>
        /// Local security policy instance
        /// </summary>
        public AdoSecurityPolicyInstance(DbSecurityRolePolicy rolePolicy, DbSecurityPolicy policy, object securable)
        {
            this.Policy = new AdoSecurityPolicy(policy);
            this.Rule = (PolicyDecisionOutcomeType)rolePolicy.GrantType;
            this.Securable = securable;
        }

        /// <summary>
        /// Local security policy instance
        /// </summary>
        public AdoSecurityPolicyInstance(DbSecurityDevicePolicy devicePolicy, DbSecurityPolicy policy, object securable)
        {
            this.Policy = new AdoSecurityPolicy(policy);
            this.Rule = (PolicyDecisionOutcomeType)devicePolicy.GrantType;
            this.Securable = securable;
        }

        /// <summary>
        /// Local security policy instance
        /// </summary>
        public AdoSecurityPolicyInstance(DbSecurityApplicationPolicy applicationPolicy, DbSecurityPolicy policy, object securable)
        {
            this.Policy = new AdoSecurityPolicy(policy);
            this.Rule = (PolicyDecisionOutcomeType)applicationPolicy.GrantType;
            this.Securable = securable;

        }

        /// <summary>
        /// Local security policy instance
        /// </summary>
        public AdoSecurityPolicyInstance(DbActSecurityPolicy actPolicy, DbSecurityPolicy policy, object securable)
        {
            this.Policy = new AdoSecurityPolicy(policy);
            // TODO: Configuration of the policy as opt-in / opt-out
            this.Rule = PolicyDecisionOutcomeType.Grant;
            this.Securable = securable;
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
