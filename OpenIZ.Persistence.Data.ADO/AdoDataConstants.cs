using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO
{
    /// <summary>
    /// ADO data constants to be used in the ADO provider
    /// </summary>
    public static class AdoDataConstants
    {

        /// <summary>
        /// Represents the trace source name
        /// </summary>
        public const string TraceSourceName = "OpenIZ.Persistence.Data.ADO";
        /// <summary>
        /// Identity trace source name
        /// </summary>
        public const string IdentityTraceSourceName = "OpenIZ.Persistence.Data.ADO.Identity";

        /// <summary>
        /// Represents the configuration section name
        /// </summary>
        public const string ConfigurationSectionName = "openiz.persistence.data.ado";

        /// <summary>
        /// Refresh secret claim
        /// </summary>
        public const string RefreshSecretClaimType = "http://openiz.org/claims/sec-ado/refreshSecret";
        /// <summary>
        /// The refresh secret
        /// </summary>
        public const string RefreshExpiryClaimType = "http://openiz.org/claims/sec-ado/refreshExpiry";
        /// <summary>
        /// The map resource file name
        /// </summary>
        public const string MapResourceName = "OpenIZ.Persistence.Data.ADO.Data.Map.ModelMap.xml";
    }
}
