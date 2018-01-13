/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
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
	/// Represents an identifier type resource handler.
	/// </summary>
	public class IdentifierTypeResourceHandler : IResourceHandler
	{
		private IIdentifierTypeRepositoryService repository;

		/// <summary>
		/// Initializes a new instance of the <see cref="IdentifierTypeResourceHandler"/> class.
		/// </summary>
		public IdentifierTypeResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.repository = ApplicationContext.Current.GetService<IIdentifierTypeRepositoryService>();
		}

		/// <summary>
		/// Gets the resource name.
		/// </summary>
		public string ResourceName
		{
			get
			{
				return "IdentifierType";
			}
		}

		/// <summary>
		/// Gets the resource type.
		/// </summary>
		public Type Type
		{
			get
			{
				return typeof(IdentifierType);
			}
		}

        /// <summary>
        /// Creates an identifier type.
        /// </summary>
        /// <param name="data">The identifier type to be created.</param>
        /// <param name="updateIfExists">Update the identifier type if it exists.</param>
        /// <returns>Returns the newly created identifier type.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedMetadata)]
        public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException(string.Format("Bundle must have entry of type {0}", nameof(IdentifierType)));
			}
			else if (processData is IdentifierType)
			{
				var identifierTypeData = data as IdentifierType;

				if (updateIfExists)
				{
					return this.repository.Save(identifierTypeData);
				}
				else
				{
					return this.repository.Insert(identifierTypeData);
				}
			}
			else
			{
				throw new ArgumentException("Invalid persistence type");
			}
		}

        /// <summary>
        /// Gets an identifier type by id and version id.
        /// </summary>
        /// <param name="id">The id of the identifier type.</param>
        /// <param name="versionId">The version id of the identifier type.</param>
        /// <returns></returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.repository.Get(id, versionId);
		}

        /// <summary>
        /// Obsoletes an identifier type.
        /// </summary>
        /// <param name="key">The key of the identifier type to obsolete.</param>
        /// <returns>Returns the obsoleted identifier type.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedMetadata)]
        public IdentifiedData Obsolete(Guid key)
		{
			return this.repository.Obsolete(key);
		}

        /// <summary>
        /// Queries for an identifier type.
        /// </summary>
        /// <param name="queryParameters">The query parameters for which to use to query for the identifier type.</param>
        /// <returns>Returns a list of identifier types.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
            int tr = 00;
            return this.Query(queryParameters, 0, 100, out tr);
		}

        /// <summary>
        /// Queries for an identifier type.
        /// </summary>
        /// <param name="queryParameters">The query parameters for which to use to query for the identifier type.</param>
        /// <param name="count">The count of the query.</param>
        /// <param name="offset">The offset of the query.</param>
        /// <param name="totalCount">The total count of the query.</param>
        /// <returns>Returns a list of identifier types.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
            var filter = QueryExpressionParser.BuildLinqExpression<IdentifierType>(queryParameters);
            List<String> queryId = null;
            if (this.repository is IPersistableQueryRepositoryService && queryParameters.TryGetValue("_queryId", out queryId))
                return (this.repository as IPersistableQueryRepositoryService).Find(filter, offset, count, out totalCount, Guid.Parse(queryId[0]));
            else
                return this.repository.Find(filter, offset, count, out totalCount);
        }

        /// <summary>
        /// Updates an identifier type.
        /// </summary>
        /// <param name="data">The identifier type to be updated.</param>
        /// <returns>Returns the updated identifier type.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedMetadata)]
        public IdentifiedData Update(IdentifiedData data)
		{
			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException(string.Format("Bundle must have entry of type {0}", nameof(IdentifierType)));
			}
			else if (processData is IdentifierType)
			{
				var identifierData = data as IdentifierType;

				return this.repository.Save(identifierData);
			}
			else
			{
				throw new ArgumentException("Invalid persistence type");
			}
		}
	}
}