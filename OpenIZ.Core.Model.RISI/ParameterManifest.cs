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
 * Date: 2017-1-5
 */

using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI
{
	/// <summary>
	/// Represents a parameter manifest.
	/// </summary>
	[XmlType(nameof(ParameterManifest), Namespace = "http://openiz.org/risi")]
	[XmlRoot(nameof(ParameterManifest), Namespace = "http://openiz.org/risi")]
	public class ParameterManifest
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterManifest"/> class.
		/// </summary>
		public ParameterManifest()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterManifest"/> class
		/// with a specific <see cref="ParameterDefinition"/> instance.
		/// </summary>
		/// <param name="parameterDefinition">The parameter definition.</param>
		public ParameterManifest(ParameterDefinition parameterDefinition)
		{
			this.ParameterDefinition = parameterDefinition;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterManifest"/> class
		/// with a specific <see cref="ParameterTypeDefinition"/> instance.
		/// </summary>
		/// <param name="parameterTypeDefinition">The parameter type definition.</param>
		public ParameterManifest(ReportDataType parameterTypeDefinition)
		{
			this.ParameterTypeDefinition = parameterTypeDefinition;
		}

		/// <summary>
		/// Gets or sets the parameter definition.
		/// </summary>
		[XmlElement("parameterDefinition", Type = typeof(ParameterDefinition))]
		public ParameterDefinition ParameterDefinition { get; set; }

		/// <summary>
		/// Gets or sets the parameter type definition.
		/// </summary>
		[XmlElement("parameterTypeDefinition", Type = typeof(ReportDataType))]
		public ReportDataType ParameterTypeDefinition { get; set; }
	}
}