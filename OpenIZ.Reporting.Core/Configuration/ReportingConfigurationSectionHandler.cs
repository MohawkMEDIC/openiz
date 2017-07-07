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
 * Date: 2017-4-10
 */
using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace OpenIZ.Reporting.Core.Configuration
{
	/// <summary>
	/// Represents a configuration section handler for the RISI configuration.
	/// </summary>
	public class ReportingConfigurationSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Creates the configuration section.
		/// </summary>
		/// <param name="parent">The parent configuration section.</param>
		/// <param name="configContext">The configuration context.</param>
		/// <param name="section">The configuration section.</param>
		/// <returns>Returns an instance of the configuration section.</returns>
		/// <exception cref="ConfigurationErrorsException">
		/// The 'reportEngine' element must have a 'type' attribute
		/// or
		/// The 'address' attribute cannot be null
		/// or
		/// The 'address' attribute must be a well formed URI
		/// or
		/// The handler type must be a class and non-abstract
		/// or
		/// The handler type must implement <see cref="IReportExecutor"/>
		/// or
		/// The 'credentials' element must exist
		/// or
		/// The 'type' attribute on 'credential' cannot be null
		/// or
		/// The 'credential' element must exist
		/// or
		/// </exception>
		public object Create(object parent, object configContext, XmlNode section)
		{
			var configuration = new ReportingConfiguration();

			var reportEngineElement = section.SelectSingleNode("./*[local-name() = 'reportEngine']") as XmlElement;

			var type = reportEngineElement?.Attributes["type"]?.Value;

			if (type == null)
			{
				throw new ConfigurationErrorsException("The 'reportEngine' element must have a 'type' attribute");
			}

			var address = reportEngineElement.Attributes["address"]?.Value;

			if (address == null)
			{
				throw new ConfigurationErrorsException("The 'address' attribute cannot be null");
			}

			if (!Uri.IsWellFormedUriString(address, UriKind.Absolute))
			{
				throw new ConfigurationErrorsException("The 'address' attribute must be a well formed URI");
			}

			var handler = Type.GetType(type, true);

			if (!handler.IsClass || handler.IsAbstract)
			{
				throw new ConfigurationErrorsException($"The type { handler.AssemblyQualifiedName } must be a class and non-abstract");
			}

			if (handler.GetInterface(nameof(IReportExecutor)) == null)
			{
				throw new ConfigurationErrorsException($"The type { handler.AssemblyQualifiedName } must implement type { typeof(IReportExecutor).AssemblyQualifiedName }");
			}

			configuration.Address = address;
			configuration.Handler = handler;

			var credentialsElement = section.SelectSingleNode("./*[local-name() = 'credentials']") as XmlElement;

			if (credentialsElement == null)
			{
				throw new ConfigurationErrorsException("The 'credentials' element must exist");
			}

			var credentialType = credentialsElement.Attributes["type"]?.Value;

			if (credentialType == null)
			{
				throw new ConfigurationErrorsException("This 'type' attribute on 'credential' cannot be null");
			}

			var credentialElement = credentialsElement.SelectSingleNode("./*[local-name() = 'credential']") as XmlElement;

			if (credentialElement == null)
			{
				throw new ConfigurationErrorsException("The 'credential' element must exist");
			}

			configuration.Credentials = new Credentials();

			switch (credentialType)
			{
				case "Certificate":
					configuration.Credentials.CredentialType = CredentialType.Certificate;
					configuration.Credentials.Credential = new CertificateCredential((StoreLocation)Enum.Parse(typeof(StoreLocation), credentialElement.Attributes["storeLocation"]?.Value), credentialElement.Attributes["thumbprint"]?.Value);
					break;
				case "UsernamePassword":
					configuration.Credentials.CredentialType = CredentialType.UsernamePassword;
					configuration.Credentials.Credential = new UsernamePasswordCredential(credentialElement.Attributes["username"]?.Value, credentialElement.Attributes["password"]?.Value);
					break;
				default:
					throw new ConfigurationErrorsException($"This 'type' attribute on 'credential' must be one of the following values { Enum.GetValues(typeof(CredentialType)) }");
			}

			return configuration;
		}
	}
}