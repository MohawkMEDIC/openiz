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
	/// Represents a user entity resource handler.
	/// </summary>
	public class UserEntityResourceHandler : IResourceHandler
	{
		// Repository
		private ISecurityRepositoryService repository;

		/// <summary>
		/// Place resource handler subscription
		/// </summary>
		public UserEntityResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.repository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
		}

		/// <summary>
		/// Gets the name of the resource that this handler
		/// </summary>
		public string ResourceName => "UserEntity";

		/// <summary>
		/// Gets the .NET type of the resource handler
		/// </summary>
		public Type Type => typeof(UserEntity);

		/// <summary>
		/// Creates the specified user entity
		/// </summary>
		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle) // Client submitted a bundle
				throw new InvalidOperationException("Bundle must have an entry point");
			else if (processData is UserEntity)
			{
				var userEntity = processData as UserEntity;
				if (updateIfExists)
					return this.repository.CreateUserEntity(userEntity);
				else
					return this.repository.CreateUserEntity(userEntity);
			}
			else
				throw new ArgumentException(nameof(data), "Invalid data type");
		}

		/// <summary>
		/// Gets the specified user entity
		/// </summary>
		public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.repository.GetUserEntity(id, versionId);
		}

		/// <summary>
		/// Obsolete the entity
		/// </summary>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
		public IdentifiedData Obsolete(Guid key)
		{
			return this.repository.ObsoleteUserEntity(key);
		}

		/// <summary>
		/// Queries the specified user entity
		/// </summary>
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
            int tr = 0;
            return this.Query(queryParameters, 0, 100, out tr);
        }

		/// <summary>
		/// Query the specified user entity with restrictions
		/// </summary>
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
            var filter = QueryExpressionParser.BuildLinqExpression<UserEntity>(queryParameters);
            List<String> queryId = null;
            if (this.repository is IPersistableQueryRepositoryService && queryParameters.TryGetValue("_queryId", out queryId))
                return (this.repository as IPersistableQueryRepositoryService).Find(filter, offset, count, out totalCount, Guid.Parse(queryId[0]));
            else
                return this.repository.FindUserEntity(filter, offset, count, out totalCount);
        }

		/// <summary>
		/// Updates the specified user entity
		/// </summary>
		public IdentifiedData Update(IdentifiedData data)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			var bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var saveData = bundleData?.Entry ?? data;

			if (saveData is Bundle)
				throw new InvalidOperationException("Bundle must have an entry");
			else if (saveData is UserEntity)
				return this.repository.SaveUserEntity(saveData as UserEntity);
			else
				throw new ArgumentException(nameof(data), "Invalid storage type");
		}
	}
}