/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-2-1
 */
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenIZ.Core.Model.EntityLoader;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a device entity
    /// </summary>
    
    [XmlType("DeviceEntity",  Namespace = "http://openiz.org/model"), JsonObject("DeviceEntity")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "DeviceEntity")]
    public class DeviceEntity : Entity
    {

        /// <summary>
        /// Device entity ctor
        /// </summary>
        public DeviceEntity()
        {
            this.DeterminerConceptKey = DeterminerKeys.Specific;
            this.ClassConceptKey = EntityClassKeys.Device;
        }

        /// <summary>
        /// Gets or sets the security device key
        /// </summary>
        [DataIgnore, XmlElement("securityDevice"), JsonProperty("securityDevice")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? SecurityDeviceKey
        {
            get { return this.SecurityDevice?.Key; }
            set
            {
                if (this.SecurityDevice?.Key != value)
                    this.SecurityDevice = this.EntityProvider.Get<SecurityDevice>(value);
            }
        }

        /// <summary>
        /// Gets or sets the security device
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(SecurityDeviceKey))]
		public SecurityDevice SecurityDevice { get; set; }
        
        /// <summary>
        /// Gets or sets the manufacturer model name
        /// </summary>
        [XmlElement("manufacturerModelName"), JsonProperty("manufacturerModelName")]
        public String ManufacturedModelName { get; set; }
        /// <summary>
        /// Gets or sets the operating system name
        /// </summary>
        [XmlElement("operatingSystemName"), JsonProperty("operatingSystemName")]
        public String OperatingSystemName { get; set; }


    }
}
