/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-11-30
 */
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
        public String From { get; private set; }

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