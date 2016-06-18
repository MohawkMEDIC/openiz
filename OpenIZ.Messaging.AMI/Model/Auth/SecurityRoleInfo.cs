﻿/*
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
 * User: Nityan
 * Date: 2016-6-17
 */
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