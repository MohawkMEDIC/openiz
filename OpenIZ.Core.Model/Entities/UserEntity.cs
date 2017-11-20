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
 * Date: 2016-7-16
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Security;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
	/// <summary>
	/// Represents a user entity
	/// </summary>
	[XmlType("UserEntity", Namespace = "http://openiz.org/model"), JsonObject("UserEntity")]
	[XmlRoot(Namespace = "http://openiz.org/model", ElementName = "UserEntity")]
	public class UserEntity : Person
	{
		// Security user
		private SecurityUser m_securityUser;

		// Security user key
		private Guid? m_securityUserKey;

		/// <summary>
		/// Gets or sets the security user key
		/// </summary>
		[XmlIgnore, JsonIgnore, DataIgnore]
		[SerializationReference(nameof(SecurityUserKey))]
		public SecurityUser SecurityUser
		{
			get
			{
				if (this.IsDelayLoadEnabled)
					this.m_securityUser = base.DelayLoad(this.m_securityUserKey, this.m_securityUser);
				return this.m_securityUser;
			}
			set
			{
				this.m_securityUser = value;
				this.m_securityUserKey = value?.Key;
			}
		}

		/// <summary>
		/// Gets or sets the security user key
		/// </summary>
		[XmlElement("securityUser"), JsonProperty("securityUser")]
		public Guid? SecurityUserKey
		{
			get
			{
				return this.m_securityUserKey;
			}
			set
			{
				if (this.m_securityUserKey != value)
				{
					this.m_securityUserKey = value;
					this.m_securityUser = null;
				}
			}
		}

		/// <summary>
		/// Determine semantic equality
		/// </summary>
		public override bool SemanticEquals(object obj)
		{
			var other = obj as UserEntity;
			if (other == null) return false;
			return base.SemanticEquals(obj) &&
				this.SecurityUserKey == other.SecurityUserKey;
		}
	}
}