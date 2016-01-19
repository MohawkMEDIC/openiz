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
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Local policy information service uses the local database instance
    /// for policy decisions
    /// </summary>
    public class LocalPolicyInformationService : IPolicyInformationService
    {

        // Policy tracer
        private TraceSource m_tracer = new TraceSource("OpenIZ.Core.Security.Policy");

        /// <summary>
        /// Get active policies for a securable
        /// </summary>
        public IEnumerable<IPolicy> GetActivePolicies(object securable)
        {
            if (securable is SecurityEntity)
                return (securable as SecurityEntity).Policies.Select(o => o.Policy).Where(o => !o.ObsoletionTime.HasValue).OfType<IPolicy>();
            // TODO: Acts and Entities
            return null;
        }

        /// <summary>
        /// Get all policies
        /// </summary>
        public IEnumerable<IPolicy> GetPolicies()
        {
            var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityPolicy>>();
            return dataPersistence.Query(p => p.ObsoletionTime == null, null).OfType<IPolicy>();
        }

        /// <summary>
        /// Get a policy by the policy OID
        /// </summary>
        public IPolicy GetPolicy(string policyOid)
        {
            var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityPolicy>>();
            return dataPersistence.Query(p => p.ObsoletionTime == null && p.Oid == policyOid, null).FirstOrDefault();
        }
    }
}
