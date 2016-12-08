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
 * Date: 2016-8-24
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	public class ActResourceHandler : IResourceHandler
	{
		private IActRepositoryService actRepositorySerivce;

		public ActResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.actRepositorySerivce = ApplicationContext.Current.GetService<IActRepositoryService>();
		}

		public string ResourceName => "Act";

		public Type Type => typeof(Act);

		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var bundle = data as Bundle;

			bundle?.Reconstitute();

			var processData = bundle?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException("Bundle must have an entry point");
			}

			if (processData is Act)
			{
				var act = processData as Act;

				return updateIfExists ? this.actRepositorySerivce.Save(act) : this.actRepositorySerivce.Insert(act);
			}

			throw new ArgumentException(nameof(data), "Invalid data type");
		}

		public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.actRepositorySerivce.Get<Act>(id, versionId);
		}

		public IdentifiedData Obsolete(Guid key)
		{
			return this.actRepositorySerivce.Obsolete<Act>(key);
		}

		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
			int totalCount = 0;
			return this.Query(queryParameters, 0, 0, out totalCount);
		}

		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
			return this.actRepositorySerivce.Find<Act>(QueryExpressionParser.BuildLinqExpression<Act>(queryParameters), 0, count, out totalCount);
		}

		public IdentifiedData Update(IdentifiedData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var bundle = data as Bundle;

			bundle?.Reconstitute();

			var processData = bundle?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException("Bundle must have an entry");
			}

			if (processData is Act)
			{
				return this.actRepositorySerivce.Save(processData as Act);
			}

			throw new ArgumentException(nameof(data), "Invalid data type");
		}
	}
}