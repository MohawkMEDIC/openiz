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
using System;
using System.IO;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
	/// <summary>
	/// Applet package used for installations only
	/// </summary>
	[XmlType(nameof(AppletPackage), Namespace = "http://openiz.org/applet")]
    [XmlRoot(nameof(AppletPackage), Namespace = "http://openiz.org/applet")]
	public class AppletPackage
	{
        /// <summary>
        /// Load the specified manifest name
        /// </summary>
        public static AppletPackage Load(Stream resourceStream)
        {
            XmlSerializer xsz = new XmlSerializer(typeof(AppletPackage));
            var amfst = xsz.Deserialize(resourceStream) as AppletPackage;
            return amfst;
        }

        /// <summary>
        /// Applet reference metadata
        /// </summary>
        [XmlElement("info")]
		public AppletInfo Meta {
			get;
			set;
		}

		/// <summary>
		/// Gets or ses the manifest to be installed
		/// </summary>
		/// <value>The manifest.</value>
		[XmlElement("manifest")]
		public byte[] Manifest {
			get;
			set;
		}

        /// <summary>
        /// Unpack the package
        /// </summary>
        public AppletManifest Unpack()
        {
            using (MemoryStream ms = new MemoryStream(this.Manifest))
                return AppletManifest.Load(ms);
        }
    }
}

