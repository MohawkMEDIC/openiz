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
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.HL7.Configuration
{
	/// <summary>
	/// Represents a target configuration.
	/// </summary>
	public class TargetConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TargetConfiguration" /> class
		/// with a specific name, connection string, act as identifier, and device id.
		/// </summary>
		/// <param name="name">The name of the target configuration.</param>
		/// <param name="connectionString">The connection string of the target configuration.</param>
		/// <param name="actor">The act as identifier of the target configuration.</param>
		/// <param name="deviceId">The device id of the configuration.</param>
		/// <exception cref="System.Configuration.ConfigurationErrorsException">
		/// </exception>
		/// <exception cref="System.InvalidOperationException"></exception>
		public TargetConfiguration(string name, string connectionString, string actor, string deviceId)
		{
			this.Name = name;
			this.ConnectionString = connectionString;
			this.DeviceId = deviceId;
			this.NotificationDomainConfigurations = new List<NotificationDomainConfiguration>();

			var notifierType = Array.Find(typeof(TargetConfiguration).Assembly.GetExportedTypes(), t => t.Name == actor);

			if (notifierType == null)
			{
				throw new ConfigurationErrorsException($"Could not find the specified actor implementation {actor}");
			}

			var constructorInfo = notifierType.GetConstructor(Type.EmptyTypes);

			if (constructorInfo == null)
			{
				throw new ConfigurationErrorsException($"Could not find the specified actor implementation {actor}");
			}

			this.Notifier = constructorInfo.Invoke(null) as INotifier;

			if (this.Notifier == null)
			{
				throw new InvalidOperationException($"Unable to create instance of {actor}");
			}

			this.Notifier.TargetConfiguration = this;
		}

		/// <summary>
		/// Gets the actor of the configuration.
		/// </summary>
		[XmlAttribute("myActor")]
		public string Actor { get; }

		/// <summary>
		/// Gets the connection string of the target configuration.
		/// </summary>
		[XmlAttribute("connectionString")]
		public string ConnectionString { get; }

		/// <summary>
		/// Gets the device id of the target configuration.
		/// </summary>
		[XmlAttribute("deviceId")]
		public string DeviceId { get; }

		/// <summary>
		/// Gets the LLP client certificate.
		/// </summary>
		[XmlIgnore]
		public X509Certificate2 LlpClientCertificate { get; internal set; }

		/// <summary>
		/// Gets the location to scan of the server certificate.
		/// </summary>
		[XmlIgnore]
		public StoreLocation LlpClientCertLocation { get; internal set; }

		/// <summary>
		/// Gets the name of the server certificate store.
		/// </summary>
		[XmlIgnore]
		public StoreName LlpClientCertStore { get; internal set; }

		/// <summary>
		/// Gets the name of the target configuration.
		/// </summary>
		[XmlAttribute("name")]
		public string Name { get; }

		/// <summary>
		/// Gets or sets the list of notification domain configurations.
		/// </summary>
		[XmlElement("notify")]
		public List<NotificationDomainConfiguration> NotificationDomainConfigurations { get; set; }

		/// <summary>
		/// Gets the notifier.
		/// </summary>
		public INotifier Notifier { get; }

		/// <summary>
		/// Gets the server certificate.
		/// </summary>
		[XmlIgnore]
		public X509Certificate2 TrustedIssuerCertificate { get; internal set; }

		/// <summary>
		/// Gets the location to scan of the server certificate.
		/// </summary>
		[XmlIgnore]
		public StoreLocation TrustedIssuerCertLocation { get; internal set; }

		/// <summary>
		/// Gets the name of the server certificate store.
		/// </summary>
		[XmlIgnore]
		public StoreName TrustedIssuerCertStore { get; internal set; }
	}
}