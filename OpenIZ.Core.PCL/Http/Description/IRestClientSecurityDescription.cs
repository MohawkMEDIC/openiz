/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-18
 */
namespace OpenIZ.Core.Http.Description
{
	/// <summary>
	/// Represtens REST client security description
	/// </summary>
	public interface IRestClientSecurityDescription
	{
		/// <summary>
		/// Gets the certificate validator
		/// </summary>
		ICertificateValidator CertificateValidator { get; }

		/// <summary>
		/// Gets the credential provider
		/// </summary>
		ICredentialProvider CredentialProvider { get; }

		/// <summary>
		/// Gets or sets the mode of security
		/// </summary>
		SecurityScheme Mode { get; }

		/// <summary>
		/// Gets the certificate
		/// </summary>
		IRestClientCertificateDescription ClientCertificate { get; }

		/// <summary>
		/// Gets the authentication realm
		/// </summary>
		string AuthRealm { get; }
        /// <summary>
        /// When true instructs the client to pre-emptively authenticate itself
        /// </summary>
        bool PremptiveAuthentication { get; set; }
    }

	/// <summary>
	/// Security scheme
	/// </summary>
	public enum SecurityScheme
	{
		None = 0,
		Basic = 1,
		Bearer = 2
	}
}