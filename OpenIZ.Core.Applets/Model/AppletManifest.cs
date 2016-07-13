using System;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace OpenIZ.Core.Applets.Model
{
	/// <summary>
	/// The applet manifest is responsible for storing data related to a JavaScript applet
	/// </summary>
	[XmlType(nameof(AppletManifest), Namespace = "http://openiz.org/applet")]
	[XmlRoot(nameof(AppletManifest), Namespace = "http://openiz.org/applet")]
	public class AppletManifest
	{

		/// <summary>
		/// Load the specified manifest name
		/// </summary>
		public static AppletManifest Load(Stream resourceStream)
		{
			XmlSerializer xsz = new XmlSerializer (typeof(AppletManifest));
			var amfst = xsz.Deserialize (resourceStream) as AppletManifest;
            amfst.Initialize();
            return amfst;
		}

        /// <summary>
        /// Initialize the applet manifest
        /// </summary>
        public void Initialize()
        {
            foreach (var ast in this.Assets)
                ast.Manifest = this;
        }

		/// <summary>
		/// Create an unsigned package
		/// </summary>
		/// <returns>The package.</returns>
		public AppletPackage CreatePackage()
		{
			AppletPackage retVal = new AppletPackage () {
				Meta = this.Info
			};
			using (MemoryStream ms = new MemoryStream ()) {
				XmlSerializer xsz = new XmlSerializer (typeof(AppletManifest));
				xsz.Serialize (ms, this);
				retVal.Manifest = ms.ToArray ();
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
    }
}

