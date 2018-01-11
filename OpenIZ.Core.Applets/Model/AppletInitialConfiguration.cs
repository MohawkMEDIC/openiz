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
 * User: justi
 * Date: 2016-8-2
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
    /// <summary>
    /// Represents a configuration of an applet
    /// </summary>
    [XmlType(nameof(AppletInitialConfiguration), Namespace = "http://openiz.org/applet")]
    public class AppletInitialConfiguration
    {

        /// <summary>
        /// Gets or sets the applet id
        /// </summary>
        [XmlAttribute("applet")]
        public String AppletId
        {
            get;
            set;
        }

        /// <summary>
        /// Applet configuration entry
        /// </summary>
        [XmlElement("appSetting")]
        public List<AppletConfigurationEntry> AppSettings
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Applet configuration entry
    /// </summary>
    [XmlType(nameof(AppletConfigurationEntry), Namespace = "http://openiz.org/applet")]
    public class AppletConfigurationEntry
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public String Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlAttribute("value")]
        public String Value
        {
            get;
            set;
        }
    }

}
