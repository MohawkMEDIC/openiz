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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using System.Reflection;
using System.Runtime;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Instructions on how an extensionshould be handled
    /// </summary>
    [Classifier(nameof(Name)), KeyLookup(nameof(Name))]
    [XmlType(nameof(ExtensionType),  Namespace = "http://openiz.org/model"), JsonObject("ExtensionType")]
    [XmlRoot(nameof(ExtensionType), Namespace = "http://openiz.org/model")]
    public class ExtensionType : NonVersionedEntityData
    {

        /// <summary>
        /// Extension type ctor
        /// </summary>
        public ExtensionType()
        {

        }

        /// <summary>
        /// Creates  a new extension type
        /// </summary>
        public ExtensionType(String name, Type handlerClass)
        {
            this.Name = name;
            this.ExtensionHandler = handlerClass;
        }

        /// <summary>
        /// Gets or sets the extension handler
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Type ExtensionHandler { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [XmlElement("handlerClass"), JsonProperty("handlerClass")]
        public String ExtensionHandlerXml
        {
            get { return this.ExtensionHandler?.AssemblyQualifiedName; }
            set
            {
                if (value == null)
                    this.ExtensionHandler = null;
                else
                    this.ExtensionHandler = System.Type.GetType(value);
            }
        }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Whether the extension is enabled
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public bool IsEnabled { get; set; }


    }
}
