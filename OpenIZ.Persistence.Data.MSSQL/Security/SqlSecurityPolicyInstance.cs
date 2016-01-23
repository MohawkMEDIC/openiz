using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Core.Model.Security;
using OpenIZ.Persistence.Data.MSSQL.Services.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Security
{
    /// <summary>
    /// Represents a local security policy instance
    /// </summary>
    public class SqlSecurityPolicyInstance : IPolicyInstance
    {


        /// <summary>
        /// Local security policy instance
        /// </summary>
        public SqlSecurityPolicyInstance(Data.SecurityRolePolicy rolePolicy)
        {
            this.Policy = new SqlSecurityPolicy(rolePolicy.Policy);
            this.Rule = (PolicyDecisionOutcomeType)rolePolicy.PolicyAction;
            this.Securable = new SecurityRolePersistenceService().ConvertToModel(rolePolicy.SecurityRole);
        }

        /// <summary>
        /// Local security policy instance
        /// </summary>
        public SqlSecurityPolicyInstance(Data.SecurityDevicePolicy devicePolicy)
        {
            this.Policy = new SqlSecurityPolicy(devicePolicy.Policy);
            this.Rule = (PolicyDecisionOutcomeType)devicePolicy.PolicyAction;
            // TODO: Securable
        }

        /// <summary>
        /// Local security policy instance
        /// </summary>
        public SqlSecurityPolicyInstance(Data.SecurityApplicationPolicy applicationPolicy)
        {
            this.Policy = new SqlSecurityPolicy(applicationPolicy.Policy);
            this.Rule = (PolicyDecisionOutcomeType)applicationPolicy.PolicyAction;
            // TODO: Securable
        }

        /// <summary>
        /// Local security policy instance
        /// </summary>
        public SqlSecurityPolicyInstance(Data.ActPolicy actPolicy)
        {
            this.Policy = new SqlSecurityPolicy(actPolicy.Policy);
            // TODO: Configuration of the policy as opt-in / opt-out
            this.Rule = PolicyDecisionOutcomeType.Grant;
            // TODO: Securable
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
