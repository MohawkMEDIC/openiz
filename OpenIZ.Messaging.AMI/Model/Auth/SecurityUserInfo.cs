using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.AMI.Model.Auth
{
    /// <summary>
    /// Gets or sets security user information
    /// </summary>
    [XmlType(nameof(SecurityUserInfo), Namespace = "http://openiz.org/ami")]
    [XmlRoot(nameof(SecurityUserInfo), Namespace = "http://openiz.org/ami")]
    public class SecurityUserInfo
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SecurityUserInfo()
        {

        }

        /// <summary>
        /// Creates a new security user from the specified info
        /// </summary>
        /// <param name="u"></param>
        public SecurityUserInfo(SecurityUser u)
        {
            this.UserId = u.Key;
            this.UserName = u.UserName;
            this.Email = u.Email;
            this.Lockout = u.Lockout != null;
            this.Roles = u.Roles.Select(o => new SecurityRoleInfo(o)).ToList();
            this.User = u;
        }

        /// <summary>
        /// Gets the user identifier
        /// </summary>
        [XmlAttribute("id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the user
        /// </summary>
        [XmlAttribute("name")]
        public String UserName { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        [XmlElement("password")]
        public String Password { get; set; }

        /// <summary>
        /// E-mail address
        /// </summary>
        [XmlElement("email")]
        public String Email { get; set; }

        /// <summary>
        /// Lockout
        /// </summary>
        [XmlAttribute("lockout")]
        public bool Lockout { get; set; }

        /// <summary>
        /// Roles
        /// </summary>
        [XmlElement("role")]
        public List<SecurityRoleInfo> Roles { get; set; }

        /// <summary>
        /// Security user object
        /// </summary>
        [XmlElement("userInfo")]
        public SecurityUser User { get; set; }
    }
}
