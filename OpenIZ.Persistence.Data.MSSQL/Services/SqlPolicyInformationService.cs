using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Services;
using OpenIZ.Persistence.Data.MSSQL.Configuration;
using OpenIZ.Persistence.Data.MSSQL.Data;
using OpenIZ.Persistence.Data.MSSQL.Services.Persistence;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Services
{
    /// <summary>
    /// Represents a PIP fed from SQL Server tables
    /// </summary>
    public class SqlPolicyInformationService : IExtendedPolicyInformationService
    {
        // Get the SQL configuration
        private SqlConfiguration m_configuration = ConfigurationManager.GetSection("openiz.persistence.data.mssql") as SqlConfiguration;

        
        /// <summary>
        /// Get active policies for the specified securable type
        /// </summary>
        public IEnumerable<IPolicy> GetActivePolicies(object securable)
        {
            return this.GetActivePolicyInstances(securable as IdentifiedData).Select(o => o.Policy);
        }

        /// <summary>
        /// Get active policy instances
        /// </summary>
        public IEnumerable<SecurityPolicyInstance> GetActivePolicyInstances(IdentifiedData securable)
        {
            var policyInstance = new SecurityPolicyPersistenceService();
            using (ModelDataContext context = new ModelDataContext(this.m_configuration.ReadonlyConnectionString))
                // Security device
                if (securable is Core.Model.Security.SecurityDevice)
                    return context.SecurityDevicePolicies.Where(o => o.DeviceId == securable.Key && o.Policy.ObsoletionTime == null).Select(o => new SecurityPolicyInstance(
                          policyInstance.ConvertToModel(o.Policy as Policy), (PolicyDecisionOutcomeType)o.PolicyAction));
                else if (securable is Core.Model.Security.SecurityRole)
                    return context.SecurityRolePolicies.Where(o => o.RoleId == securable.Key && o.Policy.ObsoletionTime == null).Select(o => new SecurityPolicyInstance(
                          policyInstance.ConvertToModel(o.Policy as Policy), (PolicyDecisionOutcomeType)o.PolicyAction));
                else if (securable is Core.Model.Security.SecurityUser)
                    return (securable as Core.Model.Security.SecurityUser).Policies;
                else if (securable is Core.Model.Security.SecurityApplication)
                    return context.SecurityApplicationPolicies.Where(o => o.ApplicationId == securable.Key && o.Policy.ObsoletionTime == null).Select(o => new SecurityPolicyInstance(
                          policyInstance.ConvertToModel(o.Policy as Policy), (PolicyDecisionOutcomeType)o.PolicyAction));
                else if (securable is Core.Model.Acts.Act)
                {
                    var pAct = securable as Core.Model.Acts.Act;
                    return context.ActPolicies.Where(o => o.ActId == securable.Key && o.Policy.ObsoletionTime == null && pAct.VersionSequence >= o.EffectiveVersionSequenceId && (pAct.VersionSequence < o.ObsoleteVersionSequenceId || o.ObsoleteVersionSequenceId == null)).Select(o => new SecurityPolicyInstance(
                          policyInstance.ConvertToModel(o.Policy as Policy), PolicyDecisionOutcomeType.Grant));
                }
                else if (securable is Core.Model.Entities.Entity)
                {
                    throw new NotImplementedException();
                }
                else
                    return new List<SecurityPolicyInstance>();
        }

        /// <summary>
        /// Get all policies on the system
        /// </summary>
        public IEnumerable<IPolicy> GetPolicies()
        {
            var policyInstance = new SecurityPolicyPersistenceService();
            using (var dataContext = new ModelDataContext(this.m_configuration.ReadonlyConnectionString))
                return dataContext.Policies.Where(o => o.ObsoletionTime == null).Select(o => policyInstance.ConvertToModel(o as Policy));
        }

        /// <summary>
        /// Get a specific policy
        /// </summary>
        public IPolicy GetPolicy(string policyOid)
        {
            var policyInstance = new SecurityPolicyPersistenceService();
            using (var dataContext = new ModelDataContext(this.m_configuration.ReadonlyConnectionString))
                return policyInstance.ConvertToModel(dataContext.Policies.SingleOrDefault(o=>o.PolicyOid == policyOid));
        }
    }
}
