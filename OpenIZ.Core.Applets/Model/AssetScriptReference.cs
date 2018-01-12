/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-11-2
 */
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
    /// <summary>
    /// Represents an asset script reference
    /// </summary>
    [XmlType(nameof(AssetScriptReference), Namespace = "http://openiz.org/applet")]
    [JsonObject(nameof(AssetScriptReference))]
    public class AssetScriptReference
    {
        /// <summary>
        /// True if the reference is static
        /// </summary>
        [XmlAttribute("static")]
        [JsonProperty("static")]
        public string IsStaticString
        {
            get
            {
                return (this.IsStatic ?? true).ToString();
            }
            set
            {
                if (value == null)
                    this.IsStatic = null;
                else
                    this.IsStatic = bool.Parse(value);
            }
        }

        /// <summary>
        /// Static
        /// </summary>
        [XmlIgnore]
        public bool? IsStatic { get; set; }

        /// <summary>
        /// Reference itself
        /// </summary>
        [XmlText]
        [JsonProperty("href")]
        public string Reference { get; set; }

    }
}