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
 * Date: 2017-4-22
 */
using OpenIZ.Reporting.Jasper.Attributes;

namespace OpenIZ.Reporting.Jasper.Model.Core
{
	/// <summary>
	/// Represents jasper parameter types.
	/// </summary>
	public enum ReportParameterType
	{
		/// <summary>
		/// Represents a date parameter.
		/// </summary>
		[StringValue("date")]
		Date,

		/// <summary>
		/// Represents a text parameter.
		/// </summary>
		[StringValue("text")]
		Text,

		/// <summary>
		/// Represents a date time parameter.
		/// </summary>
		[StringValue("datetime")]
		DateTime
	}
}