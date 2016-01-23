using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Security
{
    /// <summary>
    /// Represents a security device
    /// </summary>
    
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "SecurityDevice")]
    [XmlType("SecurityDevice", Namespace = "http://openiz.org/model")]
    public class SecurityDevice : SecurityEntity
    {

        /// <summary>
        /// Gets or sets the device secret
        /// </summary>
        [XmlElement("deviceSecret")]
        public String DeviceSecret { get; set; }


    }
}
