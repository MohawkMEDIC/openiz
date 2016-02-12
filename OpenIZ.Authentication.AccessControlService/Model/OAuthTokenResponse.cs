using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Authentication.OAuth2.Model
{
    /// <summary>
    /// OAuth token response
    /// </summary>
    [JsonObject]
    public class OAuthTokenResponse
    {

        /// <summary>
        /// Access token
        /// </summary>
        [JsonProperty("access_token")]
        public String AccessToken { get; set; }

        /// <summary>
        /// Token type
        /// </summary>
        [JsonProperty("token_type")]
        public String TokenType { get; set; }

        /// <summary>
        /// Expires in
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        [JsonProperty("refresh_token")]
        public String RefreshToken { get; set; }


    }
}
