/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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

