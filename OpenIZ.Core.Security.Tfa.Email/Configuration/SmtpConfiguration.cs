using System;

namespace OpenIZ.Core.Security.Tfa.Email.Configuration
{
    /// <summary>
    /// Configuration for SMTP
    /// </summary>
    public class SmtpConfiguration
    {

        /// <summary>
        /// SMTP configuration
        /// </summary>
        public SmtpConfiguration(Uri server, String userName, String password, bool ssl)
        {
            this.Server = server;
            this.Username = userName;
            this.Password = password;
            this.Ssl = ssl;
        }

        /// <summary>
        /// Gets the SMTP server
        /// </summary>
        public Uri Server { get; private set; }

        /// <summary>
        /// Gets the username for connecting to the server
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Gets the password
        /// </summary>
        public string Password { get; private set; }
        /// <summary>
        /// Get the SSL setting
        /// </summary>
        public bool Ssl { get; private set; }
    }
}