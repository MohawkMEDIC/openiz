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
 * Date: 2016-7-16
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using System;
using System.Xml.Serialization;
namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a code system which is a collection of reference terms
    /// </summary>
    
    [XmlType("CodeSystem",  Namespace = "http://openiz.org/model"), JsonObject("CodeSystem")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "CodeSystem")]
    [Classifier(nameof(Name))]
    public class CodeSystem : NonVersionedEntityData
    {

        /// <summary>
        /// Creates a new code system
        /// </summary>
        public CodeSystem()
        {

        }

        /// <summary>
        /// Creates a new code system object
        /// </summary>
        public CodeSystem(String name, String oid, String authority)
        {
            this.Name = name;
            this.Oid = oid;
            this.Authority = authority;
        }

        /// <summary>
        /// Gets or sets the name of the code system
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Oid of the code system
        /// </summary>
        [XmlElement("oid"), JsonProperty("oid")]
        public string Oid { get; set; }

        /// <summary>
        /// Gets or sets the authority of the code system
        /// </summary>
        [XmlElement("authority"), JsonProperty("authority")]
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets the obsoletion reason of the code system
        /// </summary>
        [XmlElement("obsoletionReason"), JsonProperty("obsoletionReason")]
        public string ObsoletionReason { get; set; }

        /// <summary>
        /// Gets or sets the URL of the code system
        /// </summary>
        [XmlElement("url"), JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the version text of the code system
        /// </summary>
        [XmlElement("version"), JsonProperty("version")]
        public string VersionText { get; set; }

        /// <summary>
        /// Gets or sets the human description
        /// </summary>
        [XmlElement("description"), JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Determine equality of 
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            CodeSystem other = obj as CodeSystem;
            if (other == null) return false;
            return base.SemanticEquals(obj) && this.Name == other.Name &&
                this.Oid == other.Oid &&
                this.Authority == other.Authority &&
                this.VersionText == other.VersionText;
        }
    }
}