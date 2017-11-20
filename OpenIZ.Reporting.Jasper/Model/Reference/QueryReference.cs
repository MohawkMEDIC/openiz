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

namespace OpenIZ.Reporting.Jasper.Model.Reference
{
	/// <summary>
	/// Represents a query reference.
	/// </summary>
	/// <seealso cref="OpenIZ.Reporting.Jasper.Model.Reference.ReferenceBase" />
	[XmlType("queryReference")]
	public class QueryReference : ReferenceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QueryReference"/> class.
		/// </summary>
		public QueryReference()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryReference"/> class.
		/// </summary>
		/// <param name="uri">The URI.</param>
		public QueryReference(string uri) : base(uri)
		{
		}
	}
}