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
 * Date: 2017-1-7
 */
using System.Security.Cryptography.X509Certificates;

namespace OpenIZ.Reporting.Core
{
	/// <summary>
	/// Represents a service which supports certificate based authentication.
	/// </summary>
	public interface ISupportCertificateAuthentication : IAuthenticationHandler
	{
		/// <summary>
		/// Authenticates against a remote system using a certificate.
		/// </summary>
		/// <param name="certificate">The certificate to use to authenticate.</param>
		void Authenticate(X509Certificate2 certificate);
	}
}