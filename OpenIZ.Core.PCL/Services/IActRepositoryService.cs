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
    /// TODO: Should the entire interface be genericised? 
	public interface IActRepositoryService
	{
		/// <summary>
		/// Find all acts
		/// </summary>
		IEnumerable<TAct> Find<TAct>(Expression<Func<TAct, bool>> query, int offset, int? count, out int totalResults) where TAct : Act;

        /// <summary>
        /// Get the specified act
        /// </summary>
        TAct Get<TAct>(Guid key, Guid versionId) where TAct : Act;

        /// <summary>
        /// Insert the specified act
        /// </summary>
        TAct Insert<TAct>(TAct insert) where TAct : Act;

		/// <summary>
		/// Obsolete the specified act
		/// </summary>
		TAct Obsolete<TAct>(Guid key) where TAct : Act;

		/// <summary>
		/// Insert or update the specified act
		/// </summary>
		TAct Save<TAct>(TAct act) where TAct : Act;

		/// <summary>
		/// Validate the act
		/// </summary>
		TAct Validate<TAct>(TAct act) where TAct : Act;
	}
}