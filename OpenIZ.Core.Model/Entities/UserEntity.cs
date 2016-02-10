using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // Security user key
        private Guid m_securityUserKey;
        // Security user
        private SecurityUser m_securityUser;

        /// <summary>
        /// Gets or sets the security user key
        /// </summary>
        [XmlElement("securityUser"), JsonProperty("securityUser")]
        public Guid SecurityUserKey
        {
            get
            {
                return this.m_securityUserKey;
            }
            set
            {
                this.m_securityUserKey = value;
                this.m_securityUser = null;
            }
        }

        /// <summary>
        /// Gets or sets the security user key
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [DelayLoad(nameof(SecurityUserKey))]
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
                this.m_securityUserKey = value.Key;
            }
        }

    }
}
