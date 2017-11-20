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
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Auth
{
	/// <summary>
	/// Represents a wrapper for the <see cref="SecurityApplication"/> class.
	/// </summary>
	[XmlType(nameof(SecurityApplicationInfo), Namespace = "http://openiz.org/ami")]
	[XmlRoot(nameof(SecurityApplicationInfo), Namespace = "http://openiz.org/ami")]
	public class SecurityApplicationInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityApplicationInfo"/> class.
		/// </summary>
		public SecurityApplicationInfo()
		{
			this.Policies = new List<SecurityPolicyInfo>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityApplicationInfo"/> class
		/// with a specific <see cref="SecurityApplication"/> instance.
		/// </summary>
		/// <param name="application">The security application.</param>
		public SecurityApplicationInfo(SecurityApplication application)
		{
			this.Application = application;
			this.ApplicationSecret = application.ApplicationSecret;
			this.Id = application.Key;
			this.Name = application.Name;
			this.Policies = application.Policies.Select(p => new SecurityPolicyInfo(p)).ToList();
		}

		/// <summary>
		/// Gets or sets the application.
		/// </summary>
		[XmlElement("application")]
		public SecurityApplication Application { get; set; }

		/// <summary>
		/// Gets or sets the id of the application.
		/// </summary>
		[XmlElement("id")]
		public Guid? Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the application.
		/// </summary>
		[XmlElement("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the application secret of the application.
		/// </summary>
		[XmlElement("applicationSecret")]
		public string ApplicationSecret { get; set; }

		/// <summary>
		/// Gets or sets the policies associated with the application.
		/// </summary>
		[XmlElement("policy")]
		public List<SecurityPolicyInfo> Policies { get; set; }
	}
}