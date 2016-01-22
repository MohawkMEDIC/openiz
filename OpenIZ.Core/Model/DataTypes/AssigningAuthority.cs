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
 * Date: 2016-1-19
 */
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Security;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a model class which is an assigning authority
    /// </summary>
    [Serializable]
    [XmlType("AssigningAuthority", Namespace = "http://openiz.org/model")]
    public  class AssigningAuthority : BaseEntityData
    {
        // Assigning device id
        private Guid? m_assigningDeviceId;

        // TODO: Change this to SecurityDevice
        private SecurityDevice m_assigningDevice;

        /// <summary>
        /// Gets or sets the name of the assigning authority
        /// </summary>
        [XmlElement("name")]
        public String Name { get; set; }
        /// <summary>
        /// Gets or sets the domain name of the assigning authority
        /// </summary>
        [XmlElement("domainName")]
        public String DomainName { get; set; }
        /// <summary>
        /// Gets or sets the description of the assigning authority
        /// </summary>
        [XmlElement("description")]
        public String Description { get; set; }
        /// <summary>
        /// Gets or sets the oid of the assigning authority
        /// </summary>
        [XmlElement("oid")]
        public String Oid { get; set; }
        /// <summary>
        /// The URL of the assigning authority
        /// </summary>
        [XmlElement("url")]
        public String Url { get; set; }
        /// <summary>
        /// Assigning device identifier
        /// </summary>
        [XmlElement("assigningDevice")]
        public Guid? AssigningDeviceKey
        {
            get { return this.m_assigningDeviceId; }
            set
            {
                this.m_assigningDeviceId = value;
                this.m_assigningDevice = null;
            }
        }
        
        /// <summary>
        /// Gets or sets the assigning device
        /// </summary>
        [XmlIgnore]
        public SecurityDevice AssigningDevice {
            get
            {
                this.m_assigningDevice = base.DelayLoad(this.m_assigningDeviceId, this.m_assigningDevice);
                return this.m_assigningDevice;
            }
            set
            {
                this.m_assigningDevice = value;
                this.m_assigningDeviceId = value?.Key;
            }
        }

        /// <summary>
        /// Convert this AA to OID Data for configuration purposes
        /// </summary>
        public OidData ToOidData()
        {
            return new OidData()
            {
                Name = this.Name,
                Description = this.Description,
                Oid = this.Oid,
                Ref = new Uri(String.IsNullOrEmpty(this.Url) ? String.Format("urn:uuid:{0}", this.Oid) : this.Url),
                Attributes = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>>()
                {
                    new System.Collections.Generic.KeyValuePair<string, string>("HL7CX4", this.DomainName)
                    //new System.Collections.Generic.KeyValuePair<string, string>("AssigningDevFacility", this.AssigningDevice.DeviceEvidence)
                }
            };
        }

        /// <summary>
        /// Force reloading of delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_assigningDevice = null;
        }
    }
}