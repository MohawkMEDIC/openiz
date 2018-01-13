using System;

namespace OpenIZ.Persistence.Diagnostics.Email.Configuration
{
	/// <summary>
	/// Configuration for SMTP
	/// </summary>
	public class SmtpConfiguration
	{
		/// <summary>
		/// SMTP configuration
		/// </summary>
		public SmtpConfiguration(Uri server, String userName, String password, bool ssl, String from)
		{
			this.Server = server;
			this.Username = userName;
			this.Password = password;
			this.Ssl = ssl;
            this.From = from;
		}

        /// <summary>
        /// Gets the from address
        /// </summary>
        public string From { get; private set; }

        /// <summary>
        /// Gets the password
        /// </summary>
        public string Password { get; private set; }

		/// <summary>
		/// Gets the SMTP server
		/// </summary>
		public Uri Server { get; private set; }

		/// <summary>
		/// Get the SSL setting
		/// </summary>
		public bool Ssl { get; private set; }

		/// <summary>
		/// Gets the username for connecting to the server
		/// </summary>
		public string Username { get; private set; }
	}
}