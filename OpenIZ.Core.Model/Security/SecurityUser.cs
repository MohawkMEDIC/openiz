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

namespace OpenIZ.Core.Model.Security
{
    /// <summary>
    /// Security user represents a user for the purpose of security 
    /// </summary>
    [XmlType("SecurityUser",  Namespace = "http://openiz.org/model"), JsonObject("SecurityUser")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "SecurityUser")]
    [KeyLookup(nameof(UserName))]
    public class SecurityUser : SecurityEntity
    {

        
        // Roles
        private List<SecurityRole> m_roles;
        // User entities
        private List<Person> m_userEntities;

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
        /// Gets the list of entities associated with this security user
        /// </summary>
        [XmlIgnore,JsonIgnore]
        public List<Person> Entities
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_userEntities = EntitySource.Current.Provider.Query<UserEntity>(o => o.SecurityUserKey == this.Key && o.ObsoletionTime == null).OfType<Person>().ToList();
                return this.m_userEntities;
            }
            set
            {
                this.m_userEntities = value;
            }
        }

        /// <summary>
        /// Concepts as identifiers for XML purposes only
        /// </summary>
        [XmlElement("entity"), JsonProperty("entity")]
        
        public List<Guid> EntitiesXml
        {
            get
            {
                return this.Entities?.Where(o=>o.Key.HasValue).Select(o => o.Key.Value).ToList();
            }
            set
            {
                this.Entities = new List<Person>(value.Select(o => new Person() { Key = o }));
                ; // nothing
            }
        }

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
        
        public List<SecurityRole> Roles {
            get
            {
                if(this.IsDelayLoadEnabled && this.m_roles == null)
                    this.m_roles = EntitySource.Current.Provider.Query<SecurityRole>(r => r.Users.Any(u => u.Key == this.Key)).ToList();
                return this.m_roles;
            }
            set
            {
                this.m_roles = value;
            }
        }
      
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
        public Guid UserClass { get; set; }

        /// <summary>
        /// Forces delay load properties to be from the database
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_roles = null;
        }

    }
}
