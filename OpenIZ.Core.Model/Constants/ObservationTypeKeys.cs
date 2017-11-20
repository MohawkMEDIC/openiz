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
 * User: fyfej
 * Date: 2017-7-9
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
	/// <summary>
	/// Represents act type keys related to types of observations that can be made about a patient
	/// </summary>
	public static class ObservationTypeKeys
    {

		/// <summary>
		/// The observation is a condition which the patient has
		/// </summary>
		public static readonly Guid Condition = Guid.Parse("236b5641-61d2-4d12-91f7-5dddbd7f8931");
		/// <summary>
		/// The observation is a symptom which the patient is presenting
		/// </summary>
		public static readonly Guid Symptom = Guid.Parse("10a0fb51-687d-41ec-8d50-ad6549e2ae58");
		/// <summary>
		/// The observtion represents a concrete clinical finding (after performing tests for example)
		/// </summary>
		public static readonly Guid Finding = Guid.Parse("5dbd3949-fda0-4c5d-a849-a673fd5565f6");
		/// <summary>
		/// The observation represents a complaint that the patient has (example: back pain)
		/// </summary>
		public static readonly Guid Complaint = Guid.Parse("402051ae-fa84-45b7-ac3b-586d1323ebe7");
		/// <summary>
		/// The observation represents a functional limitation of the patient (example: paraplegia)
		/// </summary>
		public static readonly Guid Functionallimitation = Guid.Parse("bfc26b1f-af4c-4d50-a084-eb0d9eabd519");
		/// <summary>
		/// The observation represents a known problem that the patient has reported
		/// </summary>
		public static readonly Guid Problem = Guid.Parse("260ffe90-7882-4b38-a7af-d2110e91e752");
		/// <summary>
		/// The observation represents a concrete clinical diagnosis of the patient
		/// </summary>
		public static readonly Guid Diagnosis = Guid.Parse("d5e0a5be-d227-413a-a752-b7d79d7d4ef3");
		/// <summary>
		/// The observation represents a severity 
		/// </summary>
		public static readonly Guid Severity = Guid.Parse("05012084-3351-4045-8390-fbcbd7ec1d19");
		/// <summary>
		/// The observation represents a cause of death
		/// </summary>
		public static readonly Guid CauseOfDeath = Guid.Parse("d5e0a5be-d227-413a-a752-b7d79d7d4ede");
		/// <summary>
		/// The observation represents the current clinical state about an entity (example: Clinical State is the patient is Dead)
		/// </summary>
		public static readonly Guid ClinicalState = Guid.Parse("6fb8487c-cd6f-44f1-ab63-27dc65405792");
		/// <summary>
		/// The observation represents a finding site where a problem or other observtion was made
		/// </summary>
		public static readonly Guid FindingSite = Guid.Parse("25D9F855-F0C8-4718-884D-04D3B6439E5C");
    }
}
