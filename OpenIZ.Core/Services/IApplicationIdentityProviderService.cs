using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents a service which retrieves IPrincipal objects for applications
    /// </summary>
    public interface IApplicationIdentityProviderService 
    {

        /// <summary>
        /// Authenticate the application identity
        /// </summary>
        IPrincipal Authenticate(String applicationId, String applicationSecret);
    }
}
