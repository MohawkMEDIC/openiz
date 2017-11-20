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
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Auth
{
	/// <summary>
	/// Wrapper for policy information
	/// </summary>
	[XmlType(nameof(SecurityPolicyInfo), Namespace = "http://openiz.org/ami")]
	[XmlRoot(nameof(SecurityPolicyInfo), Namespace = "http://openiz.org/ami")]
	public class SecurityPolicyInfo
	{
		/// <summary>
		/// Policy information
		/// </summary>
		public SecurityPolicyInfo()
		{
		}

		/// <summary>
		/// Constructs this policy information object from an IMS policy instane
		/// </summary>
		public SecurityPolicyInfo(SecurityPolicy o)
		{
			this.Name = o.Name;
			this.Oid = o.Oid;
			this.CanOverride = o.CanOverride;
			this.Policy = o;
		}

		/// <summary>
		/// Constructs this policy information object from an IMS policy instane
		/// </summary>
		public SecurityPolicyInfo(SecurityPolicyInstance o) : this(o.Policy)
		{
			this.Grant = o.GrantType;
		}

		/// <summary>
		/// True if the policy can be overridden
		/// </summary>
		[XmlAttribute("canOverride")]
		public bool CanOverride { get; set; }

		/// <summary>
		/// The outcome grant if an instance
		/// </summary>
		[XmlAttribute("grant")]
		public PolicyGrantType Grant { get; set; }

		/// <summary>
		/// The name of the policy
		/// </summary>
		[XmlAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		/// The OID of the policy
		/// </summary>
		[XmlAttribute("oid")]
		public string Oid { get; set; }

		/// <summary>
		/// Gets or sets the policy information
		/// </summary>
		[XmlElement("policyInfo")]
		public SecurityPolicy Policy { get; set; }
	}
}