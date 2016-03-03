using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security.Claims
{
    /// <summary>
    /// Identifies a claim type handler
    /// </summary>
    public interface IClaimTypeHandler
    {

        /// <summary>
        /// Get the specified claim type this handler handles
        /// </summary>
        String ClaimType { get; }

        /// <summary>
        /// Validate the specified claim
        /// </summary>
        bool Validate(IPrincipal principal, String value);

    }
}
