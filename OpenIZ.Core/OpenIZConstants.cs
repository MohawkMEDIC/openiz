using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core
{
    /// <summary>
    /// OpenIZ constants
    /// </summary>
    internal static class OpenIzConstants
    {

        // Configuration name
        internal const string OpenIZConfigurationName = "openiz.core";

        // Security trace source
        internal const string SecurityTraceSourceName = "OpenIZ.Core.Security";

        // Map trace source
        internal const string MapTraceSourceName= "OpenIZ.Core.Map";

        // Client claim header
        internal const string BasicHttpClientClaimHeaderName = "X-OpenIZClient-Claim";
        // Client auth header
        internal const string BasicHttpClientCredentialHeaderName = "X-OpenIZClient-Authorization";
    }
}
