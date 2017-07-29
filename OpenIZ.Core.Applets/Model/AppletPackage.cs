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
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors.LZMA;
using System;
using System.IO;
using System.Reflection;
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
        /// Applet package
        /// </summary>
        public AppletPackage()
        {
            this.Version = typeof(AppletPackage).GetTypeInfo().Assembly.GetName().Version.ToString();
        }

        // Serializer
        private static XmlSerializer s_xsz = new XmlSerializer(typeof(AppletPackage));

        /// <summary>
        /// Load the specified manifest name
        /// </summary>
        public static AppletPackage Load(byte[] resourceData)
        {
            using (MemoryStream ms = new MemoryStream(resourceData))
                return AppletPackage.Load(ms);
        }

        /// <summary>
        /// Load the specified manifest name
        /// </summary>
        public static AppletPackage Load(Stream resourceStream)
        {
            using (GZipStream gzs = new GZipStream(resourceStream,  CompressionMode.Decompress))
            {
                var amfst = s_xsz.Deserialize(gzs) as AppletPackage;
                return amfst;
            }
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
        /// The pak version
        /// </summary>
        [XmlAttribute("pakVersion")]
        public String Version { get; set; }

        /// <summary>
        /// Compression algorithm
        /// </summary>
        [XmlAttribute("compress")]
        public String Compression { get; set; }

        /// <summary>
        /// Public signing certificate
        /// </summary>
        [XmlElement("certificate")]
        public byte[] PublicKey { get; set; }

        /// <summary>
        /// Unpack the package
        /// </summary>
        public AppletManifest Unpack()
        {
            switch(this.Compression)
            {
                case "lzma":
                    using (MemoryStream ms = new MemoryStream(this.Manifest))
                    using (var dfs = new LZipStream(ms, SharpCompress.Compressors.CompressionMode.Decompress, true))
                        return AppletManifest.Load(dfs);
                case "bzip2":
                    using (MemoryStream ms = new MemoryStream(this.Manifest))
                    using (var dfs = new BZip2Stream(ms, SharpCompress.Compressors.CompressionMode.Decompress, true))
                        return AppletManifest.Load(dfs);
                case "gzip":
                    using (MemoryStream ms = new MemoryStream(this.Manifest))
                    using (GZipStream dfs = new GZipStream(ms, CompressionMode.Decompress))
                        return AppletManifest.Load(dfs);
                default:
                    using (MemoryStream ms = new MemoryStream(this.Manifest))
                    using (DeflateStream dfs = new DeflateStream(ms, CompressionMode.Decompress))
                        return AppletManifest.Load(dfs);
            }
        }

        /// <summary>
        /// Save the compressed applet manifest
        /// </summary>
        public void Save(Stream stream)
        {
            using (GZipStream gzs = new GZipStream(stream, CompressionMode.Compress))
            {
                s_xsz.Serialize(gzs, this);
            }
        }
    }
}

