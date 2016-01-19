/**
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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Security
{
    /// <summary>
    /// Security Entity base class
    /// </summary>
    [DataContract(Namespace = "http://openiz.org/model", Name = "SecurityEntity")]
    public abstract class SecurityEntity : BaseEntityData
    {

        // Policies
        [NonSerialized]
        private List<SecurityPolicyInstance> m_policies = new List<SecurityPolicyInstance>();

        /// <summary>
        /// Policies associated with the entity
        /// </summary>
        [IgnoreDataMember]
        public virtual List<SecurityPolicyInstance> Policies { get { return this.m_policies; } }
        
    }
}
