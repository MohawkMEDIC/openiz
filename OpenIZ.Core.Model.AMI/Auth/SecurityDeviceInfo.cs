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
 * User: khannan
 * Date: 2016-11-9
 */
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Auth
{

	/// <summary>
	/// Represents a wrapper for a <see cref="SecurityDevice"/>.
	/// </summary>
	[XmlType(nameof(SecurityDeviceInfo), Namespace = "http://openiz.org/ami")]
	[XmlRoot(nameof(SecurityDeviceInfo), Namespace = "http://openiz.org/ami")]
	public class SecurityDeviceInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityDeviceInfo"/> class.
		/// </summary>
		public SecurityDeviceInfo()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityDeviceInfo"/> class
		/// with a specific <see cref="SecurityDevice"/> instance.
		/// </summary>
		/// <param name="device">The security device instance.</param>
		public SecurityDeviceInfo(SecurityDevice device)
		{
			this.Id = device.Key;
			this.Name = device.Name;
			this.DeviceSecret = device.DeviceSecret;
			this.Device = device;
		}

		/// <summary>
		/// Gets or sets the security device of the security device info.
		/// </summary>
		[XmlElement("device")]
		public SecurityDevice Device { get; set; }

		/// <summary>
		/// Gets or sets the secret of the device.
		/// </summary>
		[XmlAttribute("deviceSecret")]
		public string DeviceSecret { get; set; }

		/// <summary>
		/// Gets or sets the id of the device.
		/// </summary>
		[XmlElement("id")]
		public Guid? Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the device.
		/// </summary>
		[XmlAttribute("name")]
		public string Name { get; set; }
		/// <summary>
		/// Gets or sets the list of security policies associated with the security device.
		/// </summary>
		[XmlElement("policy")]
		public List<SecurityPolicyInstance> Policies { get; set; }
	}
}
