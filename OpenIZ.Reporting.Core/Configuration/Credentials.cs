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
 * Date: 2017-4-10
 */
using System.Xml.Serialization;

namespace OpenIZ.Reporting.Core.Configuration
{
	/// <summary>
	/// Represents a set of credentials.
	/// </summary>
	[XmlType(nameof(Credentials), Namespace = "http://openiz.org/reporting")]
	public class Credentials
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Credentials"/> class.
		/// </summary>
		public Credentials()
		{
		}

		/// <summary>
		/// Gets or sets the credential.
		/// </summary>
		/// <value>The credential.</value>
		[XmlElement("credential")]
		public CredentialBase Credential { get; set; }

		/// <summary>
		/// Gets or sets the type of the credential.
		/// </summary>
		/// <value>The type of the credential.</value>
		[XmlAttribute("type")]
		public CredentialType CredentialType { get; set; }
	}
}