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
	/// Gets or sets security user information
	/// </summary>
	[XmlType(nameof(SecurityUserInfo), Namespace = "http://openiz.org/ami")]
	[XmlRoot(nameof(SecurityUserInfo), Namespace = "http://openiz.org/ami")]
	public class SecurityUserInfo
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public SecurityUserInfo()
		{
			this.Roles = new List<SecurityRoleInfo>();
		}

		/// <summary>
		/// Creates a new security user from the specified info
		/// </summary>
		/// <param name="u"></param>
		public SecurityUserInfo(SecurityUser u)
		{
			this.UserId = u.Key;
			this.UserName = u.UserName;
			this.Email = u.Email;

			DateTime lockout;

			if (DateTime.TryParse(u.LockoutXml, out lockout))
			{
				this.Lockout = lockout >= DateTime.Now;
			}

			this.Roles = u.Roles.Select(o => new SecurityRoleInfo(o)).ToList();
			this.User = u;
		}

		/// <summary>
		/// E-mail address
		/// </summary>
		[XmlElement("email")]
		public String Email { get; set; }

		/// <summary>
		/// Lockout
		/// </summary>
		[XmlElement("lockout")]
		public bool? Lockout { get; set; }

		/// <summary>
		/// User password
		/// </summary>
		[XmlElement("password")]
		public String Password { get; set; }

		/// <summary>
		/// Roles
		/// </summary>
		[XmlElement("role")]
		public List<SecurityRoleInfo> Roles { get; set; }

		/// <summary>
		/// Security user object
		/// </summary>
		[XmlElement("userInfo")]
		public SecurityUser User { get; set; }

		/// <summary>
		/// Gets the user identifier
		/// </summary>
		[XmlElement("id")]
		public Guid? UserId { get; set; }

		/// <summary>
		/// Gets or sets the name of the user
		/// </summary>
		[XmlElement("name")]
		public String UserName { get; set; }
	}
}