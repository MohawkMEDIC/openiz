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
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Represents a resource handler that can perform operations on materials
	/// </summary>
	public class MaterialResourceHandler : IResourceHandler
	{
		// Repository
		private IMaterialRepositoryService repository;

		/// <summary>
		/// Place resource handler subscription
		/// </summary>
		public MaterialResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.repository = ApplicationContext.Current.GetService<IMaterialRepositoryService>();
		}

		/// <summary>
		/// Gets the name of the resource that this handler handles
		/// </summary>
		public string ResourceName => "Material";

		/// <summary>
		/// Gets the type of resource that this handler handles
		/// </summary>
		public Type Type => typeof(Material);

		/// <summary>
		/// Creates the specified place
		/// </summary>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			Bundle bundleData = data as Bundle;

			bundleData?.Reconstitute();

			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				// Client submitted a bundle
				throw new InvalidOperationException("Bundle must have an entry point");
			}

			if (processData is Material)
			{
				return updateIfExists ? this.repository.Save(processData as Material) : this.repository.Insert(processData as Material);
			}

			throw new ArgumentException(nameof(data), "Invalid data type");
		}

		/// <summary>
		/// Gets the specified data
		/// </summary>
		public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.repository.GetMaterial(id, versionId);
		}

		/// <summary>
		/// Obsoletes the specified data
		/// </summary>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
		public IdentifiedData Obsolete(Guid key)
		{
			return this.repository.ObsoleteMaterial(key);
		}

        /// <summary>
        /// Query for the specified material
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
        {
            int tr = 0;
            return this.Query(queryParameters, 0, 100, out tr);
        }

        /// <summary>
        /// Query for the specified material with restrictions
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            var filter = QueryExpressionParser.BuildLinqExpression<Material>(queryParameters);
            List<String> queryId = null;
            if (this.repository is IPersistableQueryRepositoryService && queryParameters.TryGetValue("_queryId", out queryId))
                return (this.repository as IPersistableQueryRepositoryService).Find(filter, offset, count, out totalCount, Guid.Parse(queryId[0]));
            else
                return this.repository.FindMaterial(filter, offset, count, out totalCount);
        }

        /// <summary>
        /// Updates the specified object
        /// </summary>
        public IdentifiedData Update(IdentifiedData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var bundleData = data as Bundle;

			bundleData?.Reconstitute();

			var saveData = bundleData?.Entry ?? data;

			if (saveData is Bundle)
			{
				throw new InvalidOperationException("Bundle must have an entry");
			}

			if (saveData is Material)
			{
				return this.repository.Save(saveData as Material);
			}

			throw new ArgumentException(nameof(data), "Invalid storage type");
		}
	}
}