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
    /// Represents a panel. A panel is a special pointer which has a title and content which can be rendered.
    /// </summary>
    [XmlType(nameof(AppletPanel), Namespace = "http://openiz.org/applet")]
    [JsonObject]
    public class AppletPanel
    {

        /// <summary>
        /// Gets the main titles of the panel
        /// </summary>
        [XmlElement("title")]
        [JsonProperty("title")]
        public List<LocaleString> Titles { get; set; }

        /// <summary>
        /// Gets or sets the content to show in the panel
        /// </summary>
        [XmlElement("contentRef", Type = typeof(String))]
        [XmlElement("contentHtml", Type = typeof(AppletAssetHtml))]
        [JsonIgnore]
        public Object PanelAsset { get; set; }

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
        /// Gets or sets the description
        /// </summary>
        [XmlElement("description")]
        [JsonProperty("description")]
        public List<LocaleString> Descriptions { get; set; }

        /// <summary>
        /// Gets the specified name
        /// </summary>
        public String GetTitle(String language, bool returnNuetralIfNotFound = true)
        {
            var str = this.Titles?.Find(o => o.Language == language);
            if (str == null && returnNuetralIfNotFound)
                str = this.Titles?.Find(o => o.Language == null);
            return str?.Value;
        }


        /// <summary>
        /// Gets the specified decription
        /// </summary>
        public String GetDescription(String language, bool returnNuetralIfNotFound = true)
        {
            var str = this.Descriptions?.Find(o => o.Language == language);
            if (str == null && returnNuetralIfNotFound)
                str = this.Descriptions?.Find(o => o.Language == null);
            return str?.Value;
        }
    }

}
