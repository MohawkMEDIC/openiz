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
		/// Creates a security policy.
		/// </summary>
		/// <param name="policy">The security policy to be created.</param>
		/// <returns>Returns the newly created security policy.</returns>
		public SecurityPolicyInfo CreatePolicy(SecurityPolicyInfo policy)
		{
			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(ISecurityRepositoryService)));
			}

			SecurityPolicy policyToCreate = new SecurityPolicy
			{
				CanOverride = policy.CanOverride,
				Name = policy.Name,
				Oid = policy.Oid
			};

			return new SecurityPolicyInfo(securityRepository.CreatePolicy(policyToCreate));
		}

		/// <summary>
		/// Deletes a security policy.
		/// </summary>
		/// <param name="policyId">The id of the policy to be deleted.</param>
		/// <returns>Returns the deleted policy.</returns>
		public SecurityPolicyInfo DeletePolicy(string policyId)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(policyId, out key))
			{
				throw new ArgumentException($"{nameof(policyId)} must be a valid GUID");
			}

			var securityRepositoryService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			return new SecurityPolicyInfo(securityRepositoryService.ObsoletePolicy(key));
		}

		/// <summary>
		/// Gets a list of policies.
		/// </summary>
		/// <returns>Returns a list of policies.</returns>
		public AmiCollection<SecurityPolicyInfo> GetPolicies()
		{
			var expression = QueryExpressionParser.BuildLinqExpression<SecurityPolicy>(this.CreateQuery(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters));
			var userRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			return new AmiCollection<SecurityPolicyInfo>() { CollectionItem = userRepository.FindPolicies(expression).Select(o => new SecurityPolicyInfo(o)).ToList() };
		}

		/// <summary>
		/// Gets a specific security policy.
		/// </summary>
		/// <param name="policyId">The id of the security policy to be retrieved.</param>
		/// <returns>Returns the security policy.</returns>
		public SecurityPolicyInfo GetPolicy(string policyId)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(policyId, out key))
			{
				throw new ArgumentException($"{nameof(policyId)} must be a valid GUID");
			}

			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepository == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			return new SecurityPolicyInfo(securityRepository.GetPolicy(Guid.Parse(policyId)));
		}

		/// <summary>
		/// Updates a policy.
		/// </summary>
		/// <param name="policyId">The id of the policy to be updated.</param>
		/// <param name="policyInfo">The policy containing the updated information.</param>
		/// <returns>Returns the updated policy.</returns>
		public SecurityPolicyInfo UpdatePolicy(string policyId, SecurityPolicyInfo policyInfo)
		{
			var id = Guid.Empty;

			if (!Guid.TryParse(policyId, out id))
			{
				throw new ArgumentException($"{nameof(policyId)} must be a valid GUID");
			}

			if (id != policyInfo.Policy.Key)
			{
				throw new ArgumentException($"Unable to update role using id: {id}, and id: {policyInfo.Policy.Key}");
			}

			var policyRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (policyRepository == null)
			{
				throw new InvalidOperationException($"{nameof(ISecurityRepositoryService)} not found");
			}

			var policy = policyRepository.SavePolicy(policyInfo.Policy);

			return new SecurityPolicyInfo(policy);
		}
	}
}