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
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
    /// <summary>
    /// Represents view state information related to the applet
    /// </summary>
    [XmlType(nameof(AppletViewState), Namespace = "http://openiz.org/applet")]
    public class AppletViewState
    {

        /// <summary>
        /// Gets or sets the name of the view state
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the list of routes associated with the applet view
        /// </summary>
        [XmlElement("route")]
        public String Route { get; set; }

        /// <summary>
        /// Gets or sets the view(s) related to the applet view state
        /// </summary>
        [XmlElement("view")]
        public List<AppletView> View { get; set; }

        /// <summary>
        /// Indicates the view is abstract
        /// </summary>
        [XmlAttribute("abstract")]
        public bool IsAbstract { get; set; }
    }
}