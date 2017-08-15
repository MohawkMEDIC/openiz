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
 * Date: 2017-4-4
 */

using OpenIZ.Reporting.Jasper.Model.Reference;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Reporting.Jasper.Model.Core
{
	/// <summary>
	/// Represents a report unit.
	/// </summary>
	[XmlType("reportUnit")]
	[XmlRoot("reportUnit")]
	public class ReportUnit : ResourceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportUnit"/> class.
		/// </summary>
		public ReportUnit()
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether [always prompt controls].
		/// </summary>
		/// <value><c>true</c> if [always prompt controls]; otherwise, <c>false</c>.</value>
		[XmlElement("alwaysPromptControls")]
		public bool AlwaysPromptControls { get; set; }

		/// <summary>
		/// Gets or sets the controls layout.
		/// </summary>
		/// <value>The controls layout.</value>
		[XmlElement("controlsLayout")]
		public string ControlsLayout { get; set; }

		/// <summary>
		/// Gets or sets the data source reference.
		/// </summary>
		/// <value>The data source reference.</value>
		[XmlElement("dataSourceReference")]
		public DataSourceReference DataSourceReference { get; set; }

		/// <summary>
		/// Gets or sets the input control references.
		/// </summary>
		/// <value>The input control references.</value>
		[XmlArray("inputControls")]
		public List<InputControlReference> InputControlReferences { get; set; }

		/// <summary>
		/// Gets or sets the input control rendering view.
		/// </summary>
		/// <value>The input control rendering view.</value>
		[XmlElement("inputControlRenderingView")]
		public string InputControlRenderingView { get; set; }

		/// <summary>
		/// Gets or sets the JR XML.
		/// </summary>
		/// <value>The JR XML.</value>
		[XmlElement("jrxmlFileReference")]
		public JrXmlFileReference JrXmlFileReference { get; set; }

		/// <summary>
		/// Gets or sets the query reference.
		/// </summary>
		/// <value>The query reference.</value>
		[XmlElement("queryReference")]
		public QueryReference QueryReference { get; set; }

		/// <summary>
		/// Gets or sets the report rendering view.
		/// </summary>
		/// <value>The report rendering view.</value>
		[XmlElement("reportRenderingView")]
		public string ReportRenderingView { get; set; }

		/// <summary>
		/// Gets or sets the resources.
		/// </summary>
		/// <value>The resources.</value>
		[XmlArray("resources")]
		public List<ResourceFileReference> Resources { get; set; }
	}
}