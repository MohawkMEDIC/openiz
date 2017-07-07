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
 * Date: 2017-1-11
 */
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
    /// <summary>
    /// Represents a view model definition
    /// </summary>
    [XmlType(nameof(AppletViewModelDefinition), Namespace = "http://openiz.org/applet")]
    public class AppletViewModelDefinition
    {

        /// <summary>
        /// Gets or sets the mnemonic
        /// </summary>
        [XmlAttribute("key")]
        public String ViewModelId { get; set; }

        /// <summary>
        /// Gets or sets the definition
        /// </summary>
        [XmlElement("definition")]
        public String Definition { get; set; }

        /// <summary>
        /// The content loaded
        /// </summary>
        [XmlIgnore]
        public byte[] DefinitionContent { get; internal set; }

    }
}