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
 * Date: 2016-7-18
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Data;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.Security.Claims;
using OpenIZ.Messaging.IMSI.Util;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Security;
using System.Security.Permissions;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Model.Roles;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	public class ProviderResourceHandler : IResourceHandler
	{
		public string ResourceName
		{
			get
			{
				return nameof(Provider);
			}
		}

		public Type Type
		{
			get
			{
				return typeof(Provider);
			}
		}

		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			var providerService = ApplicationContext.Current.GetService<IProviderRepositoryService>();

			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException(string.Format("Bundle must have entry of type {0}", nameof(Provider)));
			}
			else if (processData is Concept)
			{
				var providerData = data as Provider;

				if (updateIfExists)
				{
					return providerService.Save(providerData);
				}
				else
				{
					return providerService.Insert(providerData);
				}
			}
			else
			{
				throw new ArgumentException("Invalid persistence type");
			}
		}

		public IdentifiedData Get(Guid id, Guid versionId)
		{
			var providerService = ApplicationContext.Current.GetService<IProviderRepositoryService>();
			return providerService.Get(id, versionId);
		}

		public IdentifiedData Obsolete(Guid key)
		{
			var providerService = ApplicationContext.Current.GetService<IProviderRepositoryService>();
			return providerService.Obsolete(key);
		}

		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
			var providerService = ApplicationContext.Current.GetService<IProviderRepositoryService>();
			return providerService.Find(QueryExpressionParser.BuildLinqExpression<Provider>(queryParameters));
		}

		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
			var providerService = ApplicationContext.Current.GetService<IProviderRepositoryService>();
			return providerService.Find(QueryExpressionParser.BuildLinqExpression<Provider>(queryParameters), offset, count, out totalCount);
		}

		public IdentifiedData Update(IdentifiedData data)
		{
			var providerService = ApplicationContext.Current.GetService<IProviderRepositoryService>();

			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException(string.Format("Bundle must have entry of type {0}", nameof(Provider)));
			}
			else if (processData is Concept)
			{
				var providerData = data as Provider;
				return providerService.Save(providerData);
			}
			else
			{
				throw new ArgumentException("Invalid persistence type");
			}
		}
	}
}
