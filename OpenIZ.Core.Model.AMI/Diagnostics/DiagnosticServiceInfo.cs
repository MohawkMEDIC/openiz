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
 * Date: 2017-9-1
 */
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Diagnostics
{
    /// <summary>
    /// Represents diagnostic service info
    /// </summary>
	[JsonObject(nameof(DiagnosticServiceInfo)), XmlType(nameof(DiagnosticServiceInfo), Namespace = "http://openiz.org/ami/diagnostics")]
    public class DiagnosticServiceInfo
    {

        public DiagnosticServiceInfo()
        {

        }

        /// <summary>
        /// Create the service info
        /// </summary>
        public DiagnosticServiceInfo(object daemon)
        {
            var dat = daemon?.GetType().GetTypeInfo().GetCustomAttributes().FirstOrDefault(a => a.GetType().Name == "DescriptionAttribute");
            this.Description = dat?.GetType().GetRuntimeProperty("Description")?.GetValue(dat)?.ToString();
            this.IsRunning = (bool)daemon.GetType().GetRuntimeProperty("IsRunning")?.GetValue(daemon);
            this.Type = daemon.GetType().FullName;
        }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [XmlElement("description"), JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets whether the service is running
        /// </summary>
        [XmlElement("running"), JsonProperty("running")]
        public bool IsRunning { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        [XmlElement("type"), JsonProperty("type")]
        public string Type { get; set; }

    }
}