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
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a repository service for managing places
	/// </summary>
	public interface IPlaceRepositoryService
	{
		/// <summary>
		/// Searches the patient service for the specified place matching the
		/// given predicate
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		IEnumerable<Place> Find(Expression<Func<Place, bool>> predicate);

		/// <summary>
		/// Searches the database for the specified place
		/// </summary>
		IEnumerable<Place> Find(Expression<Func<Place, bool>> predicate, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets the specified place
		/// </summary>
		Place Get(Guid id, Guid versionId);

		/// <summary>
		/// Inserts the specified place
		/// </summary>
		Place Insert(Place plc);

		/// <summary>
		/// Obsoletes the specified place
		/// </summary>
		Place Obsolete(Guid id);

		/// <summary>
		/// Saves the specified place
		/// </summary>
		Place Save(Place plc);
	}
}