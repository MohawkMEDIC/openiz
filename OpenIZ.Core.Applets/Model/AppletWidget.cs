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
 * Date: 2016-8-2
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{

    /// <summary>
    /// Identifies the scope of the panel
    /// </summary>
    [XmlType(nameof(AppletWidgetScope), Namespace = "http://openiz.org/applet")]
    public enum AppletWidgetScope
    {
        /// <summary>
        /// The widget should be placed on the patient summary page
        /// </summary>
        [XmlEnum("patient")]
        Patient,
        /// <summary>
        /// The widget should be placed on the facility home page
        /// </summary>
        [XmlEnum("place")]
        Facility,
        /// <summary>
        /// The widget should be placed on the user settings page
        /// </summary>
        [XmlEnum("user")]
        User
    }

    /// <summary>
    /// Identifies the type which the widget is
    /// </summary>
    [XmlType(nameof(AppletWidgetType), Namespace = "http://openiz.org/applet")]
    public enum AppletWidgetType
    {
        /// <summary>
        /// The extension is a panel which is added to a panel container
        /// </summary>
        [XmlEnum("panel")]
        Panel,
        /// <summary>
        /// The extension is a tab
        /// </summary>
        [XmlEnum("tab")]
        Tab
    }

    /// <summary>
    /// Represents a widget. A widget is a special pointer which has a title and content which can be rendered
    /// in a container 
    /// </summary>
    [XmlType(nameof(AppletWidget), Namespace = "http://openiz.org/applet")]
    [JsonObject]
    public class AppletWidget : AppletAssetHtml
    {
        /// <summary>
        /// Gets or sets the scope where the widget can be used
        /// </summary>
        [XmlAttribute("scope")]
        [JsonProperty("scope")]
        public AppletWidgetScope Scope { get; set; }

        /// <summary>
        /// Gets or sets the type of widget
        /// </summary>
        [XmlAttribute("type")]
        [JsonProperty("type")]
        public AppletWidgetType Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the widget
        /// </summary>
        [XmlAttribute("name")]
        [JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the controller
        /// </summary>
        [XmlElement("controller")]
        [JsonProperty("controller")]
        public String Controller { get; set; }

        /// <summary>
        /// Gets the main titles of the panel
        /// </summary>
        [XmlElement("description")]
        [JsonProperty("description")]
        public List<LocaleString> Description { get; set; }


        /// <summary>
        /// Gets or sets the icon file reference
        /// </summary>
        [XmlElement("icon")]
        [JsonProperty("icon")]
        public String Icon
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the specified decription
        /// </summary>
        public String GetDescription(String language, bool returnNuetralIfNotFound = true)
        {
            var str = this.Description?.Find(o => o.Language == language);
            if (str == null && returnNuetralIfNotFound)
                str = this.Description?.Find(o => o.Language == null);
            return str?.Value;
        }
    }

}
