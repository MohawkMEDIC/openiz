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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.HL7.Configuration
{
	/// <summary>
	/// Represents a notification configuration.
	/// </summary>
	[XmlRoot("targets")]
	public class NotificationConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationConfiguration"/> class.
		/// </summary>
		public NotificationConfiguration()
		{
			this.TargetConfigurations = new List<TargetConfiguration>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationConfiguration"/> class
		/// with a specific concurrency level.
		/// </summary>
		/// <param name="concurrencyLevel">The concurrency level of the notification configuration.</param>
		public NotificationConfiguration(int concurrencyLevel) : this()
		{
			this.ConcurrencyLevel = concurrencyLevel;
		}

		/// <summary>
		/// Gets or sets the concurrency level of the notification configuration.
		/// </summary>
		[XmlAttribute("concurrencyLevel")]
		public int ConcurrencyLevel { get; set; }

		/// <summary>
		/// Gets or sets a list of target configurations.
		/// </summary>
		public List<TargetConfiguration> TargetConfigurations { get; set; }
	}
}