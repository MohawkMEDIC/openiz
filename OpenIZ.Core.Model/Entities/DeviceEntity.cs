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

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a device entity
    /// </summary>
    
    [XmlType("DeviceEntity", Namespace = "http://openiz.org/model")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "DeviceEntity")]
    public class DeviceEntity : Entity
    {

        // Security device key
        private Guid m_securityDeviceKey;
        // Security device
        private SecurityDevice m_securityDevice;

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
        [XmlElement("securityDevice")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid SecurityDeviceKey
        {
            get { return this.m_securityDeviceKey; }
            set
            {
                this.m_securityDeviceKey = value;
                this.m_securityDevice = null;
            }
        }

        /// <summary>
        /// Gets or sets the security device
        /// </summary>
        [DelayLoad(nameof(SecurityDeviceKey))]
        [XmlIgnore]
        public SecurityDevice SecurityDevice
        {
            get {
                this.m_securityDevice = base.DelayLoad(this.m_securityDeviceKey, this.m_securityDevice);
                return this.m_securityDevice;
            }
            set
            {
                this.m_securityDevice = value;
                if (value == null)
                    this.m_securityDeviceKey = Guid.Empty;
                else
                    this.m_securityDeviceKey = value.Key;
            }
        }
        /// <summary>
        /// Gets or sets the manufacturer model name
        /// </summary>
        [XmlElement("manufacturerModelName")]
        public String ManufacturedModelName { get; set; }
        /// <summary>
        /// Gets or sets the operating system name
        /// </summary>
        [XmlElement("operatingSystemName")]
        public String OperatingSystemName { get; set; }

        /// <summary>
        /// Force refresh of data model
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_securityDevice = null;
        }
    }
}
