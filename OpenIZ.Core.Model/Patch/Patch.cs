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
 * Date: 2016-11-30
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Patch
{
    /// <summary>
    /// Represents a series of patch instructions 
    /// </summary>
    [XmlType(nameof(Patch), Namespace = "http://openiz.org/model")]
    [XmlRoot(nameof(Patch), Namespace = "http://openiz.org/model")]
    [JsonObject(nameof(Patch))]
    public class Patch : BaseEntityData
    {
        /// <summary>
        /// Patch
        /// </summary>
        public Patch()
        {
            this.Version = typeof(Patch).GetTypeInfo().Assembly.GetName().Version.ToString();
            this.Operation = new List<PatchOperation>();
        }

        /// <summary>
        /// Gets or sets the version of the patch file
        /// </summary>
        [XmlAttribute("version"), JsonProperty("version")]
        public String Version { get; set; }

        /// <summary>
        /// Application version
        /// </summary>
        [XmlElement("appliesTo"), JsonProperty("appliesTo")]
        public PatchTarget AppliesTo { get; set; }

        /// <summary>
        /// A list of patch operations to be applied to the object
        /// </summary>
        [XmlElement("change"), JsonProperty("change")]
        public List<PatchOperation> Operation { get; set; }

        /// <summary>
        /// To string representation
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var itm in this.Operation)
                builder.AppendFormat("{0}\r\n", itm);
            return builder.ToString();
        }
    }
}
