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
 * Date: 2016-6-14
 */
using System;

namespace OpenIZ.Core.Model.Constants
{
	/// <summary>
	/// Determiner codes classify an entity into one of three categories. 
	/// </summary>
	public static class DeterminerKeys
	{
		/// <summary>
		/// Indicates the entity is not a particular instance of a thing, rather a type of thing
		/// </summary>
		public static readonly Guid Described = Guid.Parse("AD28A7AC-A66B-42C4-91B4-DE40A2B11980");

		/// <summary>
		/// Indicates the entity is a type of thing that has been qualified further
		/// </summary>
		public static readonly Guid DescribedQualified = Guid.Parse("604CF1B7-8891-49FB-B95F-3E4E875691BC");

		/// <summary>
		/// Indicates the entity is a specific instance of a thing
		/// </summary>
		public static readonly Guid Specific = Guid.Parse("F29F08DE-78A7-4A5E-AEAF-7B545BA19A09");
	}
}