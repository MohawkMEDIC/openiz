using System.Collections.Generic;

namespace OpenIZ.Core.Configuration
{
    /// <summary>
    /// Basic authorization configuration
    /// </summary>
    public class OpenIzBasicAuthorization
    {

        /// <summary>
        /// Require client authentication.
        /// </summary>
        public bool RequireClientAuth { get; set; }

        /// <summary>
        /// Allowed claims 
        /// </summary>
        public List<string> AllowedClientClaims { get; set; }

    }
}