using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Security;
using MARC.HI.EHRS.SVC.Core;
using System.Security;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Represents a default implementation of a TFA relay service which scans the entire application domain for 
    /// mechanisms and allows calling of them all
    /// </summary>
    public class DefaultTfaRelayService : ITfaRelayService
    {

        /// <summary>
        /// Construct the default relay service
        /// </summary>
        public DefaultTfaRelayService()
        {
            this.Mechanisms = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(o => typeof(ITfaMechanism).IsAssignableFrom(o) && o.IsClass).Select(o => Activator.CreateInstance(o) as ITfaMechanism));
        }

        /// <summary>
        /// Gets the configured mechanisms
        /// </summary>
        public IEnumerable<ITfaMechanism> Mechanisms
        {
            get; private set;
        }

        /// <summary>
        /// Sends the secret via the specified mechanism
        /// </summary>
        public void SendSecret(Guid mechanismId, SecurityUser user, string mechanismVerification, string tfaSecret)
        {
            // Get the mechanism
            var mechanism = this.Mechanisms.FirstOrDefault(o => o.Id == mechanismId);
            if (mechanism == null)
                throw new SecurityException($"TFA mechanism {mechanismId} not found");

            // send the secret
            mechanism.Send(user, mechanismVerification, tfaSecret);
        }
    }
}
