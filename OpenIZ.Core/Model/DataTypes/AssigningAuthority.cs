using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.Attributes;
using System;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a model class which is an assigning authority
    /// </summary>
    public  class AssigningAuthority : BaseEntityData
    {
        // Assigning device id
        private Guid m_assigningDeviceId;

        // TODO: Change this to SecurityDevice
        private Object m_assigningDevice;

        /// <summary>
        /// Gets or sets the name of the assigning authority
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Gets or sets the domain name of the assigning authority
        /// </summary>
        public String DomainName { get; set; }
        /// <summary>
        /// Gets or sets the description of the assigning authority
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// Gets or sets the oid of the assigning authority
        /// </summary>
        public String Oid { get; set; }
        /// <summary>
        /// The URL of the assigning authority
        /// </summary>
        public String Url { get; set; }
        /// <summary>
        /// Assigning device identifier
        /// </summary>
        public Guid AssigningDeviceId
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
        public Object AssigningDevice { get; set; }

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