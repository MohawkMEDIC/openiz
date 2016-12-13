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
 * User: khannan
 * Date: 2016-12-4
 */
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using OpenIZ.Core.Model.RISI.Model;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Core.Model.RISI
{
    /// <summary>
    /// Represents a stored query to be performed against the RISI
    /// </summary>
    [XmlType(nameof(ReportDefinition), Namespace = "http://openiz.org/risi")]
    [XmlRoot(nameof(ReportDefinition), Namespace = "http://openiz.org/risi")]
    public class ReportDefinition : BaseEntityData
    {

        /// <summary>
        /// Gets the name of the stored query
        /// </summary>
        [XmlElement("name")]
        public String Name { get; set; }

        /// <summary>
        /// A list of parameters which is supported for the specified query
        /// </summary>
        [XmlElement("parameters")]
        public List<ParameterDefinition> Parameters { get; set; }

        /// <summary>
        /// Security policy instances related to the query definition
        /// </summary>
        [XmlElement("policy")]
        public List<SecurityPolicyInstance> Policies { get; set; }

    }
}
