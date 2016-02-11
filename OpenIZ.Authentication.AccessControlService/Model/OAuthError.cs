using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Authentication.AccessControlService.Model
{
    /// <summary>
    /// OAuth error type
    /// </summary>
    public enum OAuthErrorType
    {
        invalid_request,
        invalid_client,
        invalid_grant,
        unauthorized_client,
        unsupported_grant_type,
        invalid_scope
    }

    /// <summary>
    /// OAuth error response message
    /// </summary>
    [JsonObject]
    public class OAuthError
    {

        /// <summary>
        /// Gets or sets the error
        /// </summary>
        [JsonProperty("error")]
        public OAuthErrorType Error { get; set; }

        /// <summary>
        /// Description of the error
        /// </summary>
        [JsonProperty("error_description")]
        public String ErrorDescription { get; set; }
    }
}
