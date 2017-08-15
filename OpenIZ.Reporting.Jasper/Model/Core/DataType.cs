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

using System.Xml.Serialization;

namespace OpenIZ.Reporting.Jasper.Model.Core
{
	/// <summary>
	/// Represents a data type.
	/// </summary>
	/// <seealso cref="OpenIZ.Reporting.Jasper.Model.ResourceBase" />
	[XmlType("dataType")]
	[XmlRoot("dataType")]
	public class DataType : ResourceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataType"/> class.
		/// </summary>
		public DataType()
		{
		}

		/// <summary>
		/// Gets or sets the maximum length.
		/// </summary>
		/// <value>The maximum length.</value>
		[XmlElement("maxLength")]
		public int MaxLength { get; set; }

		/// <summary>
		/// Gets or sets the maximum value.
		/// </summary>
		/// <value>The maximum value.</value>
		[XmlElement("maxValue")]
		public string MaxValue { get; set; }

		/// <summary>
		/// Gets or sets the minimum value.
		/// </summary>
		/// <value>The minimum value.</value>
		[XmlElement("minValue")]
		public string MinValue { get; set; }

		/// <summary>
		/// Gets or sets the pattern.
		/// </summary>
		/// <value>The pattern.</value>
		[XmlElement("pattern")]
		public string Pattern { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [strict maximum].
		/// </summary>
		/// <value><c>true</c> if [strict maximum]; otherwise, <c>false</c>.</value>
		[XmlElement("strictMax")]
		public bool StrictMax { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [strict minimum].
		/// </summary>
		/// <value><c>true</c> if [strict minimum]; otherwise, <c>false</c>.</value>
		[XmlElement("strictMin")]
		public bool StrictMin { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		[XmlElement("type")]
		public string Type { get; set; }
	}
}