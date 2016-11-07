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
 * Date: 2016-6-14
 */
using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;

namespace OpenIZ.Core.Applets.Model
{

	/// <summary>
	/// Represents an applet asset
	/// </summary>
	[XmlType(nameof(AppletAsset), Namespace = "http://openiz.org/applet")]
	public class AppletAsset
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Core.Applets.Model.AppletAsset"/> class.
		/// </summary>
		public AppletAsset ()
		{
		}

        /// <summary>
        /// Gets the or sets the manifest to which the asset belongs
        /// </summary>
        [XmlIgnore]
        public AppletManifest Manifest { get; internal set; }

        /// <summary>
        /// Gets or sets the name of the asset
        /// </summary>
        [XmlAttribute("name")]
		public String Name {
			get;
			set;
		}

        /// <summary>
        /// Language
        /// </summary>
        [XmlAttribute("lang")]
		public String Language {
			get;
			set;
		}

		/// <summary>
		/// Mime type
		/// </summary>
		[XmlAttribute("mimeType")]
		public String MimeType {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the applets required policies for a user to run
        /// </summary>
        [XmlElement("demand")]
        public List<String> Policies
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content of the asset
        /// </summary>
        /// <remarks>
        /// Assets of type contentXml 
        /// </remarks>
        [XmlElement("contentText", Type = typeof(String))]
		[XmlElement("contentBin", Type = typeof(byte[]))]
		[XmlElement("contentXml", Type = typeof(XElement))]
        [XmlElement("contentHtml", Type = typeof(AppletAssetHtml))]
		public Object Content {
			get;
			set;
		}


        /// <summary>
        /// Represent the asset as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("/{1}/{2}", AppletCollection.APPLET_SCHEME, this.Manifest?.Info?.Id, this.Name);
        }
    }

}

