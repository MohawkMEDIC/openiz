/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-9
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Entities;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Data;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents a place repository service.
	/// </summary>
	public class LocalPlaceRepositoryService : IPlaceRepositoryService
	{
		/// <summary>
		/// Searches the place service for the specified place matching the 
		/// given predicate
		/// </summary>
		/// <param name="predicate">The predicate function to search by.</param>
		/// <returns>Returns a list of places.</returns>
		public IEnumerable<Place> Find(Expression<Func<Place, bool>> predicate)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Query(predicate, AuthenticationContext.Current.Principal);
		}

		/// <summary>
		/// Searches the place service for the specified place matching the 
		/// given predicate
		/// </summary>
		/// <param name="predicate">The predicate function to search by.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of places.</returns>
		public IEnumerable<Place> Find(Expression<Func<Place, bool>> predicate, int offset, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Query(predicate, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Gets the specified place by key and optional version key.
		/// </summary>
		/// <param name="key">The key of the place.</param>
		/// <param name="versionKey">The version key of the place.</param>
		/// <returns>Returns a place.</returns>
		public IdentifiedData Get(Guid key, Guid versionKey)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(key, versionKey), AuthenticationContext.Current.Principal, false);
		}
	}
}
