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
 * Date: 2016-11-20
 */
using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.EntityLoader;
using System.Globalization;
using Newtonsoft.Json;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Core.Model.Security
{
    /// <summary>
    /// Security user represents a user for the purpose of security 
    /// </summary>
    [XmlType("SecurityUser", Namespace = "http://openiz.org/model"), JsonObject("SecurityUser")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "SecurityUser")]
    [KeyLookup(nameof(UserName))]
    public class SecurityUser : SecurityEntity
    {
        /// <summary>
        /// Roles belonging to the user
        /// </summary>
        public SecurityUser()
        {
            this.Roles = new List<SecurityRole>();
        }

        /// <summary>
        /// Gets or sets the email address of the user
        /// </summary>
        [XmlElement("email"), JsonProperty("email")]
        public String Email { get; set; }
        /// <summary>
        /// Gets or sets whether the email address is confirmed
        /// </summary>
        [XmlElement("emailConfirmed"), JsonProperty("emailConfirmed")]
        public Boolean EmailConfirmed { get; set; }
        /// <summary>
        /// Gets or sets the number of invalid login attempts by the user
        /// </summary>
        [XmlElement("invalidLoginAttempts"), JsonProperty("invalidLoginAttempts")]
        public Int32 InvalidLoginAttempts { get; set; }
        /// <summary>
        /// Gets or sets whether the account is locked out
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public DateTime? Lockout { get; set; }

        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [XmlElement("lockout"), JsonProperty("lockout")]
        public String LockoutXml
        {
            get { return this.Lockout?.ToString("o", CultureInfo.InvariantCulture); }
            set
            {
                if (value != null)
                    this.Lockout = DateTime.ParseExact(value, "o", CultureInfo.InvariantCulture);
                else
                    this.Lockout = default(DateTime);
            }
        }

        /// <summary>
        /// Gets or sets whether the password hash is enabled
        /// </summary>
        [XmlElement("passwordHash"), JsonProperty("passwordHash")]
        public String PasswordHash { get; set; }
        /// <summary>
        /// Gets or sets whether the security has is enabled
        /// </summary>
        [XmlElement("securityStamp"), JsonProperty("securityStamp")]
        public String SecurityHash { get; set; }
        /// <summary>
        /// Gets or sets whether two factor authentication is required
        /// </summary>
        [XmlElement("twoFactorEnabled"), JsonProperty("twoFactorEnabled")]
        public Boolean TwoFactorEnabled { get; set; }
        /// <summary>
        /// Gets or sets the logical user name ofthe user
        /// </summary>
        [XmlElement("userName"), JsonProperty("userName")]
        public String UserName { get; set; }
        /// <summary>
        /// Gets or sets the binary representation of the user's photo
        /// </summary>
        [XmlElement("photo"), JsonProperty("photo")]
        public byte[] UserPhoto { get; set; }
        

        /// <summary>
        /// The last login time
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public DateTimeOffset? LastLoginTime { get; set; }

        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [XmlElement("lastLoginTime"), JsonProperty("lastLoginTime")]
        public String LastLoginTimeXml
        {
            get { return this.LastLoginTime?.ToString("o", CultureInfo.InvariantCulture); }
            set
            {
                if (value != null)
                    this.LastLoginTime = DateTimeOffset.ParseExact(value, "o", CultureInfo.InvariantCulture);
                else
                    this.LastLoginTime = default(DateTimeOffset);
            }
        }

        /// <summary>
        /// Represents roles
        /// </summary>
        [XmlIgnore, JsonIgnore]

        public List<SecurityRole> Roles { get; set; }

        /// <summary>
        /// Gets or sets the patient's phone number
        /// </summary>
        [XmlElement("phoneNumber"), JsonProperty("phoneNumber")]
        public String PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets whether the phone number was confirmed
        /// </summary>
        [XmlElement("phoneNumberConfirmed"), JsonProperty("phoneNumberConfirmed")]
        public Boolean PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the user class key
        /// </summary>
        [XmlElement("userClass"), JsonProperty("userClass")]
        [Binding(typeof(UserClassKeys))]
        public Guid UserClass { get; set; }

        /// <summary>
        /// Gets the etag
        /// </summary>
        public override string Tag
        {
            get
            {
                return this.SecurityHash;
            }
        }

        /// <summary>
        /// Gets or sets the policies for the user
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public override List<SecurityPolicyInstance> Policies
        {
            get
            {
                return this.Roles.SelectMany(o => o.Policies).ToList();
            }

            set
            {
                
            }
        }

        /// <summary>
        /// Determine semantic equality of user
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as SecurityUser;
            if (other == null) return false;
            return base.SemanticEquals(obj) &&
                this.Email == other.Email &&
                this.EmailConfirmed == other.EmailConfirmed &&
                this.PasswordHash == other.PasswordHash &&
                this.PhoneNumber == other.PhoneNumber &&
                this.PhoneNumberConfirmed == other.PhoneNumberConfirmed &&
                this.Policies?.SemanticEquals(other.Policies) == true &&
                this.Roles?.SemanticEquals(other.Roles) == true &&
                this.SecurityHash == other.SecurityHash &&
                this.TwoFactorEnabled == other.TwoFactorEnabled &&
                this.UserClass == other.UserClass &&
                this.UserName == other.UserName &&
                (this.UserPhoto ?? new byte[0]).HashCode().Equals((other.UserPhoto?? new byte[0]).HashCode()) == true;
        }
    }
}
