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
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Security;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
	/// <summary>
	/// Represents a device entity
	/// </summary>

	[XmlType("DeviceEntity", Namespace = "http://openiz.org/model"), JsonObject("DeviceEntity")]
	[XmlRoot(Namespace = "http://openiz.org/model", ElementName = "DeviceEntity")]
	public class DeviceEntity : Entity
	{
		// Security device
		private SecurityDevice m_securityDevice;

		// Security device key
		private Guid? m_securityDeviceKey;

		/// <summary>
		/// Device entity ctor
		/// </summary>
		public DeviceEntity()
		{
			this.DeterminerConceptKey = DeterminerKeys.Specific;
			this.ClassConceptKey = EntityClassKeys.Device;
		}

		/// <summary>
		/// Gets or sets the manufacturer model name
		/// </summary>
		[XmlElement("manufacturerModelName"), JsonProperty("manufacturerModelName")]
		public String ManufacturerModelName { get; set; }

		/// <summary>
		/// Gets or sets the operating system name
		/// </summary>
		[XmlElement("operatingSystemName"), JsonProperty("operatingSystemName")]
		public String OperatingSystemName { get; set; }

		/// <summary>
		/// Gets or sets the security device
		/// </summary>
		[SerializationReference(nameof(SecurityDeviceKey))]
		[XmlIgnore, JsonIgnore]
		public SecurityDevice SecurityDevice
		{
			get
			{
				this.m_securityDevice = base.DelayLoad(this.m_securityDeviceKey, this.m_securityDevice);
				return this.m_securityDevice;
			}
			set
			{
				this.m_securityDevice = value;
				this.m_securityDeviceKey = value?.Key;
			}
		}

		/// <summary>
		/// Gets or sets the security device key
		/// </summary>
		[XmlElement("securityDevice"), JsonProperty("securityDevice")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Guid? SecurityDeviceKey
		{
			get { return this.m_securityDeviceKey; }
			set
			{
				if (this.m_securityDeviceKey != value)
				{
					this.m_securityDeviceKey = value;
					this.m_securityDevice = null;
				}
			}
		}

		/// <summary>
		/// Force refresh of data model
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();
			this.m_securityDevice = null;
		}

		/// <summary>
		/// Determine semantic equality
		/// </summary>
		public override bool SemanticEquals(object obj)
		{
			var other = obj as DeviceEntity;
			if (other == null) return false;
			return base.SemanticEquals(obj) &&
				this.SecurityDeviceKey == other.SecurityDeviceKey &&
				this.ManufacturerModelName == other.ManufacturerModelName &&
				this.OperatingSystemName == other.OperatingSystemName;
		}
	}
}