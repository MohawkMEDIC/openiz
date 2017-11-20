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
	/// Name component type keys
	/// </summary>
	public static class NameComponentKeys
	{
        /// <summary>
        /// The name component represents a delimeter in a name such as hyphen or space
        /// </summary>
		public static readonly Guid Delimiter = Guid.Parse("4C6B9519-A493-44A9-80E6-32D85109B04B");
        /// <summary>
        /// The name component represents the surname
        /// </summary>
		public static readonly Guid Family = Guid.Parse("29B98455-ED61-49F8-A161-2D73363E1DF0");
        /// <summary>
        /// The name component represents the given name
        /// </summary>
		public static readonly Guid Given = Guid.Parse("2F64BDE2-A696-4B0A-9690-B21EBD7E5092");
        /// <summary>
        /// The name component represents the prefix such as Von or Van
        /// </summary>
		public static readonly Guid Prefix = Guid.Parse("A787187B-6BE4-401E-8836-97FC000C5D16");
        /// <summary>
        /// The name component represents a suffix such as III or Esq.
        /// </summary>
		public static readonly Guid Suffix = Guid.Parse("064523DF-BB03-4932-9323-CDF0CC9590BA");
        /// <summary>
        /// The name component represents a formal title like Mr, Dr, Capt.
        /// </summary>
		public static readonly Guid Title = Guid.Parse("4386D92A-D81B-4033-B968-01E57E20D5E0");
	}

	/// <summary>
	/// Name use keys
	/// </summary>
	public static class NameUseKeys
	{
		/// <summary>
		/// The name used is an alphabetic representation of the name (ex: romaji in Japanese)
		/// </summary>
		public static readonly Guid Alphabetic = Guid.Parse("71D1C07C-6EE6-4240-8A95-19F96583512E");

		/// <summary>
		/// The name is an anonymous name for the object (not the real name but a name used for care delivery)
		/// </summary>
		public static readonly Guid Anonymous = Guid.Parse("95E6843A-26FF-4046-B6F4-EB440D4B85F7");

		/// <summary>
		/// The name represents an artist name or stage name
		/// </summary>
		public static readonly Guid Artist = Guid.Parse("4A7BF199-F33B-42F9-8B99-32433EA67BD7");

		/// <summary>
		/// The name represents an assigned name (given or bestowed by an authority)
		/// </summary>
		public static readonly Guid Assigned = Guid.Parse("A87A6D21-2CA6-4AEA-88F3-6135CCEB58D1");

		/// <summary>
		/// THe name represents an ideographic representation of the name
		/// </summary>
		public static readonly Guid Ideographic = Guid.Parse("09000479-4672-44F8-BB4A-72FB25F7356A");

		/// <summary>
		/// The name is an indigenous name or tribal name for the patient
		/// </summary>
		public static readonly Guid Indigenous = Guid.Parse("A3FB2A05-5EBE-47AE-AFD0-4C1B22336090");

		/// <summary>
		/// The name represents the current legal name of an object (such as a corporate name)
		/// </summary>
		public static readonly Guid Legal = Guid.Parse("EFFE122D-8D30-491D-805D-ADDCB4466C35");

		/// <summary>
		/// The name represents a name as displayed on a license or known to a license authority
		/// </summary>
		public static readonly Guid License = Guid.Parse("48075D19-7B29-4CA5-9C73-0CBD31248446");

		/// <summary>
		/// THe name is a maiden name (name of a patient before marriage)
		/// </summary>
		public static readonly Guid MaidenName = Guid.Parse("0674C1C8-963A-4658-AFF9-8CDCD308FA68");

		/// <summary>
		/// The name as it appears on an official record
		/// </summary>
		public static readonly Guid OfficialRecord = Guid.Parse("1EC9583A-B019-4BAA-B856-B99CAF368656");

		/// <summary>
		/// The name represents a phonetic representation of a name such as a SOUNDEX code
		/// </summary>
		public static readonly Guid Phonetic = Guid.Parse("2B085D38-3308-4664-9F89-48D8EF4DABA7");

		/// <summary>
		/// The name is a pseudonym for the object or an synonym name
		/// </summary>
		public static readonly Guid Pseudonym = Guid.Parse("C31564EF-CA8D-4528-85A8-88245FCEF344");

        /// <summary>
        /// The name is to be used for religious purposes (such as baptismal name)
        /// </summary>
		public static readonly Guid Religious = Guid.Parse("15207687-5290-4672-A7DF-2880A23DCBB5");

        /// <summary>
        /// The name is to be used in the performing of matches only
        /// </summary>
		public static readonly Guid Search = Guid.Parse("87964BFF-E442-481D-9749-69B2A84A1FBE");

        /// <summary>
        /// The name represents the computed soundex code of a name
        /// </summary>
		public static readonly Guid Soundex = Guid.Parse("E5794E3B-3025-436F-9417-5886FEEAD55A");

		/// <summary>
		/// The name represents a syllabic name.
		/// </summary>
		public static readonly Guid Syllabic = Guid.Parse("B4CA3BF0-A7FC-44F3-87D5-E126BEDA93FF");
	}
}