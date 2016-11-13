/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
	/// Represents concept set identifiers
	/// </summary>
	public static class ConceptSetKeys
	{
		/// <summary>
		/// Act Classiciation
		/// </summary>
		public static readonly Guid ActClass = Guid.Parse("62C5FDE0-A3AA-45DF-94E9-242F4451644A");

		/// <summary>
		/// Act interpretation
		/// </summary>
		public static readonly Guid ActInterpretation = Guid.Parse("404BF87C-E7A6-4A5A-89CF-02E6804555A0");

		/// <summary>
		/// Act mood
		/// </summary>
		public static readonly Guid ActMood = Guid.Parse("E6A8E44F-0A57-4EBD-80A9-5C53B7A03D76");

		/// <summary>
		/// Act relationship type
		/// </summary>
		public static readonly Guid ActRelationshipType = Guid.Parse("CF686A21-86E5-41E7-AF07-0016A054227A");

		/// <summary>
		/// Act status
		/// </summary>
		public static readonly Guid ActStatus = Guid.Parse("93A48F6A-6808-4C70-83A2-D02178C2A883");

		/// <summary>
		/// Address component type
		/// </summary>
		public static readonly Guid AddressComponentType = Guid.Parse("5CCA5869-8A7B-47A3-83DB-041D5AF5C9DA");

		/// <summary>
		/// Address use id
		/// </summary>
		public static readonly Guid AddressUse = Guid.Parse("C68A7690-D78D-4AFC-8A36-1EBDFB86F15F");

		/// <summary>
		/// Administrative gender code
		/// </summary>
		public static readonly Guid AdministrativeGenderCode = Guid.Parse("E9EECD3C-7B80-47F9-9CB6-55C8D3110FB0");

		/// <summary>
		/// Concept status
		/// </summary>
		public static readonly Guid ConceptStatus = Guid.Parse("AAE906AA-27B3-4CDB-AFF1-F08B0FD31E59");

		/// <summary>
		/// Entity classifications
		/// </summary>
		public static readonly Guid EntityClass = Guid.Parse("4E6DA567-0094-4F23-8555-11DA499593AF");

		/// <summary>
		/// Entity relationships
		/// </summary>
		public static readonly Guid EntityRelationshipType = Guid.Parse("EE16A667-2085-440A-B1E7-4032D10B9F40");

		/// <summary>
		/// Entity status identifiers
		/// </summary>
		public static readonly Guid EntityStatus = Guid.Parse("C7578340-A8FF-4D7D-8105-581016324E68");

		/// <summary>
		/// Family members
		/// </summary>
		public static readonly Guid FamilyMember = Guid.Parse("d3692f40-1033-48ea-94cb-31fc0f352a4e");

		/// <summary>
		/// Entity industry codes
		/// </summary>
		public static readonly Guid IndustryCode = Guid.Parse("D1597E50-845A-46E1-B9AE-6F99FF93D9DB");

		/// <summary>
		/// Entity user identifiers
		/// </summary>
		public static readonly Guid NameUse = Guid.Parse("8DF14280-3D05-45A6-BFAE-15B63DFC379F");

		/// <summary>
		/// Entity service codes
		/// </summary>
		public static readonly Guid ServiceCode = Guid.Parse("95F9A19A-FA85-4AF7-9342-4BA3AF0DE72A");

		/// <summary>
		/// Telecom address type
		/// </summary>
		public static readonly Guid TelecomAddressType = Guid.Parse("0D79B02C-6444-40B5-ACA4-4009FB03AD54");

		/// <summary>
		/// Telecom address use
		/// </summary>
		public static readonly Guid TelecomAddressUse = Guid.Parse("1DABE3E2-44B8-4C45-9102-25EA147E5710");
	}
}