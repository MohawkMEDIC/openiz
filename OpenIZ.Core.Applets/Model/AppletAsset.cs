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
            return String.Format("{0}{1}/{2}", AppletCollection.APPLET_SCHEME, this.Manifest?.Info?.Id, this.Name);
        }
    }

}

