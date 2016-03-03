using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Authentication.OAuth2.Configuration
{
    /// <summary>
    /// OAuth2 configuration
    /// </summary>
    public class OAuthConfiguration
    {

        /// <summary>
        /// Creates a new instance of the OAuth configuration
        /// </summary>
        public OAuthConfiguration()
        {
            this.AllowedScopes = new List<string>();
            this.AllowedClientClaims = new List<string>();
        }

        /// <summary>
        /// Signing certificate
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Gets or sets the expiry time
        /// </summary>
        public TimeSpan ValidityTime { get; set; }

        /// <summary>
        /// Gets or sets whether the ACS will validate client claims
        /// </summary>
        public List<String> AllowedClientClaims { get; set; }

        /// <summary>
        /// Issuer name
        /// </summary>
        public String IssuerName { get; set; }
        /// <summary>
        /// When using HMAC256 signing this represents the server's secret
        /// </summary>
        public String ServerSecret { get; set; }

        /// <summary>
        /// The scopes that are permitted for granting on this endpoint
        /// </summary>
        public List<String> AllowedScopes { get; set; }

        /// <summary>
        /// Raw server key
        /// </summary>
        public byte[] ServerKey { get; internal set; }

        /// <summary>
        /// Validate
        /// </summary>
        internal void Validate()
        {
            if ((String.IsNullOrEmpty(this.ServerSecret) || this.ServerKey != null) &&
                this.Certificate == null)
                throw new ConfigurationErrorsException("Configuration must use symmetric key or certificate");

        }
    }
}
