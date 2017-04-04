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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Resource handler which can deal with metadata resources
	/// </summary>
	public class AssigningAuthorityResourceHandler : IResourceHandler
	{
		// repository
		private IMetadataRepositoryService m_repository;

		public AssigningAuthorityResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.m_repository = ApplicationContext.Current.GetService<IMetadataRepositoryService>();
		}

		/// <summary>
		/// The name of the resource
		/// </summary>
		public string ResourceName
		{
			get
			{
				return "AssigningAuthority";
			}
		}

		/// <summary>
		/// Gets the type this resource handler exposes
		/// </summary>
		public Type Type
		{
			get
			{
				return typeof(AssigningAuthority);
			}
		}

		/// <summary>
		/// Create an assigning authority - not supported
		/// </summary>
		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Get the assigning authority
		/// </summary>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
		public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.m_repository.GetAssigningAuthority(id);
		}

		/// <summary>
		/// Obsoletes an assigning authority
		/// </summary>
		public IdentifiedData Obsolete(Guid key)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Queries for assigning authority
		/// </summary>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
			return this.m_repository.FindAssigningAuthority(QueryExpressionParser.BuildLinqExpression<AssigningAuthority>(queryParameters));
		}

		/// <summary>
		/// Query for the specified AA
		/// </summary>
		[PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
			return this.m_repository.FindAssigningAuthority(QueryExpressionParser.BuildLinqExpression<AssigningAuthority>(queryParameters), offset, count, out totalCount);
		}

		/// <summary>
		/// Update assigning authority
		/// </summary>
		public IdentifiedData Update(IdentifiedData data)
		{
			throw new NotSupportedException();
		}
	}
}