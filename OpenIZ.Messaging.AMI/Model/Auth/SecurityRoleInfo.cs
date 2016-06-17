using OpenIZ.Core.Model.Security;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.AMI.Model.Auth
{
    /// <summary>
    /// Security role information
    /// </summary>
    [XmlRoot(nameof(SecurityRoleInfo), Namespace = "http://openiz.org/ami")]
    [XmlType(nameof(SecurityRoleInfo), Namespace = "http://openiz.org/ami")]
    public class SecurityRoleInfo
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public SecurityRoleInfo()
        {

        }

        /// <summary>
        /// Creates a new security role information object from the specified security role
        /// </summary>
        /// <param name="r"></param>
        public SecurityRoleInfo(SecurityRole r)
        {
            this.Id = r.Key;
            this.Name = r.Name;
            this.Role = r;
        }

        /// <summary>
        /// Gets or sets the identifier of the message
        /// </summary>
        [XmlAttribute("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the group
        /// </summary>
        [XmlAttribute("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the role information
        /// </summary>
        [XmlElement("roleInfo")]
        public SecurityRole Role { get; set; }
    }
}