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
 * User: khannan
 * Date: 2017-9-6
 */

using System.Xml.Serialization;

namespace OpenIZ.Reporting.Jasper.Configuration
{
	/// <summary>
	/// Represents a jasper configuration.
	/// </summary>
	public class JasperConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JasperConfiguration"/> class.
		/// </summary>
		public JasperConfiguration()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JasperConfiguration"/> class.
		/// </summary>
		/// <param name="reportPath">The report path.</param>
		public JasperConfiguration(string reportPath)
		{
			this.ReportPath = reportPath;
		}

		/// <summary>
		/// Gets or sets the report path.
		/// </summary>
		/// <value>The report path.</value>
		[XmlAttribute("reportPath")]
		public string ReportPath { get; set; }
	}
}