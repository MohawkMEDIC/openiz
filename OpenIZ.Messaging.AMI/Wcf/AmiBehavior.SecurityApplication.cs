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
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Services;
using System;
using System.Data;
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
		/// Creates a security application.
		/// </summary>
		/// <param name="applicationInfo">The security application to be created.</param>
		/// <returns>Returns the created security application.</returns>
		public SecurityApplicationInfo CreateApplication(SecurityApplicationInfo applicationInfo)
		{
			var securityRepositoryService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			applicationInfo.Application?.Policies.AddRange(applicationInfo.Policies.Select(p => new SecurityPolicyInstance(p.Policy, p.Grant)));

			var createdApplication = securityRepositoryService.CreateApplication(applicationInfo.Application);

			return new SecurityApplicationInfo(createdApplication);
		}

		/// <summary>
		/// Deletes an application.
		/// </summary>
		/// <param name="applicationId">The id of the application to be deleted.</param>
		/// <returns>Returns the deleted application.</returns>
		public SecurityApplicationInfo DeleteApplication(string applicationId)
		{
			Guid applicationKey = Guid.Empty;

			if (!Guid.TryParse(applicationId, out applicationKey))
			{
				throw new ArgumentException($"{nameof(applicationId)} must be a valid GUID");
			}

			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepository == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			var obsoletedApplication = securityRepository.ObsoleteApplication(applicationKey);

			return new SecurityApplicationInfo(obsoletedApplication);
		}

		/// <summary>
		/// Gets a specific application.
		/// </summary>
		/// <param name="applicationId">The id of the application to retrieve.</param>
		/// <returns>Returns the application.</returns>
		public SecurityApplicationInfo GetApplication(string applicationId)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(applicationId, out key))
			{
				throw new ArgumentException($"{nameof(applicationId)} must be a valid GUID");
			}

			var securityRepositoryService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			var application = securityRepositoryService.GetApplication(key);

			return new SecurityApplicationInfo(application);
		}

		/// <summary>
		/// Gets a list applications for a specific query.
		/// </summary>
		/// <returns>Returns a list of application which match the specific query.</returns>
		public AmiCollection<SecurityApplicationInfo> GetApplications()
		{
			var parameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

			if (parameters.Count == 0)
			{
				throw new ArgumentException($"{nameof(parameters)} cannot be empty");
			}

			var expression = QueryExpressionParser.BuildLinqExpression<SecurityApplication>(this.CreateQuery(parameters));

			var securityRepositoryService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			var applications = new AmiCollection<SecurityApplicationInfo>();

			int totalCount = 0;

			applications.CollectionItem = securityRepositoryService.FindApplications(expression, 0, null, out totalCount).Select(a => new SecurityApplicationInfo(a)).ToList();
			applications.Size = totalCount;

			return applications;
		}

		/// <summary>
		/// Updates an application.
		/// </summary>
		/// <param name="applicationId">The id of the application to be updated.</param>
		/// <param name="applicationInfo">The application containing the updated information.</param>
		/// <returns>Returns the updated application.</returns>
		public SecurityApplicationInfo UpdateApplication(string applicationId, SecurityApplicationInfo applicationInfo)
		{
			var id = Guid.Empty;

			if (!Guid.TryParse(applicationId, out id))
			{
				throw new ArgumentException($"{nameof(applicationId)} must be a valid GUID");
			}

			if (id != applicationInfo.Id)
			{
				throw new ArgumentException($"Cannot update application using id: {id} and {applicationInfo.Id}");
			}

			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepository == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			applicationInfo.Application.Policies.AddRange(applicationInfo.Policies.Select(p => new SecurityPolicyInstance(p.Policy, p.Grant)));

			var updatedApplication = securityRepository.SaveApplication(applicationInfo.Application);

			return new SecurityApplicationInfo(updatedApplication);
		}
	}
}