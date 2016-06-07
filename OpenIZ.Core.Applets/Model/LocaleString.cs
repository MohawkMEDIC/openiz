using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;

namespace OpenIZ.Core.Applets.Model
{
	/// <summary>
	/// Applet localized string
	/// </summary>
	[XmlType(nameof(LocaleString), Namespace = "http://openiz.org/applet")]
	public class LocaleString
	{

		/// <summary>
		/// Gets or sets the language representation
		/// </summary>
		/// <value>The language.</value>
		[XmlAttribute("lang")]
		public String Language {
			get;
			set;
		}

		/// <summary>
		/// Value of the applet string
		/// </summary>
		/// <value>The value.</value>
		[XmlText]
		public String Value {
			get;
			set;
		}
	}


}

