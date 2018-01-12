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
 * User: fyfej
 * Date: 2017-9-1
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Represents act reasons (reasons for an act)
    /// </summary>
    public static class ActReasonKeys
    {
        /// <summary>
        /// The patient started too late for the therapy
        /// </summary>
        public static Guid StartedTooLate = Guid.Parse("B75BF533-9804-4450-83C7-23F0332F87B8");
        /// <summary>
        /// The patient is allergic or intolerant to the consumable
        /// </summary>
        public static Guid AllergyOrIntolerance = Guid.Parse("4FF3617B-BB91-4F3F-B4D2-2425F477037F");
        /// <summary>
        /// The vaccine or drug was expired
        /// </summary>
        public static Guid Expired = Guid.Parse("4B518938-B1EA-44E3-B837-31617FA188A4");
        /// <summary>
        /// The vaccine was considered unsafe
        /// </summary>
        public static Guid VaccineSafety = Guid.Parse("C6718DF8-C8C0-49FD-A73D-52F6981CCBF7");
        /// <summary>
        /// The vaccine was not performed per the professional judgement of the provider
        /// </summary>
        public static Guid ProfessionalJudgement = Guid.Parse("9D947E6D-8406-42F3-BB8A-634FB3C81A08");
        /// <summary>
        /// The patient had a religious objection
        /// </summary>
        public static Guid ReligiousObjecton = Guid.Parse("0D40C2B6-7CEB-4492-AB2A-6E7C730EAF22");
        /// <summary>
        /// The patient refused the treatment
        /// </summary>
        public static Guid PatientRefused = Guid.Parse("42351A36-F60F-4687-B334-7A41B091BAE1");
        /// <summary>
        /// There was insufficient stock to perform the action
        /// </summary>
        public static Guid OutOfStock = Guid.Parse("C7469FAD-F190-40A2-A28D-F97D1863E8CF");
        /// <summary>
        /// The items are broken and can no longer be used to deliver care
        /// </summary>
        public static Guid Broken = Guid.Parse("DCFF308D-CCA5-4EB3-AD92-770917D88E56");
        /// <summary>
        /// There was a cold-storage failure which resulted in the material being unusable.
        /// </summary>
        public static Guid ColdStorageFailure = Guid.Parse("06922EAC-0CAE-49AF-A33C-FC7096349E4A");
    }
}
