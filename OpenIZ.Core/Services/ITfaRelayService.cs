using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents the TFA mechanism
    /// </summary>
    public interface ITfaMechanism
    {

        /// <summary>
        /// Gets the identifier of the mechanism
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name of the TFA mechanism
        /// </summary>
        String Name { get; }

        /// <summary>
        /// The challenge text this mechanism uses
        /// </summary>
        String Challenge { get; }

        /// <summary>
        /// Send the specified two factor authentication via the mechanism
        /// </summary>
        void Send(SecurityUser user, String challengeResponse, String tfaSecret);

    }

    /// <summary>
    /// Represents a two-factor authentication relay service
    /// </summary>
    public interface ITfaRelayService
    {

        /// <summary>
        /// Send the secret for the specified user
        /// </summary>
        void SendSecret(Guid mechanismId, SecurityUser user, String mechanismVerification, String tfaSecret);
        
        /// <summary>
        /// Gets the tfa mechanisms supported by this relay service
        /// </summary>
        IEnumerable<ITfaMechanism> Mechanisms { get; }
        
    }
}
