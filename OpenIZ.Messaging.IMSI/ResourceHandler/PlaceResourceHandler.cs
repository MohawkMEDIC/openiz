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
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Services;
using MARC.HI.EHRS.SVC.Core;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Represents a resource handler for places.
	/// </summary>
	public class PlaceResourceHandler : IResourceHandler
	{
		// repository
		private IPlaceRepositoryService repository;

		public PlaceResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.repository = ApplicationContext.Current.GetService<IPlaceRepositoryService>();
		}

		public string ResourceName
		{
			get
			{
				return nameof(Place);
			}
		}

		public Type Type
		{
			get
			{
				return typeof(Place);
			}
		}

		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			throw new NotImplementedException();
		}

		public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.repository.Get(id, versionId);
		}

		public IdentifiedData Obsolete(Guid key)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
			return this.repository.Find(QueryExpressionParser.BuildLinqExpression<Place>(queryParameters));
		}

		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
			return this.repository.Find(QueryExpressionParser.BuildLinqExpression<Place>(queryParameters), offset, count, out totalCount);
		}

		public IdentifiedData Update(IdentifiedData data)
		{
			throw new NotImplementedException();
		}
	}
}
