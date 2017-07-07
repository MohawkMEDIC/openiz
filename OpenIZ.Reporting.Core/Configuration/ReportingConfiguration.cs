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
using System;
using System.Xml.Serialization;

namespace OpenIZ.Reporting.Core.Configuration
{
	/// <summary>
	/// Represents a configuration for a RISI configuration.
	/// </summary>
	public class ReportingConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportingConfiguration"/> class.
		/// </summary>
		public ReportingConfiguration()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportingConfiguration"/> class
		/// with a specified report engine type.
		/// </summary>
		/// <param name="address">The address of the reporting engine.</param>
		/// <param name="handler">The type of report engine.</param>
		public ReportingConfiguration(string address, Type handler)
		{
			this.Address = address;
			this.Handler = handler;
		}

		/// <summary>
		/// Gets or sets the address of the reporting engine.
		/// </summary>
		[XmlAttribute("address")]
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets the credentials.
		/// </summary>
		/// <value>The credentials.</value>
		[XmlElement("credentials")]
		public Credentials Credentials { get; set; }

		/// <summary>
		/// Gets the engine handler of the configuration.
		/// </summary>
		[XmlAttribute("type")]
		public Type Handler { get; set; }
	}
}