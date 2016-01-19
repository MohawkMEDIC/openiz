using MARC.HI.EHRS.SVC.Core.Services.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Represnts an individual policy handler instance
    /// </summary>
    public interface IPolicyHandler 
    {

        /// <summary>
        /// Get policy decision
        /// </summary>
        PolicyDecision GetPolicyDecision(IPrincipal principal, IPolicy policy, Object securable);
    }
}
