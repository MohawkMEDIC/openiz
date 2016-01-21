using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Security
{
    /// <summary>
    /// Represents a security device
    /// </summary>
    [Serializable]
    [DataContract(Name = "SecurityDevice", Namespace = "http://openiz.org/model")]
    public class SecurityDevice : SecurityEntity
    {

        /// <summary>
        /// Gets or sets the device secret
        /// </summary>
        [DataMember(Name = "deviceSecret")]
        public String DeviceSecret { get; set; }


    }
}
