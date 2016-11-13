/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-8-2
 */
using System;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Represents a base protocol element
    /// </summary>
    [XmlType(nameof(DecisionSupportBaseElement), Namespace = "http://openiz.org/cdss")]
    public abstract class DecisionSupportBaseElement
    {

        /// <summary>
        /// Gets or sets the identifier of the object
        /// </summary>
        [XmlAttribute("uuid")]
        public Guid Uuid { get; set; }

        /// <summary>
        /// Identifies the object within the protocol
        /// </summary>
        [XmlAttribute("id")]
        public String Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the object
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version of the object
        /// </summary>
        [XmlAttribute("version")]
        public string Version { get; set; }

    }
}