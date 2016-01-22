/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-1-19
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core;
using System.ComponentModel;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using System.Runtime.Serialization;
using OpenIZ.Core.Model.Attributes;

namespace OpenIZ.Core.Model.Security
{
    /// <summary>
    /// Represents a simply security policy
    /// </summary>
    [DataContract(Name = "SecurityPolicy", Namespace = "http://openiz.org/model")]
    [Serializable]
    [Resource(ModelScope.Security)]
    public class SecurityPolicy : BaseEntityData, IPolicy
    {
        
        /// <summary>
        /// Gets or sets the handler which may handle this policy
        /// </summary>
        [IgnoreDataMember]
        public Type Handler { get; set; }

        /// <summary>
        /// Gets or sets the name of the policy
        /// </summary>
        [DataMember(Name = "name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the universal ID
        /// </summary>
        [DataMember(Name = "o")]
        public String Oid { get; set; }

        /// <summary>
        /// Whether the property is public
        /// </summary>
        [DataMember(Name = "isPublic")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Whether the policy can be elevated over
        /// </summary>
        [DataMember(Name = "canOverride")]
        public bool CanOverride { get; set; }
    }

    /// <summary>
    /// Represents a security policy instance
    /// </summary>
    public class SecurityPolicyInstance 
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public SecurityPolicyInstance()
        {

        }

        /// <summary>
        /// Creates a new policy instance with the specified policy and grant
        /// </summary>
        public SecurityPolicyInstance(SecurityPolicy policy, PolicyDecisionOutcomeType grantType)
        {
            this.Policy = policy;
            this.GrantType = grantType;
        }

        /// <summary>
        /// The policy
        /// </summary>
        public SecurityPolicy Policy { get; set; }

        /// <summary>
        /// Gets or sets whether the policy is a Deny
        /// </summary>
        public PolicyDecisionOutcomeType GrantType { get; set; }

    }
}
