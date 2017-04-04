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
 * User: khannan
 * Date: 2016-11-11
 */

using OpenIZ.Core.Applets.Model;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Applet
{
	/// <summary>
	/// Represents a wrapper for the <see cref="Applets.Model.AppletManifest"/> class.
	/// </summary>
	[XmlType(nameof(AppletManifestInfo), Namespace = "http://openiz.org/ami")]
	[XmlRoot(nameof(AppletManifestInfo), Namespace = "http://openiz.org/ami")]
	public class AppletManifestInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AppletManifestInfo"/> class.
		/// </summary>
		public AppletManifestInfo()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppletManifestInfo"/> class
		/// with a specific applet manifest instance.
		/// </summary>
		/// <param name="manifest">The applet manifest instance.</param>
		public AppletManifestInfo(AppletManifest manifest)
		{

			this.AppletManifest = new AppletManifest()
            {
                Info = manifest.Info
            };
		}

		/// <summary>
		/// Gets or sets the applet manifest.
		/// </summary>
		public AppletManifest AppletManifest { get; set; }

		/// <summary>
		/// Gets or sets the file extension of the applet.
		/// </summary>
		public string FileExtension { get; set; }
	}
}