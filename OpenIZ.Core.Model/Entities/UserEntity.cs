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
 * Date: 2016-2-10
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.EntityLoader;
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


        /// <summary>
        /// Gets or sets the security user key
        /// </summary>
        [DataIgnore, XmlElement("securityUser"), JsonProperty("securityUser")]
        public Guid? SecurityUserKey
        {
            get
            {
                return this.SecurityUser?.Key;
            }
            set
            {
                if (this.SecurityUser?.Key != value)
                    this.SecurityUser = this.EntityProvider?.Get<SecurityUser>(value);
            }
        }

        /// <summary>
        /// Gets or sets the security user key
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(SecurityUserKey))]
		public SecurityUser SecurityUser { get; set; }

    }
}
