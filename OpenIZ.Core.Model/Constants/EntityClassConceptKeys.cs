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
	/// Entity class concept keys
	/// </summary>
	public static class EntityClassKeyStrings
    {
        /// <summary>
        /// Animal
        /// </summary>
        public const string Animal = "61fcbf42-b5e0-4fb5-9392-108a5c6dbec7";

        /// <summary>
        /// Chemical Substance
        /// </summary>
        public const string ChemicalSubstance = "2e9fa332-9391-48c6-9fc8-920a750b25d3";

        /// <summary>
        /// City or town
        /// </summary>
        public const string CityOrTown = "79dd4f75-68e8-4722-a7f5-8bc2e08f5cd6";

        /// <summary>
        /// Container
        /// </summary>
        public const string Container = "b76ff324-b174-40b7-a6ac-d1fdf8e23967";

        /// <summary>
        /// Country or nation
        /// </summary>
        public const string Country = "48b2ffb3-07db-47ba-ad73-fc8fb8502471";

        /// <summary>
        /// County or parish
        /// </summary>
        public const string CountyOrParish = "d9489d56-ddac-4596-b5c6-8f41d73d8dc5";

        /// <summary>
        /// Device
        /// </summary>
        public const string Device = "1373ff04-a6ef-420a-b1d0-4a07465fe8e8";

        /// <summary>
        /// Entity
        /// </summary>
        public const string Entity = "e29fcfad-ec1d-4c60-a055-039a494248ae";

        /// <summary>
        /// Food
        /// </summary>
        public const string Food = "e5a09cc2-5ae5-40c2-8e32-687dba06715d";

        /// <summary>
        /// Living Subject
        /// </summary>
        public const string LivingSubject = "8ba5e5c9-693b-49d4-973c-d7010f3a23ee";

        /// <summary>
        /// Manufactured material
        /// </summary>
        public const string ManufacturedMaterial = "fafec286-89d5-420b-9085-054aca9d1eef";

        /// <summary>
        /// Material
        /// </summary>
        public const string Material = "d39073be-0f8f-440e-b8c8-7034cc138a95";

        /// <summary>
        /// Non living subject
        /// </summary>
        public const string NonLivingSubject = "9025e5c9-693b-49d4-973c-d7010f3a23ee";

        /// <summary>
        /// Organization
        /// </summary>
        public const string Organization = "7c08bd55-4d42-49cd-92f8-6388d6c4183f";

        /// <summary>
        /// Patient
        /// </summary>
        public const string Patient = "bacd9c6f-3fa9-481e-9636-37457962804d";

        /// <summary>
        /// Person
        /// </summary>
        public const string Person = "9de2a846-ddf2-4ebc-902e-84508c5089ea";

        /// <summary>
        /// Place
        /// </summary>
        public const string Place = "21ab7873-8ef3-4d78-9c19-4582b3c40631";

        /// <summary>
        /// Service delivery location
        /// </summary>
        public const string Provider = "6b04fed8-c164-469c-910b-f824c2bda4f0";

        /// <summary>
        /// Service delivery location
        /// </summary>
        public const string ServiceDeliveryLocation = "ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c";

        /// <summary>
        /// State
        /// </summary>
        public const string State = "8cf4b0b0-84e5-4122-85fe-6afa8240c218";
    }


    /// <summary>
    /// Entity class concept keys
    /// </summary>
    // TODO: Refactor these
    public static class EntityClassKeys
	{
		/// <summary>
		/// Animal
		/// </summary>
		public static readonly Guid Animal = Guid.Parse("61FCBF42-B5E0-4FB5-9392-108A5C6DBEC7");

		/// <summary>
		/// Chemical Substance
		/// </summary>
		public static readonly Guid ChemicalSubstance = Guid.Parse("2E9FA332-9391-48C6-9FC8-920A750B25D3");

		/// <summary>
		/// City or town
		/// </summary>
		public static readonly Guid CityOrTown = Guid.Parse("79DD4F75-68E8-4722-A7F5-8BC2E08F5CD6");

		/// <summary>
		/// Container
		/// </summary>
		public static readonly Guid Container = Guid.Parse("B76FF324-B174-40B7-A6AC-D1FDF8E23967");

		/// <summary>
		/// Country or nation
		/// </summary>
		public static readonly Guid Country = Guid.Parse("48B2FFB3-07DB-47BA-AD73-FC8FB8502471");

		/// <summary>
		/// County or parish
		/// </summary>
		public static readonly Guid CountyOrParish = Guid.Parse("D9489D56-DDAC-4596-B5C6-8F41D73D8DC5");

		/// <summary>
		/// Device
		/// </summary>
		public static readonly Guid Device = Guid.Parse("1373FF04-A6EF-420A-B1D0-4A07465FE8E8");

		/// <summary>
		/// Entity
		/// </summary>
		public static readonly Guid Entity = Guid.Parse("E29FCFAD-EC1D-4C60-A055-039A494248AE");

		/// <summary>
		/// Food
		/// </summary>
		public static readonly Guid Food = Guid.Parse("E5A09CC2-5AE5-40C2-8E32-687DBA06715D");

		/// <summary>
		/// Living Subject
		/// </summary>
		public static readonly Guid LivingSubject = Guid.Parse("8BA5E5C9-693B-49D4-973C-D7010F3A23EE");

		/// <summary>
		/// Manufactured material
		/// </summary>
		public static readonly Guid ManufacturedMaterial = Guid.Parse("FAFEC286-89D5-420B-9085-054ACA9D1EEF");

		/// <summary>
		/// Material
		/// </summary>
		public static readonly Guid Material = Guid.Parse("D39073BE-0F8F-440E-B8C8-7034CC138A95");

		/// <summary>
		/// Non living subject
		/// </summary>
		public static readonly Guid NonLivingSubject = Guid.Parse("9025E5C9-693B-49D4-973C-D7010F3A23EE");

		/// <summary>
		/// Organization
		/// </summary>
		public static readonly Guid Organization = Guid.Parse("7C08BD55-4D42-49CD-92F8-6388D6C4183F");

		/// <summary>
		/// Patient
		/// </summary>
		public static readonly Guid Patient = Guid.Parse("BACD9C6F-3FA9-481E-9636-37457962804D");

		/// <summary>
		/// Person
		/// </summary>
		public static readonly Guid Person = Guid.Parse("9DE2A846-DDF2-4EBC-902E-84508C5089EA");

		/// <summary>
		/// Place
		/// </summary>
		public static readonly Guid Place = Guid.Parse("21AB7873-8EF3-4D78-9C19-4582B3C40631");

		/// <summary>
		/// Service delivery location
		/// </summary>
		public static readonly Guid Provider = Guid.Parse("6B04FED8-C164-469C-910B-F824C2BDA4F0");

		/// <summary>
		/// Service delivery location
		/// </summary>
		public static readonly Guid ServiceDeliveryLocation = Guid.Parse("FF34DFA7-C6D3-4F8B-BC9F-14BCDC13BA6C");

		/// <summary>
		/// State
		/// </summary>
		public static readonly Guid State = Guid.Parse("8CF4B0B0-84E5-4122-85FE-6AFA8240C218");
	}
}