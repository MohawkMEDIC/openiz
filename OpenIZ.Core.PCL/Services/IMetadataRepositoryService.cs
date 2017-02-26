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
 * Date: 2016-8-15
 */

using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a repository which deals with metadata such as assigning authorities,
	/// concept classes, etc.
	/// </summary>
	public interface IMetadataRepositoryService
	{
		/// <summary>
		/// Finds the specified assigning authority
		/// </summary>
		/// <returns></returns>
		IEnumerable<IdentifiedData> FindAssigningAuthority(Expression<Func<AssigningAuthority, bool>> expression);

		/// <summary>
		/// Finds the specified assigning authority with restrictions
		/// </summary>
		IEnumerable<IdentifiedData> FindAssigningAuthority(Expression<Func<AssigningAuthority, bool>> expression, int offset, int count, out int totalCount);

		/// <summary>
		/// Gets an assigning authority
		/// </summary>
		IdentifiedData GetAssigningAuthority(Guid id);
	}
}