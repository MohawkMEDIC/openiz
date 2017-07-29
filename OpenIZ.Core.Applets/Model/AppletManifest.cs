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
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using SharpCompress.Compressors.LZMA;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors.BZip2;

namespace OpenIZ.Core.Applets.Model
{
	/// <summary>
	/// The applet manifest is responsible for storing data related to a JavaScript applet
	/// </summary>
	[XmlType(nameof(AppletManifest), Namespace = "http://openiz.org/applet")]
	[XmlRoot(nameof(AppletManifest), Namespace = "http://openiz.org/applet")]
	public class AppletManifest : IEquatable<AppletManifest>
	{

        private static XmlSerializer x_xsz = new XmlSerializer(typeof(AppletManifest));

        /// <summary>
        /// Load the specified manifest name
        /// </summary>
        public static AppletManifest Load(Stream resourceStream)
		{
			
			var amfst = x_xsz.Deserialize (resourceStream) as AppletManifest;
            amfst.Initialize();
            return amfst;
		}

        /// <summary>
        /// Save this applet manifest to the stream
        /// </summary>
        public void Save(Stream resourceStream)
        {
            x_xsz.Serialize(resourceStream, this);
        }

        /// <summary>
        /// Initialize the applet manifest
        /// </summary>
        public void Initialize()
        {
            foreach (var ast in this.Assets)
                ast.Manifest = this;
            foreach (var mnu in this.Menus)
                mnu.Initialize(this);
        }

        /// <summary>
        /// Create an unsigned package
        /// </summary>
        /// <returns>The package.</returns>
        public AppletPackage CreatePackage(String compression = null)
		{
			AppletPackage retVal = new AppletPackage () {
				Meta = this.Info,
                Compression = compression
			};
            using (MemoryStream ms = new MemoryStream())
            {
                Stream compressStream = null;
                try {
                    switch (compression)
                    {
                        case "lzma":
                            compressStream = new LZipStream(ms, SharpCompress.Compressors.CompressionMode.Compress, leaveOpen: true);
                            break;
                        case "bzip2":
                            compressStream = new BZip2Stream(ms, SharpCompress.Compressors.CompressionMode.Compress, leaveOpen: true);
                            break;
                        case "gzip":
                            compressStream = new GZipStream(ms, SharpCompress.Compressors.CompressionMode.Compress, leaveOpen: true);
                            break;
                        default:
                            compressStream = new DeflateStream(ms, SharpCompress.Compressors.CompressionMode.Compress, leaveOpen: true);
                            break;
                    }
                    XmlSerializer xsz = new XmlSerializer(typeof(AppletManifest));
                    xsz.Serialize(compressStream, this);
                }
                finally
                {
                    compressStream.Dispose();
                }
                retVal.Manifest = ms.ToArray();

            }
            return retVal;

		}

        /// <summary>
        /// Gets or sets the data operations to be performed
        /// </summary>
        [XmlElement("data")]
        public AssetData DataSetup { get; set; }

        /// <summary>
        /// Applet information itself
        /// </summary>
        [XmlElement("info")]
        public AppletInfo Info
        {
            get;
            set;
        }

        /// <summary>
        /// Instructs the host which asset can be used as a starting point
        /// </summary>
        [XmlElement("startupAsset")]
        public String StartAsset { get; set; }

        /// <summary>
        /// Instructs the host which asset can be used as a starting point
        /// </summary>
        [XmlElement("loginAsset")]
        public String LoginAsset { get; set; }


        /// <summary>
        /// Initial applet configuration
        /// </summary>
        /// <value>The configuration.</value>
        [XmlElement("configuration")]
		public AppletInitialConfiguration Configuration {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the tile sizes the applet can have
        /// </summary>
        [XmlElement("menuItem")]
        public List<AppletMenu> Menus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the applet panels
        /// </summary>
        [XmlElement("panel")]
        public List<AppletPanel> Panels
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or ets the templates for use in the applet
        /// </summary>
        [XmlElement("template")]
        public List<AppletTemplateDefinition> Templates { get; set; }

        /// <summary>
        /// Gets or sets the view model definitions 
        /// </summary>
        [XmlElement("viewModel")]
        public List<AppletViewModelDefinition> ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the assets which are to be used in the applet
        /// </summary>
        [XmlElement("asset")]
		public List<AppletAsset> Assets {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the applet strings
        /// </summary>
        [XmlElement("strings")]
        public List<AppletStrings> Strings { get; set; }

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		public bool Equals(AppletManifest other)
		{
			if (other == null)
			{
				return false;
			}

			if (this.Info == null && other.Info == null)
			{
				return true;
			}

			return this.Info?.Id == other.Info?.Id;
		}
	}
}