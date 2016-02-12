using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Authentication.OAuth2
{
    /// <summary>
    /// OAuth constants
    /// </summary>
    public static class OAuthConstants
    {

        /// <summary>
        /// ACS trace source name
        /// </summary>
        public const string TraceSourceName = "OpenIZ.Authentication.OAuth2";


        /// <summary>
        /// Grant name for password grant
        /// </summary>
        public const string GrantNamePassword = "password";

        /// <summary>
        /// JWT token type
        /// </summary>
        public const string JwtTokenType = "urn:ietf:params:oauth:token-type:jwt";

        /// <summary>
        /// Configuration section name
        /// </summary>
        public const string ConfigurationName = "openiz.authentication.oauth2";
    }
}
