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
 * Date: 2016-8-15
 */
using OpenIZ.Core.Model;
using System.Collections.Generic;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents a business rules service with no behavior, but intended to help in the implementation of another
	/// business rules service
	/// </summary>
	public abstract class BaseBusinessRulesService<TModel> : IBusinessRulesService<TModel> where TModel : IdentifiedData
	{
		/// <summary>
		/// After insert
		/// </summary>
		public virtual TModel AfterInsert(TModel data)
		{
			return data;
		}

		/// <summary>
		/// After obsolete
		/// </summary>
		public virtual TModel AfterObsolete(TModel data)
		{
			return data;
		}

		/// <summary>
		/// After query
		/// </summary>
		/// <param name="results"></param>
		/// <returns></returns>
		public virtual IEnumerable<TModel> AfterQuery(IEnumerable<TModel> results)
		{
			return results;
		}

		/// <summary>
		/// Fired after retrieve
		/// </summary>
		public virtual TModel AfterRetrieve(TModel result)
		{
			return result;
		}

		/// <summary>
		/// After update
		/// </summary>
		public virtual TModel AfterUpdate(TModel data)
		{
			return data;
		}

		/// <summary>
		/// Before insert complete
		/// </summary>
		public virtual TModel BeforeInsert(TModel data)
		{
			return data;
		}

		/// <summary>
		/// Before obselete
		/// </summary>
		public virtual TModel BeforeObsolete(TModel data)
		{
			return data;
		}

		/// <summary>
		/// Before update
		/// </summary>
		public virtual TModel BeforeUpdate(TModel data)
		{
			return data;
		}

		/// <summary>
		/// Validate the specified object
		/// </summary>
		public virtual List<DetectedIssue> Validate(TModel data)
		{
			return new List<DetectedIssue>();
		}
	}
}