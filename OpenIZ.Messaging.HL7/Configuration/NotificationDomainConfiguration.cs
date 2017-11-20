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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.HL7.Configuration
{
	/// <summary>
	/// Represents a notification domain configuration.
	/// </summary>
	public class NotificationDomainConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationDomainConfiguration"/> class.
		/// </summary>
		public NotificationDomainConfiguration()
		{
			this.ActionConfigurations = new List<ActionConfiguration>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationDomainConfiguration"/> class
		/// with a specific notification domain.
		/// </summary>
		/// <param name="domain">The notification domain.</param>
		public NotificationDomainConfiguration(string domain) : this()
		{
			this.Domain = domain;
		}

		/// <summary>
		/// Gets or sets a list of action configurations of the notification domain configuration.
		/// </summary>
		[XmlElement("action")]
		public List<ActionConfiguration> ActionConfigurations { get; set; }

		/// <summary>
		/// Gets or sets the domain of the notification domain configuration.
		/// </summary>
		[XmlAttribute("domain")]
		public string Domain { get; set; }

		/// <summary>
		/// Returns true of the notification domain should be applied for the specific type.
		/// </summary>
		/// <param name="type">The type of the notification.</param>
		/// <returns>Returns true of the notification domain should be applied for the specific type.</returns>
		public bool IsApplicableFor(ActionType type)
		{
			return this.ActionConfigurations.Any(a => a.ActionType == type);
		}
	}
}