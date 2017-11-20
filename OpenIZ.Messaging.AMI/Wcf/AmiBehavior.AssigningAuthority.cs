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
 * Date: 2016-11-30
 */

using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using System;
using System.Linq;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.AMI.Wcf
{
	/// <summary>
	/// Represents the administrative contract interface.
	/// </summary>
	public partial class AmiBehavior
	{
		/// <summary>
		/// Creates an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityInfo">The assigning authority to be created.</param>
		/// <returns>Returns the created assigning authority.</returns>
		/// <exception cref="System.InvalidOperationException">If the IAssigningAuthorityRepositoryService is not found.</exception>
		public AssigningAuthorityInfo CreateAssigningAuthority(AssigningAuthorityInfo assigningAuthorityInfo)
		{
			var assigningAuthorityRepositoryService = ApplicationContext.Current.GetService<IAssigningAuthorityRepositoryService>();

			if (assigningAuthorityRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(IAssigningAuthorityRepositoryService)} not found");
			}

			var createdAssigningAuthority = assigningAuthorityRepositoryService.Insert(assigningAuthorityInfo.AssigningAuthority);

			return new AssigningAuthorityInfo(createdAssigningAuthority);
		}

		/// <summary>
		/// Deletes an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to be deleted.</param>
		/// <returns>Returns the deleted assigning authority.</returns>
		/// <exception cref="System.ArgumentException">Assigning authority not found.</exception>
		/// <exception cref="System.InvalidOperationException">If the IAssigningAuthorityRepositoryService is not found.</exception>
		public AssigningAuthorityInfo DeleteAssigningAuthority(string assigningAuthorityId)
		{
			Guid id;

			if (!Guid.TryParse(assigningAuthorityId, out id))
			{
				throw new ArgumentException($"{nameof(assigningAuthorityId)} must be a valid GUID");
			}

			var assigningAuthorityService = ApplicationContext.Current.GetService<IAssigningAuthorityRepositoryService>();

			if (assigningAuthorityService == null)
			{
				throw new InvalidOperationException($"{nameof(IAssigningAuthorityRepositoryService)} not found");
			}

			return new AssigningAuthorityInfo
			{
				AssigningAuthority = assigningAuthorityService.Obsolete(id),
				Id = id
			};
		}

		/// <summary>
		/// Gets a list of assigning authorities for a specific query.
		/// </summary>
		/// <returns>Returns a list of assigning authorities which match the specific query.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.InvalidOperationException">If the IAssigningAuthorityRepositoryService is not found.</exception>
		public AmiCollection<AssigningAuthorityInfo> GetAssigningAuthorities()
		{
			var parameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

			if (parameters.Count == 0)
			{
				throw new ArgumentException($"{nameof(parameters)} cannot be empty");
			}

			var expression = QueryExpressionParser.BuildLinqExpression<AssigningAuthority>(this.CreateQuery(parameters));

			var assigningAuthorityRepositoryService = ApplicationContext.Current.GetService<IAssigningAuthorityRepositoryService>();

			if (assigningAuthorityRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(IAssigningAuthorityRepositoryService)} not found");
			}

			var assigningAuthorities = new AmiCollection<AssigningAuthorityInfo>();

			var totalCount = 0;

			assigningAuthorities.CollectionItem = assigningAuthorityRepositoryService.Find(expression, 0, null, out totalCount).Select(a => new AssigningAuthorityInfo(a)).ToList();
			assigningAuthorities.Size = totalCount;

			return assigningAuthorities;
		}

		/// <summary>
		/// Gets a specific assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to retrieve.</param>
		/// <returns>Returns the assigning authority.</returns>
		/// <exception cref="System.ArgumentException">If the assigning authority is not found.</exception>
		/// <exception cref="System.InvalidOperationException">If the IAssigningAuthorityRepositoryService is not found.</exception>
		public AssigningAuthorityInfo GetAssigningAuthority(string assigningAuthorityId)
		{
			Guid id;

			if (!Guid.TryParse(assigningAuthorityId, out id))
			{
				throw new ArgumentException($"{nameof(assigningAuthorityId)} must be a valid GUID");
			}

			var assigningAuthorityRepositoryService = ApplicationContext.Current.GetService<IAssigningAuthorityRepositoryService>();

			if (assigningAuthorityRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(IAssigningAuthorityRepositoryService)} not found");
			}

			return new AssigningAuthorityInfo(assigningAuthorityRepositoryService.Get(id));
		}

		/// <summary>
		/// Updates an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to be updated.</param>
		/// <param name="assigningAuthorityInfo">The assigning authority containing the updated information.</param>
		/// <returns>Returns the updated assigning authority.</returns>
		/// <exception cref="System.ArgumentException">If the assigning authority is not found.</exception>
		/// <exception cref="System.InvalidOperationException">If the IAssigningAuthorityRepositoryService is not found.</exception>
		public AssigningAuthorityInfo UpdateAssigningAuthority(string assigningAuthorityId, AssigningAuthorityInfo assigningAuthorityInfo)
		{
			Guid id;

			if (!Guid.TryParse(assigningAuthorityId, out id))
			{
				throw new ArgumentException($"{nameof(assigningAuthorityId)} must be a valid GUID");
			}

			if (id != assigningAuthorityInfo.Id)
			{
				throw new ArgumentException($"Unable to update assigning authority using id: {id}, and id: {assigningAuthorityInfo.Id}");
			}

			var assigningAuthorityRepositoryService = ApplicationContext.Current.GetService<IAssigningAuthorityRepositoryService>();

			if (assigningAuthorityRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(IAssigningAuthorityRepositoryService)} not found");
			}

			var result = assigningAuthorityRepositoryService.Save(assigningAuthorityInfo.AssigningAuthority);

			return new AssigningAuthorityInfo(result);
		}
	}
}