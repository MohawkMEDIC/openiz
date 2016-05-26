using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;

namespace OpenIZ.Core.Applets.Model
{
	/// <summary>
	/// Applet tile
	/// </summary>
	[XmlType(nameof(AppletMenu), Namespace = "http://openiz.org/applet")]
	public class AppletMenu
	{


		/// <summary>
		/// Gets or sets the icon file reference
		/// </summary>
		[XmlElement("icon")]
		public String Icon {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the applet asset that this item should launch
        /// </summary>
        [XmlAttribute("launch")]
        public String Launcher { get; set; }

        /// <summary>
        /// The live source indicates the data source from which live information should be given
        /// </summary>
        /// <value>The live source SQL query to execute.</value>
        [XmlElement("liveSourceSql")]
		public String LiveSourceSql {
			get;
			set;
		}

		/// <summary>
		/// Gets the specified name
		/// </summary>
		public String GetText(String language, bool returnNuetralIfNotFound = true)
		{
			var str = this.Text?.Find(o=>o.Language == language);
			if(str == null && returnNuetralIfNotFound)
				str = this.Text?.Find(o=>o.Language == null);
			return str?.Value;
		}

		/// <summary>
		/// Gets or sets the name of the applet info
		/// </summary>
		/// <value>The name.</value>
		[XmlElement("text")]
		public List<LocaleString> Text {
			get;
			set;
		}
	}

}

