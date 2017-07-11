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
 * Date: 2016-8-2
 */
using System;

namespace OpenIZ.Core.Model.Constants
{
	/// <summary>
	/// Phonetic algorithm keys for built in phonetic algorithms in OpenIZ
	/// </summary>
	public static class PhoneticAlgorithmKeys
	{
		/// <summary>
		/// Represents the metaphone phonetic algorithm
		/// </summary>
		public static readonly Guid Metaphone = Guid.Parse("d79a4dc6-66a6-4602-8fcb-7dc09a895793");

		/// <summary>
		/// Represents the null phonetic algorithm
		/// </summary>
		public static readonly Guid None = Guid.Parse("402CD339-D0E4-46CE-8FC2-12A4B0E17226");

		/// <summary>
		/// Represents the soundex algorithm
		/// </summary>
		public static readonly Guid Soundex = Guid.Parse("3352a79a-d2e0-4e0c-9b48-6fd2a202c681");
	}
}