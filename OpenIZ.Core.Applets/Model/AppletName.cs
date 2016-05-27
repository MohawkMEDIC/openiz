using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;

namespace OpenIZ.Core.Applets.Model
{
	/// <summary>
	/// Applet reference
	/// </summary>
	[XmlType(nameof(AppletName), Namespace = "http://openiz.org/applet")]
	public class AppletName
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Core.Applets.Model.AppletName"/> class.
		/// </summary>
		public AppletName ()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Core.Applets.Model.AppletName"/> class.
		/// </summary>
		public AppletName (String id, String version, String publicKeyToken)
		{
			this.Id = id;
			this.Version = version;
			this.PublicKeyToken = publicKeyToken;
		}

		/// <summary>
		/// The identifier of the applet
		/// </summary>
		[XmlAttribute("id")]
		public String Id {
			get;
			set;
		}

		/// <summary>
		/// The version of the applet
		/// </summary>
		[XmlAttribute("version")]
		public String Version {
			get;
			set;
		}

		/// <summary>
		/// The signature of the applet (not used for verification, rather lookup)
		/// </summary>
		/// <value>The signature.</value>
		[XmlAttribute("publicKeyToken")]
		public String PublicKeyToken {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the signature which can be used to validate the file
		/// </summary>
		[XmlElement("signature")]
		public byte[] Signature {
			get;
			set;
		}

		/// <summary>
		/// A hash which validates the file has not changed on the disk. Different from signature which 
		/// indicates the manifest has not changed since installed.
		/// </summary>
		/// <value><c>true</c> if this instance hash; otherwise, <c>false</c>.</value>
		[XmlElement("hash")]
		public byte[] Hash {
			get;
			set;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format ("Id={0}, Version={1}, PublicKeyToken={2}", Id, Version, PublicKeyToken);
		}
	}


}

