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
 * User: khannan
 * Date: 2017-10-30
 */

using OpenIZ.Core.Model.RISI;
using System.Collections.Generic;

namespace OpenIZ.Reporting.Jasper
{
	/// <summary>
	/// Represents a report definition equality comparer.
	/// </summary>
	/// <seealso cref="System.Collections.Generic.IEqualityComparer{OpenIZ.Core.Model.RISI.ReportDefinition}" />
	public class ReportDefinitionEqualityComparer : IEqualityComparer<ReportDefinition>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportDefinitionEqualityComparer"/> class.
		/// </summary>
		public ReportDefinitionEqualityComparer()
		{
			
		}

		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <param name="x">The first object of type <paramref name="ReportDefinition" /> to compare.</param>
		/// <param name="y">The second object of type <paramref name="ReportDefinition" /> to compare.</param>
		/// <returns>true if the specified objects are equal; otherwise, false.</returns>
		public bool Equals(ReportDefinition x, ReportDefinition y)
		{
			return x?.CorrelationId == y?.CorrelationId;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
		/// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
		public int GetHashCode(ReportDefinition obj)
		{
			return obj.CorrelationId.GetHashCode();
		}
	}
}