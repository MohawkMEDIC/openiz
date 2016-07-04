using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenIZ.Core.Applets.Model
{
	/// <summary>
	/// Applet localized string
	/// </summary>
	[XmlType(nameof(LocaleString), Namespace = "http://openiz.org/applet")]
    [JsonObject]
	public class LocaleString
	{

		/// <summary>
		/// Gets or sets the language representation
		/// </summary>
		/// <value>The language.</value>
		[XmlAttribute("lang"), JsonProperty("lang")]
		public String Language {
			get;
			set;
		}

		/// <summary>
		/// Value of the applet string
		/// </summary>
		/// <value>The value.</value>
		[XmlText, JsonProperty("value")]
		public String Value {
			get;
			set;
		}
	}


}

