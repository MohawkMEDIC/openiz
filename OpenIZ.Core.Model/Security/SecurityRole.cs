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
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.EntityLoader;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.Security
{
    /// <summary>
    /// Security role
    /// </summary>
    [XmlType(Namespace = "http://openiz.org/model", TypeName = "SecurityRole")]
    [KeyLookup(nameof(Name))]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "SecurityRole")]
    [JsonObject(nameof(SecurityRole))]
    public class SecurityRole : SecurityEntity
    {
        /// <summary>
        /// Users in teh group
        /// </summary>
        public SecurityRole()
        {
            this.Users = new List<SecurityUser>();
        }
        
        /// <summary>
        /// Gets or sets the name of the security role
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Description of the role
        /// </summary>
        [XmlElement("description"), JsonProperty("description")]
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the security users in the role
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public List<SecurityUser> Users { get; set; }

        /// <summary>
        /// Determine semantic equality
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as SecurityRole;
            if (other == null) return false;
            return base.SemanticEquals(obj) &&
                this.Name == other.Name;
        }
    }
}
