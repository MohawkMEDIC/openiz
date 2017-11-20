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
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents the patient repository service. This service is responsible
	/// for ensuring that patient roles in the IMS database are in a consistent
	/// state.
	/// </summary>
	public interface IPatientRepositoryService
	{
		/// <summary>
		/// Searches the patient service for the specified patient matching the
		/// given predicate
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate);

		/// <summary>
		/// Searches the database for the specified patient
		/// </summary>
		IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets the specified patient
		/// </summary>
		Patient Get(Guid id, Guid versionId);

		/// <summary>
		/// Inserts the given patient
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		Patient Insert(Patient p);

		/// <summary>
		/// Merges two patients together
		/// </summary>
		/// <param name="survivor">The surviving patient record</param>
		/// <param name="victim">The victim patient record</param>
		/// <returns>A new version of patient <paramref name="a"/> representing the merge</returns>
		Patient Merge(Patient survivor, Patient victim);

		/// <summary>
		/// Obsoletes the given patient
		/// </summary>
		Patient Obsolete(Guid key);

		/// <summary>
		/// Updates the given patient only if they already exist
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		Patient Save(Patient p);

		/// <summary>
		/// Un-merges two patients from each other
		/// </summary>
		/// <param name="patient">The patient which is to be un-merged</param>
		/// <param name="versionKey">The version of patient P where the split should occur</param>
		/// <returns>A new patient representing the split record</returns>
		Patient UnMerge(Patient patient, Guid versionKey);

		/// <summary>
		/// Validate the specified patient, or rather ensure the patient is valid
		/// </summary>
		Patient Validate(Patient p);
	}
}