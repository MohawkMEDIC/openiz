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
 * Date: 2017-1-21
 */
using System;
using System.Collections;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Non-generic form of the data persistene service
	/// </summary>
	public interface IDataPersistenceService
	{
		/// <summary>
		/// Inserts the specified object
		/// </summary>
		Object Insert(Object data);

		/// <summary>
		/// Updates the specified data
		/// </summary>
		Object Update(Object data);

		/// <summary>
		/// Obsoletes the specified data
		/// </summary>
		Object Obsolete(Object data);

		/// <summary>
		/// Gets the specified data
		/// </summary>
		Object Get(Guid id);

		/// <summary>
		/// Query based on the expression given
		/// </summary>
		IEnumerable Query(Expression query, int offset, int? count, out int totalResults);
	}
}