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
using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents the model of a protocol
    /// </summary>
    [XmlType(nameof(Protocol), Namespace = "http://openiz.org/model"), JsonObject(nameof(Protocol))]
    public class Protocol : BaseEntityData
    {

        /// <summary>
        /// Gets or sets the name of the protocol
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the handler for this protocol (which can load the definition
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Type HandlerClass
        {
            get
            {
                return System.Type.GetType(this.HandlerClassName);
            }
            set
            {
                this.HandlerClassName = value.AssemblyQualifiedName;
            }
        }

        /// <summary>
        /// Gets or sets the handler class AQN
        /// </summary>
        [XmlElement("handlerClass"), JsonProperty("handlerClass")]
        public String HandlerClassName { get; set; }

        /// <summary>
        /// Contains instructions which the handler class can understand
        /// </summary>
        [XmlElement("definition"), JsonProperty("definition")]
        public byte[] Definition { get; set; }

        /// <summary>
        /// Semantic equality
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as Protocol;
            if (other == null) return false;
            return base.SemanticEquals(obj) &&
                this.Name == other.Name;
        }
    }
}