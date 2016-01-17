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
