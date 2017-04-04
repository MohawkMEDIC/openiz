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
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents a local metadata repository service
	/// </summary>
	public class LocalMetadataRepositoryService : IMetadataRepositoryService
	{
		/// <summary>
		/// Find an assigning authority
		/// </summary>
		public IEnumerable<IdentifiedData> FindAssigningAuthority(Expression<Func<AssigningAuthority, bool>> query)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No concept persistence service found");

			int totalResults = 0;
			return persistenceService.Query(query, 0, 100, AuthenticationContext.Current.Principal, out totalResults);
		}

		/// <summary>
		/// Find assigning authority
		/// </summary>
		public IEnumerable<IdentifiedData> FindAssigningAuthority(Expression<Func<AssigningAuthority, bool>> query, int offset, int count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No concept persistence service found");

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Get the assigning authority
		/// </summary>
		public IdentifiedData GetAssigningAuthority(Guid id)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No concept persistence service found");

			return persistenceService.Get<Guid>(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(id), AuthenticationContext.Current.Principal, false);
		}
	}
}