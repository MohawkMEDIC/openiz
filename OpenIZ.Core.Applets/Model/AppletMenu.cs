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
using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;

namespace OpenIZ.Core.Applets.Model
{
	/// <summary>
	/// Applet tile
	/// </summary>
	[XmlType(nameof(AppletMenu), Namespace = "http://openiz.org/applet")]
	public class AppletMenu
	{

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="host"></param>
        internal void Initialize(AppletManifest host)
        {
            this.Manifest = host;
            if (this.Menus != null)
                foreach (var itm in this.Menus)
                    itm.Initialize(host);
            
        }

        /// <summary>
        /// Gets or sets the order
        /// </summary>
        [XmlAttribute("order")]
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the icon file reference
        /// </summary>
        [XmlElement("icon")]
		public String Icon {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the applet asset that this item should launch
        /// </summary>
        [XmlAttribute("launch")]
        public String Launch { get; set; }

        /// <summary>
        /// Asset that this menu item is linked to
        /// </summary>
        [XmlAttribute("asset")]
        public String Asset { get; set; }

        /// <summary>
        /// Applet manifest
        /// </summary>
        [XmlIgnore]
        public AppletManifest Manifest { get; internal set; }

        /// <summary>
        /// Gets the specified name
        /// </summary>
        public String GetText(String language, bool returnNuetralIfNotFound = true)
		{
			var str = this.Text?.Find(o=>o.Language == language);
			if(str == null && returnNuetralIfNotFound)
				str = this.Text?.Find(o=>o.Language == null);
			return str?.Value;
		}

		/// <summary>
		/// Gets or sets the name of the applet info
		/// </summary>
		/// <value>The name.</value>
		[XmlElement("text")]
		public List<LocaleString> Text {
			get;
			set;
		}

        /// <summary>
        /// One or more menu items
        /// </summary>
        [XmlElement("menuItem")]
        public List<AppletMenu> Menus { get; set; }
    }

}

