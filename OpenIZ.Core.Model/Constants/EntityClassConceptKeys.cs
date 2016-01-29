/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-24
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Entity class concept keys
    /// </summary>
    public static class EntityClassKeys
    {
        /// <summary>
        /// Entity
        /// </summary>
        public static readonly Guid Entity = Guid.Parse("E29FCFAD-EC1D-4C60-A055-039A494248AE");
        /// <summary>
        /// Manufactured material
        /// </summary>
        public static readonly Guid ManufacturedMaterial = Guid.Parse("FAFEC286-89D5-420B-9085-054ACA9D1EEF");
        /// <summary>
        /// Animal
        /// </summary>
        public static readonly Guid Animal = Guid.Parse("61FCBF42-B5E0-4FB5-9392-108A5C6DBEC7");
        /// <summary>
        /// Place
        /// </summary>
        public static readonly Guid Place = Guid.Parse("21AB7873-8EF3-4D78-9C19-4582B3C40631");
        /// <summary>
        /// Device
        /// </summary>
        public static readonly Guid Device = Guid.Parse("1373FF04-A6EF-420A-B1D0-4A07465FE8E8");
        /// <summary>
        /// Organization
        /// </summary>
        public static readonly Guid Organization = Guid.Parse("7C08BD55-4D42-49CD-92F8-6388D6C4183F");
        /// <summary>
        /// Food
        /// </summary>
        public static readonly Guid Food = Guid.Parse("E5A09CC2-5AE5-40C2-8E32-687DBA06715D");
        /// <summary>
        /// Material
        /// </summary>
        public static readonly Guid Material = Guid.Parse("D39073BE-0F8F-440E-B8C8-7034CC138A95");
        /// <summary>
        /// Person
        /// </summary>
        public static readonly Guid Person = Guid.Parse("9DE2A846-DDF2-4EBC-902E-84508C5089EA");
        /// <summary>
        /// City or town
        /// </summary>
        public static readonly Guid CityOrTown = Guid.Parse("79DD4F75-68E8-4722-A7F5-8BC2E08F5CD6");
        /// <summary>
        /// Chemical Substance
        /// </summary>
        public static readonly Guid ChemicalSubstance = Guid.Parse("2E9FA332-9391-48C6-9FC8-920A750B25D3");
        /// <summary>
        /// State
        /// </summary>
        public static readonly Guid State = Guid.Parse("6B82E7F0-08BF-46DB-BDCF-95F69592E3BF");
        /// <summary>
        /// Container
        /// </summary>
        public static readonly Guid Container = Guid.Parse("B76FF324-B174-40B7-A6AC-D1FDF8E23967");
        /// <summary>
        /// Living Subject
        /// </summary>
        public static readonly Guid LivingSubject = Guid.Parse("8BA5E5C9-693B-49D4-973C-D7010F3A23EE");
        /// <summary>
        /// Patient
        /// </summary>
        public static readonly Guid Patient = Guid.Parse("BACD9C6F-3FA9-481E-9636-37457962804D");
        /// <summary>
        /// Service delivery location
        /// </summary>
        public static readonly Guid ServiceDeliveryLocation = Guid.Parse("FF34DFA7-C6D3-4F8B-BC9F-14BCDC13BA6C");
        /// <summary>
        /// Service delivery location
        /// </summary>
        public static readonly Guid Provider = Guid.Parse("6B04FED8-C164-469C-910B-F824C2BDA4F0");

    }
}
