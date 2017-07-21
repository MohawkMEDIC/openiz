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
	/// Represents identifiers for the built-in concept sets for OpenIZ
	/// </summary>
	public static class ConceptSetKeys
	{
		/// <summary>
		/// Represents codes which are used to classify acts
		/// </summary>
		public static readonly Guid ActClass = Guid.Parse("62C5FDE0-A3AA-45DF-94E9-242F4451644A");

		/// <summary>
		/// Represents codes which are used to represent clinical interpretations 
		/// </summary>
		public static readonly Guid ActInterpretation = Guid.Parse("404BF87C-E7A6-4A5A-89CF-02E6804555A0");

		/// <summary>
		/// Represents codes which are used to dictate the mood (mode) of an act
		/// </summary>
		public static readonly Guid ActMood = Guid.Parse("E6A8E44F-0A57-4EBD-80A9-5C53B7A03D76");

		/// <summary>
		/// Represents codes which are used to relate two acts together
		/// </summary>
		public static readonly Guid ActRelationshipType = Guid.Parse("CF686A21-86E5-41E7-AF07-0016A054227A");

		/// <summary>
		/// Represents codes which are used to dictate the status of an act
		/// </summary>
		public static readonly Guid ActStatus = Guid.Parse("93A48F6A-6808-4C70-83A2-D02178C2A883");

		/// <summary>
		/// Represents codes which are used qualify the components of an address
		/// </summary>
		public static readonly Guid AddressComponentType = Guid.Parse("5CCA5869-8A7B-47A3-83DB-041D5AF5C9DA");

		/// <summary>
		/// Represents codes which are used to dictate the intentded use of an address
		/// </summary>
		public static readonly Guid AddressUse = Guid.Parse("C68A7690-D78D-4AFC-8A36-1EBDFB86F15F");

		/// <summary>
		/// Represents codes which are used to classify the gender of an entity
		/// </summary>
		public static readonly Guid AdministrativeGenderCode = Guid.Parse("E9EECD3C-7B80-47F9-9CB6-55C8D3110FB0");

		/// <summary>
		/// Represents codes which are used to classify the current state of a concept
		/// </summary>
		public static readonly Guid ConceptStatus = Guid.Parse("AAE906AA-27B3-4CDB-AFF1-F08B0FD31E59");

		/// <summary>
		/// Represents the container cap concept set.
		/// </summary>
		public static readonly Guid ContainerCap = Guid.Parse("5737CA40-57DC-4590-AB3B-54C9AFCCD55A");

		/// <summary>
		/// Represents the container separator concept set.
		/// </summary>
		public static readonly Guid ContainerSeparatorTypeKeys = Guid.Parse("B9AC7826-5D19-421E-A848-61C3C8F563A2");

		/// <summary>
		/// Represents concepts which are used to classify entities
		/// </summary>
		public static readonly Guid EntityClass = Guid.Parse("4E6DA567-0094-4F23-8555-11DA499593AF");

		/// <summary>
		/// Represents concepts which are used to classify the manner in which entities are related to ne another.
		/// </summary>
		public static readonly Guid EntityRelationshipType = Guid.Parse("EE16A667-2085-440A-B1E7-4032D10B9F40");

		/// <summary>
		/// Represents concepts which are used to classify the current status of an entity
		/// </summary>
		public static readonly Guid EntityStatus = Guid.Parse("C7578340-A8FF-4D7D-8105-581016324E68");

		/// <summary>
		/// Represents concepts which are entity relationships used by family members of entities
		/// </summary>
		public static readonly Guid FamilyMember = Guid.Parse("d3692f40-1033-48ea-94cb-31fc0f352a4e");

		/// <summary>
		/// Represents concepts which dictate a partcular industry in which an oganization operates
		/// </summary>
		public static readonly Guid IndustryCode = Guid.Parse("D1597E50-845A-46E1-B9AE-6F99FF93D9DB");

		/// <summary>
		/// Represents concepts which classify the intended use of a name
		/// </summary>
		public static readonly Guid NameUse = Guid.Parse("8DF14280-3D05-45A6-BFAE-15B63DFC379F");

		/// <summary>
		/// Represents concepts which define services provided by organizations
		/// </summary>
		public static readonly Guid ServiceCode = Guid.Parse("95F9A19A-FA85-4AF7-9342-4BA3AF0DE72A");

		/// <summary>
		/// Represents concepts which identify the type of telecommunications equipment used
		/// </summary>
		public static readonly Guid TelecomAddressType = Guid.Parse("0D79B02C-6444-40B5-ACA4-4009FB03AD54");

        /// <summary>
        /// Represents concept which classify the intended use of a telecommunications address
        /// </summary>
        public static readonly Guid TelecomAddressUse = Guid.Parse("1DABE3E2-44B8-4C45-9102-25EA147E5710");

        /// <summary>
        /// Represents concepts which classify vaccination types
        /// </summary>
        public static readonly Guid VaccineTypeCodes = Guid.Parse("ab16722f-dcf5-4f5a-9957-8f87dbb390d5");

        /// <summary>
        /// Represents concepts which distinguish observations as vital signs measurements
        /// </summary>
        public static readonly Guid VitalSigns = Guid.Parse("c9791a94-7a04-4276-804d-82589b6d0be1");

        /// <summary>
        /// Reprsents concept which distinguish observations of problems or conditions
        /// </summary>
        public static readonly Guid ProblemObservations = Guid.Parse("952D89FA-9324-4008-9452-3EB6780B6EA0");

        /// <summary>
        /// Acts which represent adverse events such as diagnosis of intolerances, etc.
        /// </summary>
        public static readonly Guid AdverseEventActs = Guid.Parse("C182B380-2D77-48AA-A128-DFF0231080B6");

        /// <summary>
        /// Allergy and intolerance types
        /// </summary>
        public static readonly Guid AllergyIntoleranceTypes = Guid.Parse("4a3952e0-8998-4020-9da2-6d78b292da69");

        /// <summary>
        /// Orgnaiztaion types
        /// </summary>
        public static readonly Guid PlaceTypes = Guid.Parse("C65719EC-0795-47EC-8AAF-DCB867C5CA56");

        /// <summary>
        /// Orgnaiztaion types
        /// </summary>
        public static readonly Guid OrganizationTypes = Guid.Parse("485BC177-3E84-4A5A-9F25-466BA30E987E");
    }
}