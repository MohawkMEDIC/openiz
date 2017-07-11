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
	/// Concept classification identifiers for built-in concept classes
	/// </summary>
	public static class ConceptClassKeys
	{
		/// <summary>
		/// Classification codes
		/// </summary>
		public static readonly Guid ClassCode = Guid.Parse("17FD5254-8C25-4ABB-B246-083FBE9AFA15");

		/// <summary>
		/// Diagnosis codes
		/// </summary>
		public static readonly Guid Diagnosis = Guid.Parse("92CDEA39-B9A3-4A5B-BC88-A6646C74240D");

		/// <summary>
		/// Clinical findings
		/// </summary>
		public static readonly Guid Finding = Guid.Parse("E445E207-60A3-401A-9B81-A8AC2479F4A6");

		/// <summary>
		/// Form codes (shape, texture, etc.)
		/// </summary>
		public static readonly Guid Form = Guid.Parse("17EE5254-8C25-4ABB-B246-083FBE9AFA15");

		/// <summary>
		/// Material classifications
		/// </summary>
		public static readonly Guid Material = Guid.Parse("DC9CBC32-B8EA-4144-BEF1-DC618E28F4D7");

		/// <summary>
		/// Mood classifications
		/// </summary>
		public static readonly Guid Mood = Guid.Parse("BBA99722-23CE-469A-8FA5-10DEBA853D35");

		/// <summary>
		/// Other classifications
		/// </summary>
		public static readonly Guid Other = Guid.Parse("0D6B3439-C9BE-4480-AF39-EEB457C052D0");

		/// <summary>
		/// Problems or condition codes
		/// </summary>
		public static readonly Guid Problem = Guid.Parse("4BD7F8E6-E4B8-4DBC-93A7-CF14FBAF9700");

		/// <summary>
		/// Relationship class identifier
		/// </summary>
		public static readonly Guid Relationship = Guid.Parse("F51DFDCD-039B-4E1F-90BE-3CF56AEF8DA4");

		/// <summary>
		/// Routes of adminstration class identifier
		/// </summary>
		public static readonly Guid Route = Guid.Parse("A8A900D3-A07E-4E02-B45F-580D09BAF047");

		/// <summary>
		/// Status codes 
		/// </summary>
		public static readonly Guid Status = Guid.Parse("54B93182-FC19-47A2-82C6-089FD70A4F45");

		/// <summary>
		/// Stock classification codes
		/// </summary>
		public static readonly Guid Stock = Guid.Parse("FFD8304A-43EC-4EBC-95FC-FB4A4F2338F0");

		/// <summary>
		/// Unit of measure classification
		/// </summary>
		public static readonly Guid UnitOfMeasure = Guid.Parse("1EF69347-EF03-4FF7-B3C5-6334448845E6");
	}
}