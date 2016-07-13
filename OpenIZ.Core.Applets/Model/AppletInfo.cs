using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenIZ.Core.Applets.Model
{
	/// <summary>
	/// Represents applet information
	/// </summary>
	[XmlType(nameof(AppletInfo), Namespace = "http://openiz.org/applet")]
    [JsonObject]
	public class AppletInfo : AppletName
	{

		/// <summary>
		/// Gets the specified name
		/// </summary>
		public String GetName(String language, bool returnNuetralIfNotFound = true)
		{
			var str = this.Names?.Find(o=>o.Language == language);
			if(str == null && returnNuetralIfNotFound)
				str = this.Names?.Find(o=>o.Language == null);
			return str?.Value;
		}

		/// <summary>
		/// Gets the specified name
		/// </summary>
		public String GetGroupName(String language, bool returnNuetralIfNotFound = true)
		{
			var str = this.GroupNames?.Find(o=>o.Language == language);
			if(str == null && returnNuetralIfNotFound)
				str = this.GroupNames?.Find(o=>o.Language == null);
			return str?.Value;
		}

		/// <summary>
		/// Gets or sets the icon resource
		/// </summary>
		/// <value>The icon.</value>
		[XmlElement("icon")]
        [JsonProperty("icon")]
		public String Icon {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the applet info
		/// </summary>
		/// <value>The name.</value>
		[XmlElement("name")]
        [JsonProperty("name")]
		public List<LocaleString> Names {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the applet info
		/// </summary>
		/// <value>The name.</value>
		[XmlElement("groupName")]
        [JsonProperty("groupName")]
		public List<LocaleString> GroupNames {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the author of the applet
		/// </summary>
		[XmlElement("author")]
        [JsonProperty("author")]
		public String Author {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the applet's dependencies
		/// </summary>
		[XmlElement("dependency")]
		public List<AppletName> Dependencies {
			get;
			set;
		}


		/// <summary>
		/// Return this applet reference
		/// </summary>
		public AppletName AsReference()
		{
			return new AppletName (this.Id, this.Version, this.PublicKeyToken);
		}
	}

}

