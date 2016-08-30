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
 * User: Nityan
 * Date: 2016-8-30
 */

using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents a repository service for managing assigning authorities.
	/// </summary>
	public class LocalAssigningAuthorityRepositoryService : IAssigningAuthorityRepositoryService
	{
		/// <summary>
		/// Finds a list of assigning authorities.
		/// </summary>
		/// <param name="query">The query to use to find the assigning authorities.</param>
		/// <returns>Returns a list of assigning authorities.</returns>
		public IEnumerable<AssigningAuthority> Find(Expression<Func<AssigningAuthority, bool>> query)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Finds a list of assigning authorities.
		/// </summary>
		/// <param name="query">The query to use to find the assigning authorities.</param>
		/// <param name="offSet">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of assigning authorities.</returns>
		public IEnumerable<AssigningAuthority> Find(Expression<Func<AssigningAuthority, bool>> query, int offSet, int? count, out int totalCount)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets an assigning authority.
		/// </summary>
		/// <param name="key">The key of the assigning authority to be retrieved.</param>
		/// <returns>Returns an assiging authority.</returns>
		public AssigningAuthority Get(Guid key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Inserts an assigning authority.
		/// </summary>
		/// <param name="assigningAuthority">The assigning authority to be inserted.</param>
		/// <returns>Returns the inserted assigning authority.</returns>
		public AssigningAuthority Insert(AssigningAuthority assigningAuthority)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Obsoletes ans assigning authority.
		/// </summary>
		/// <param name="key">The key of the assigning authority to be obsoleted.</param>
		/// <returns>Returns the obsoleted assigning authority.</returns>
		public AssigningAuthority Obsolete(Guid key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Saves or inserts an assigning authority.
		/// </summary>
		/// <param name="assigningAuthority">The assigning authority to be saved.</param>
		/// <returns>Returns the saved assigning authority.</returns>
		public AssigningAuthority Save(AssigningAuthority assigningAuthority)
		{
			throw new NotImplementedException();
		}
	}
}