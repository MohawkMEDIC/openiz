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
 * Date: 2016-8-28
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

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Represents an organization resource handler.
	/// </summary>
	public class OrganizationResourceHandler : IResourceHandler
	{
		private IOrganizationRepositoryService repository;

		public OrganizationResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.repository = ApplicationContext.Current.GetService<IOrganizationRepositoryService>();
		}

		/// <summary>
		/// Gets the resource name.
		/// </summary>
		public string ResourceName
		{
			get
			{
				return "Organization";
			}
		}

		/// <summary>
		/// Gets the resource type.
		/// </summary>
		public Type Type
		{
			get
			{
				return typeof(Organization);
			}
		}

        /// <summary>
        /// Creates an organization.
        /// </summary>
        /// <param name="data">The organization to be created.</param>
        /// <param name="updateIfExists">Update the organization if it exists.</param>
        /// <returns>Returns the newly create organization.</returns>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedMetadata)]
        public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException(string.Format("Bundle must have entry of type {0}", nameof(Organization)));
			}
			else if (processData is Organization)
			{
				var organizationData = data as Organization;

				if (updateIfExists)
				{
					return this.repository.Save(organizationData);
				}
				else
				{
					return this.repository.Insert(organizationData);
				}
			}
			else
			{
				throw new ArgumentException("Invalid persistence type");
			}
		}

        /// <summary>
        /// Gets an organization by id and version id.
        /// </summary>
        /// <param name="id">The id of the organization.</param>
        /// <param name="versionId">The version id of the organization.</param>
        /// <returns>Returns the organization.</returns>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.repository.Get(id, versionId);
		}

        /// <summary>
        /// Obsoletes an organization.
        /// </summary>
        /// <param name="key">The key of the organization to obsolete.</param>
        /// <returns>Returns the obsoleted organization.</returns>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedMetadata)]
        public IdentifiedData Obsolete(Guid key)
		{
			return this.repository.Obsolete(key);
		}

        /// <summary>
        /// Query for the specified org
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
        {
            int tr = 0;
            return this.Query(queryParameters, 0, 100, out tr);
        }

        /// <summary>
        /// Query for the specified org with restrictions
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            var filter = QueryExpressionParser.BuildLinqExpression<Organization>(queryParameters);
            List<String> queryId = null;
            if (this.repository is IPersistableQueryRepositoryService && queryParameters.TryGetValue("_queryId", out queryId))
                return (this.repository as IPersistableQueryRepositoryService).Find(filter, offset, count, out totalCount, Guid.Parse(queryId[0]));
            else
                return this.repository.Find(filter, offset, count, out totalCount);
        }

        /// <summary>
        /// Updates an organization.
        /// </summary>
        /// <param name="data">The organization to be updated.</param>
        /// <returns>Returns the updated organization.</returns>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedMetadata)]
        public IdentifiedData Update(IdentifiedData data)
		{
			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException(string.Format("Bundle must have entry of type {0}", nameof(Organization)));
			}
			else if (processData is Organization)
			{
				var organizationData = data as Organization;

				return this.repository.Save(organizationData);
			}
			else
			{
				throw new ArgumentException("Invalid persistence type");
			}
		}
	}
}