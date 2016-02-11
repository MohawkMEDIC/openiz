using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents an identity service which authenticates devices
    /// </summary>
    public interface IDeviceIdentityProviderService
    {

        /// <summary>
        /// Authenticate the device based on certificate provided
        /// </summary>
        IPrincipal Authenticate(X509Certificate2 deviceCertificate);
    }
}
