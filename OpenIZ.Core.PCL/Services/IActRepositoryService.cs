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
 * User: khannan
 * Date: 2016-8-22
 */

using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents the act repository service
	/// </summary>
	public interface IActRepositoryService
	{
		/// <summary>
		/// Find all acts
		/// </summary>
		IEnumerable<Act> FindActs(Expression<Func<Act, bool>> query, int offset, int? count, out int totalResults);

		/// <summary>
		/// Find the substance administrations
		/// </summary>
		IEnumerable<SubstanceAdministration> FindSubstanceAdministrations(Expression<Func<SubstanceAdministration, bool>> filter, int offset, int? count, out int totalResults);

		/// <summary>
		/// Get the specified act
		/// </summary>
		Act Get(Guid key, Guid versionId);

		/// <summary>
		/// Insert the specified act
		/// </summary>
		Act Insert(Act insert);

		/// <summary>
		/// Obsolete the specified act
		/// </summary>
		Act Obsolete(Guid key);

		/// <summary>
		/// Insert or update the specified act
		/// </summary>
		Act Save(Act act);

		/// <summary>
		/// Validate the act
		/// </summary>
		Act Validate(Act act);
	}
}