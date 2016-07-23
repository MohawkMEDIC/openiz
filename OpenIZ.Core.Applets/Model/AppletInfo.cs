/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-6-14
 */
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

