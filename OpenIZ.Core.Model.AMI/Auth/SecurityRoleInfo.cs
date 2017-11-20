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
 * Date: 2016-8-2
 */
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Auth
{
	/// <summary>
	/// Represents security role information.
	/// </summary>
	[XmlRoot(nameof(SecurityRoleInfo), Namespace = "http://openiz.org/ami")]
	[XmlType(nameof(SecurityRoleInfo), Namespace = "http://openiz.org/ami")]
	public class SecurityRoleInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Core.Model.AMI.Auth.SecurityRoleInfo"/> class.
		/// </summary>
		public SecurityRoleInfo()
		{
			this.Policies = new List<SecurityPolicyInfo>();
		}

		/// <summary>
		/// Creates a new security role information object from the specified security role.
		/// </summary>
		/// <param name="role">The security role.</param>
		public SecurityRoleInfo(SecurityRole role)
		{
			this.Id = role.Key;
			this.Name = role.Name;
			this.Role = role;
			this.Policies = role.Policies.Select(o => new SecurityPolicyInfo(o)).ToList();
		}

		/// <summary>
		/// Gets or sets the identifier of the message
		/// </summary>
		[XmlElement("id")]
		public Guid? Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the group
		/// </summary>
		[XmlAttribute("name")]
		public String Name { get; set; }

		/// <summary>
		/// Represents policies
		/// </summary>
		[XmlElement("policy")]
		public List<SecurityPolicyInfo> Policies { get; set; }

		/// <summary>
		/// Gets or sets the role information
		/// </summary>
		[XmlElement("roleInfo")]
		public SecurityRole Role { get; set; }
	}
}