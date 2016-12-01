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
 * Date: 2016-8-2
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	public class ProviderResourceHandler : IResourceHandler
	{
		private IProviderRepositoryService repository;

		public ProviderResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.repository = ApplicationContext.Current.GetService<IProviderRepositoryService>();
		}

		public string ResourceName => nameof(Provider);

		public Type Type => typeof(Provider);

		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			var bundleData = data as Bundle;

			bundleData?.Reconstitute();

			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException($"Bundle must have entry of type {nameof(Provider)}");
			}

			if (processData is Provider)
			{
				return updateIfExists ? this.repository.Save(processData as Provider) : this.repository.Insert(processData as Provider);
			}

			throw new ArgumentException("Invalid persistence type");
		}

		public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.repository.Get(id, versionId);
		}

		public IdentifiedData Obsolete(Guid key)
		{
			return this.repository.Obsolete(key);
		}

		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
			return this.repository.Find(QueryExpressionParser.BuildLinqExpression<Provider>(queryParameters));
		}

		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
			return this.repository.Find(QueryExpressionParser.BuildLinqExpression<Provider>(queryParameters), offset, count, out totalCount);
		}

		public IdentifiedData Update(IdentifiedData data)
		{
			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException(string.Format("Bundle must have entry of type {0}", nameof(Provider)));
			}
			else if (processData is Provider)
			{
				var providerData = data as Provider;

				return this.repository.Save(providerData);
			}
			else
			{
				throw new ArgumentException("Invalid persistence type");
			}
		}
	}
}