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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Interop
{
    /// <summary>
    /// Service resource options
    /// </summary>
    [XmlType(nameof(ServiceResourceOptions), Namespace = "http://openiz.org/model"), JsonObject(nameof(ServiceResourceOptions))]
    public class ServiceResourceOptions
    {

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceOptions"/> class.
		/// </summary>
	    public ServiceResourceOptions()
	    {
		    
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResourceOptions"/> class
		/// with a specific resource name, and verbs.
		/// </summary>
		/// <param name="resourceName">The name of the resource of the service resource options.</param>
		/// <param name="verbs">The list of HTTP verbs of the resource option.</param>
	    public ServiceResourceOptions(string resourceName, List<string> verbs)
	    {
		    this.ResourceName = resourceName;
		    this.Verbs = verbs;
	    }

        /// <summary>
        /// Gets the name of the resource
        /// </summary>
        [XmlAttribute("resource"), JsonProperty("resource")]
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets the verbs supported on the specified resource
        /// </summary>
        [XmlElement("verb"), JsonProperty("verb")]
        public List<string> Verbs { get; set; }
    }
}