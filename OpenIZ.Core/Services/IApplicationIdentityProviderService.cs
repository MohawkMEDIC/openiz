using MARC.HI.EHRS.SVC.Core.Event;
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
        /// Fired prior to an authentication request being made
        /// </summary>
        event EventHandler<AuthenticatingEventArgs> Authenticating;

        /// <summary>
        /// Fired after an authentication request has been made
        /// </summary>
        event EventHandler<AuthenticatedEventArgs> Authenticated;

        /// <summary>
        /// Authenticate the application identity
        /// </summary>
        IPrincipal Authenticate(String applicationId, String applicationSecret);
    }
}
