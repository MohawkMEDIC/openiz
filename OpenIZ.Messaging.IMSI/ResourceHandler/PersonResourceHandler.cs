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
 * User: khannan
 * Date: 2017-1-3
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Security;
using System.Security.Permissions;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Represents a resource handler for persons.
	/// </summary>
	public class PersonResourceHandler : IResourceHandler
	{
		/// <summary>
		/// The internal reference to the <see cref="IPersonRepositoryService"/> instance.
		/// </summary>
		private IPersonRepositoryService repository;

		/// <summary>
		/// Initializes a new instance of the <see cref="PersonResourceHandler"/> class.
		/// </summary>
		public PersonResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.repository = ApplicationContext.Current.GetService<IPersonRepositoryService>();
		}

		public string ResourceName => nameof(Person);

		public Type Type => typeof(Person);

		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			var bundleData = data as Bundle;

			bundleData?.Reconstitute();

			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException($"Bundle must have entry of type {nameof(Person)}");
			}

			if (processData is Person)
			{
				return updateIfExists ? this.repository.Save(processData as Person) : this.repository.Insert(processData as Person);
			}

			throw new ArgumentException("Invalid persistence type");
		}

		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.QueryClinicalData)]
        public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.repository.Get(id, versionId);
		}

		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.DeleteClinicalData)]
        public IdentifiedData Obsolete(Guid key)
		{
			return this.repository.Obsolete(key);
		}

		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.QueryClinicalData)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
            int tr = 0;
            return this.Query(queryParameters, 0, 100, out tr);
        }

		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.QueryClinicalData)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
            var filter = QueryExpressionParser.BuildLinqExpression<Person>(queryParameters);
            List<String> queryId = null;
            if (this.repository is IPersistableQueryRepositoryService && queryParameters.TryGetValue("_queryId", out queryId))
                return (this.repository as IPersistableQueryRepositoryService).Find(filter, offset, count, out totalCount, Guid.Parse(queryId[0]));
            else
                return this.repository.Find(filter, offset, count, out totalCount);
        }

		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public IdentifiedData Update(IdentifiedData data)
		{
			var bundleData = data as Bundle;

			bundleData?.Reconstitute();

			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException($"Bundle must have entry of type {nameof(Person)}");
			}

			if (processData is Provider)
			{
				return this.repository.Save(processData as Person);
			}

			throw new ArgumentException("Invalid persistence type");
		}
	}
}