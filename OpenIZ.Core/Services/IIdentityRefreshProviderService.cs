using MARC.HI.EHRS.SVC.Core.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents an identity provider service that can do refresh tokens
    /// </summary>
    public interface IIdentityRefreshProviderService : IIdentityProviderService
    {

        /// <summary>
        /// Create a refresh token for the specified principal
        /// </summary>
        byte[] CreateRefreshToken(IPrincipal principal);

        /// <summary>
        /// Authenticate using a refresh token
        /// </summary>
        IPrincipal Authenticate(byte[] refreshToken);

    }
}
