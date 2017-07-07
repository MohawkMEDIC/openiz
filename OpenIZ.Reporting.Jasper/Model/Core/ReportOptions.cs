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
 * Date: 2017-4-4
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Reporting.Jasper.Model.Core
{
	/// <summary>
	/// Class ReportOptions.
	/// </summary>
	/// <seealso cref="OpenIZ.Reporting.Jasper.Model.ResourceBase" />
	[XmlType("reportOptions")]
	[XmlRoot("reportOptions")]
	public class ReportOptions : ResourceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportOptions"/> class.
		/// </summary>
		public ReportOptions()
		{
				
		}

		/// <summary>
		/// Gets or sets the report URI.
		/// </summary>
		/// <value>The report URI.</value>
		[XmlElement("reportUri")]
		public string ReportUri { get; set; }

		/// <summary>
		/// Gets or sets the report parameters.
		/// </summary>
		/// <value>The report parameters.</value>
		[XmlElement("reportParameters")]
		public List<ReportParameter> ReportParameters { get; set; }
	}
}