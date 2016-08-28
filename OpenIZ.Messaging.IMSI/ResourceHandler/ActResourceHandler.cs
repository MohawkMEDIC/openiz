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
 * Date: 2016-8-23
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

		public string ResourceName
		{
			get
			{
				return "Act";
			}
		}

		public Type Type
		{
			get
			{
				return typeof(Act);
			}
		}

		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			Bundle bundle = data as Bundle;

			bundle?.Reconstitute();

			var processData = bundle?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException("Bundle must have an entry point");
			}
			else if (processData is Act)
			{
				var act = processData as Act;

				if (updateIfExists)
				{
					return this.actRepositorySerivce.Save(act);
				}
				else
				{
					return this.actRepositorySerivce.Insert(act);
				}
			}
			else
			{
				throw new ArgumentException(nameof(data), "Invalid data type");
			}
		}

		public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.actRepositorySerivce.Get(id, versionId);
		}

		public IdentifiedData Obsolete(Guid key)
		{
			return this.actRepositorySerivce.Obsolete(key);
		}

		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
			int totalCount = 0;
			return this.Query(queryParameters, 0, 0, out totalCount);
		}

		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
			return this.actRepositorySerivce.FindActs(QueryExpressionParser.BuildLinqExpression<Act>(queryParameters), 0, count, out totalCount);
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
			else if (processData is Act)
			{
				return this.actRepositorySerivce.Save(processData as Act);
			}
			else
			{
				throw new ArgumentException(nameof(data), "Invalid data type");
			}
		}
	}
}