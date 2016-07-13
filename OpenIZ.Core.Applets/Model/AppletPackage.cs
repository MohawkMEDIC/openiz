using System;
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
	}
}

