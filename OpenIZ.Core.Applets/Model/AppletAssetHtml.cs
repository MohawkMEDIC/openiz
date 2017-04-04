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
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{


    /// <summary>
    /// Applet asset XML 
    /// </summary>
    [XmlType(nameof(AppletAssetHtml), Namespace = "http://openiz.org/applet")]
    public class AppletAssetHtml
    {

        // Backing element for HTML
        private XElement m_html;

        /// <summary>
        /// Applet asset html
        /// </summary>
        public AppletAssetHtml()
        {
            this.Bundle = new List<string>();
            this.Script = new List<string>();
            this.Style = new List<string>();
        }

        /// <summary>
        /// Gets or sets the title of the applet asset
        /// </summary>
        [XmlElement("title")]
        public List<LocaleString> Titles { get; set; }


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
        /// Gets or sets the master asset for this asset
        /// </summary>
        [XmlElement("layout")]
        public string Layout { get; set; }

        /// <summary>
        /// Gets or sets the references for the assets
        /// </summary>
        [XmlElement("bundle")]
        public List<String> Bundle { get; set; }

        /// <summary>
        /// Gets or sets the script
        /// </summary>
        [XmlElement("script")]
        public List<String> Script { get; set; }

        /// <summary>
        /// Gets or sets the script
        /// </summary>
        [XmlElement("style")]
        public List<String> Style { get; set; }


        /// <summary>
        /// Gets one or more routes
        /// </summary>
        [XmlElement("view")]
        public AppletViewState ViewState { get; set; }

        /// <summary>
        /// Content of the element
        /// </summary>
        //[XmlAnyElement("body", Namespace = "http://www.w3.org/1999/xhtml")]
        //[XmlAnyElement("html", Namespace = "http://www.w3.org/1999/xhtml")]
        //[XmlAnyElement("div", Namespace = "http://www.w3.org/1999/xhtml")]
        [XmlElement("content")]
        public XElement Html
        {
            get
            {
                return this.m_html;
            }
            set
            {
                // HACK: In mono XElement is serialized differently than .NET let's detect that
                if (value.Name.LocalName == "content" && value.Name.Namespace == "http://openiz.org/applet")
                    this.m_html = value.Elements().FirstOrDefault(o => o.Name.Namespace == "http://www.w3.org/1999/xhtml");
                else
                    this.m_html = value;
            }
        }

        /// <summary>
        /// Identifies whether the asset is static
        /// </summary>
        [XmlAttribute("static")]
        public bool Static { get; set; }
    }
}