using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents an extended policy information service
    /// </summary>
    public interface IExtendedPolicyInformationService : IPolicyInformationService
    {

        /// <summary>
        /// Get the policy instances
        /// </summary>
        IEnumerable<SecurityPolicyInstance> GetActivePolicyInstances(IdentifiedData securable);
    }
}
