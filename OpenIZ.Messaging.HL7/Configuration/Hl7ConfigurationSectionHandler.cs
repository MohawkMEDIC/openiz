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
 * User: khannan
 * Date: 2016-11-30
 */
using OpenIZ.Messaging.HL7.Notifier;
using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace OpenIZ.Messaging.HL7.Configuration
{
	/// <summary>
	/// Represents a configuration section handler for the HL7 configuration.
	/// </summary>
	public class Hl7ConfigurationSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Creates the configuration section.
		/// </summary>
		/// <param name="parent">The parent configuration section.</param>
		/// <param name="configContext">The configuration context.</param>
		/// <param name="section">The configuration section.</param>
		/// <returns>Returns an instance of the configuration section.</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			var targetsElement = section.SelectNodes("./*[local-name() = 'targets']/*[local-name() = 'add']");

			var notificationConfiguration = new NotificationConfiguration(Environment.ProcessorCount);

			if (targetsElement == null || targetsElement.Count == 0)
			{
				return notificationConfiguration;
			}

			if (section.Attributes["concurrencyLevel"] != null)
			{
				notificationConfiguration.ConcurrencyLevel = int.Parse(section.Attributes["concurrencyLevel"].Value);
			}

			foreach (XmlElement target in targetsElement)
			{
				var connectionString = target.Attributes["connectionString"]?.Value;

				if (connectionString == null)
				{
					throw new ConfigurationErrorsException("Target must have a connection string");
				}

				var name = target.Attributes["name"]?.Value;

				if (name == null)
				{
					throw new ConfigurationErrorsException("Target must have a name");
				}

				var actorType = nameof(PAT_IDENTITY_SRC);

				if (target.Attributes["myActor"] != null)
				{
					actorType = target.Attributes["myActor"]?.Value;
				}

				var deviceId = target.Attributes["deviceId"]?.Value;

				if (deviceId == null)
				{
					throw new ConfigurationErrorsException("Target must have a device id");
				}

				var targetConfiguration = new TargetConfiguration(name, connectionString, actorType, deviceId);

				// Parse certificate data
				var certificateNode = target.SelectSingleNode("./*[local-name() = 'trustedIssuerCertificate']");

				if (certificateNode != null)
				{
					XmlAttribute storeLocationAtt = certificateNode.Attributes["storeLocation"],
								storeNameAtt = certificateNode.Attributes["storeName"],
								findTypeAtt = certificateNode.Attributes["x509FindType"],
								findValueAtt = certificateNode.Attributes["findValue"];

					if (findTypeAtt == null || findValueAtt == null)
					{
						throw new ConfigurationErrorsException("Must supply x509FindType and findValue"); // can't find if nothing to find...
					}

					targetConfiguration.TrustedIssuerCertLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), storeLocationAtt == null ? "LocalMachine" : storeLocationAtt.Value);
					targetConfiguration.TrustedIssuerCertStore = (StoreName)Enum.Parse(typeof(StoreName), storeNameAtt == null ? "My" : storeNameAtt.Value);
					targetConfiguration.TrustedIssuerCertificate = Hl7ConfigurationSectionHandler.FindCertificate(targetConfiguration.TrustedIssuerCertStore, targetConfiguration.TrustedIssuerCertLocation, (X509FindType)Enum.Parse(typeof(X509FindType), findTypeAtt.Value), findValueAtt.Value);
				}

				certificateNode = target.SelectSingleNode("./*[local-name() = 'clientLLPCertificate']");

				if (certificateNode != null)
				{
					XmlAttribute storeLocationAtt = certificateNode.Attributes["storeLocation"],
								storeNameAtt = certificateNode.Attributes["storeName"],
								findTypeAtt = certificateNode.Attributes["x509FindType"],
								findValueAtt = certificateNode.Attributes["findValue"];

					if (findTypeAtt == null || findValueAtt == null)
					{
						throw new ConfigurationErrorsException("Must supply x509FindType and findValue"); // can't find if nothing to find...
					}

					targetConfiguration.LlpClientCertLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), storeLocationAtt == null ? "LocalMachine" : storeLocationAtt.Value);
					targetConfiguration.LlpClientCertStore = (StoreName)Enum.Parse(typeof(StoreName), storeNameAtt == null ? "My" : storeNameAtt.Value);
					targetConfiguration.LlpClientCertificate = Hl7ConfigurationSectionHandler.FindCertificate(targetConfiguration.LlpClientCertStore, targetConfiguration.LlpClientCertLocation, (X509FindType)Enum.Parse(typeof(X509FindType), findTypeAtt.Value), findValueAtt.Value);
				}

				// Get the notification domains and add them to the configuration
				var notificationElements = target.SelectNodes("./*[local-name() = 'notify']");

				foreach (XmlElement notificationElement in notificationElements)
				{
					// Attempt to parse the notification element configuration
					var notificationDomain = string.Empty;

					if (notificationElement.Attributes["domain"] == null)
					{
						throw new ConfigurationErrorsException("Notification element must have a domain");
					}
					else
					{
						notificationDomain = notificationElement.Attributes["domain"].Value;
					}

					var notificationDomainConfiguration = new NotificationDomainConfiguration(notificationDomain);

					// Parse the actions
					var actionsElements = notificationElement.SelectNodes("./*[local-name() = 'action']");
					foreach (XmlElement actionElement in actionsElements)
					{
						// Action types
						var value = ActionType.Any;

						if (actionElement.Attributes["type"] == null)
						{
							throw new ConfigurationErrorsException("Action element must have a type");
						}

						if (!Enum.TryParse(actionElement.Attributes["type"].Value, out value))
						{
							throw new ConfigurationErrorsException($"Invalid action type '{actionElement.Attributes["type"].Value}'");
						}

						notificationDomainConfiguration.ActionConfigurations.Add(new ActionConfiguration(value));
					}

					targetConfiguration.NotificationDomainConfigurations.Add(notificationDomainConfiguration);
				}

				notificationConfiguration.TargetConfigurations.Add(targetConfiguration);
			}

			return notificationConfiguration;
		}

		/// <summary>
		/// Finds a certificate.
		/// </summary>
		/// <param name="storeName">The store name to use to find the certificate.</param>
		/// <param name="storeLocation">The store location to use to find the certificate.</param>
		/// <param name="findType">The find type to use to find the certificate.</param>
		/// <param name="findValue">The find value to use to find the certificate.</param>
		/// <returns>Returns a certificate.</returns>
		private static X509Certificate2 FindCertificate(StoreName storeName, StoreLocation storeLocation, X509FindType findType, string findValue)
		{
			var store = new X509Store(storeName, storeLocation);

			try
			{
				store.Open(OpenFlags.ReadOnly);

				var certs = store.Certificates.Find(findType, findValue, false);

				if (certs.Count > 0)
				{
					return certs[0];
				}
				else
				{
					throw new InvalidOperationException("Cannot locate certificate");
				}
			}
			finally
			{
				store.Close();
			}
		}
	}
}