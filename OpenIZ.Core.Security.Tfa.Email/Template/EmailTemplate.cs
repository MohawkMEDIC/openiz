/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-11-30
 */
using System;
using System.IO;
using System.Xml.Serialization;

namespace OpenIZ.Core.Security.Tfa.Email.Template
{
	/// <summary>
	/// E-mail template
	/// </summary>
	[XmlType(nameof(EmailTemplate), Namespace = "http://openiz.org/tfa/email/template")]
	[XmlRoot(nameof(EmailTemplate), Namespace = "http://openiz.org/tfa/email/template")]
	public class EmailTemplate
	{
		/// <summary>
		/// Gets or sets the body
		/// </summary>
		[XmlElement("body")]
		public String Body { get; set; }

		/// <summary>
		/// Gets or sets the from element
		/// </summary>
		[XmlElement("from")]
		public String From { get; set; }

		/// <summary>
		/// Gets or sets the subject
		/// </summary>
		[XmlElement("subject")]
		public String Subject { get; set; }

		/// <summary>
		/// Load the specified e-mail template
		/// </summary>
		public static EmailTemplate Load(string fileName)
		{
			using (var fs = File.OpenRead(fileName))
			{
				XmlSerializer xsz = new XmlSerializer(typeof(EmailTemplate));
				return xsz.Deserialize(fs) as EmailTemplate;
			}
		}
	}
}