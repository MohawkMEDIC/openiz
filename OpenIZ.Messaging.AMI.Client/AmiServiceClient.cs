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
 * Date: 2016-7-22
 */
using OpenIZ.Core.Interop.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenIZ.Core.Http;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.AMI.Auth;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Messaging.AMI.Client
{
	/// <summary>
	/// Represents the AMI service client.
	/// </summary>
	public class AmiServiceClient : ServiceClientBase, IDisposable
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Messaging.AMI.Client.AmiServiceClient"/> class
		/// with a specified <see cref="OpenIZ.Core.Http.IRestClient"/> instance.
		/// </summary>
		/// <param name="client">The <see cref="OpenIZ.Core.Http.IRestClient"/> instance.</param>
		public AmiServiceClient(IRestClient client) : base(client)
        {
        }

		public SubmissionResult AcceptCertificateSigningRequest(string id)
		{
			return this.Client.Put<object, SubmissionResult>(string.Format("csr/{0}", id), this.Client.Accept, null);
		}

		/// <summary>
		/// Creates a role in the IMS.
		/// </summary>
		/// <param name="role">The role to be created.</param>
		/// <returns>Returns the newly created role.</returns>
		public SecurityRoleInfo CreateRole(SecurityRoleInfo role)
		{
			return this.Client.Post<SecurityRoleInfo, SecurityRoleInfo>("role", this.Client.Accept, role);
		}

		/// <summary>
		/// Creates a user in the IMS.
		/// </summary>
		/// <param name="user">The user to be created.</param>
		/// <returns>Returns the newly created user.</returns>
		public SecurityUserInfo CreateUser(SecurityUserInfo user)
		{
			return this.Client.Post<SecurityUserInfo, SecurityUserInfo>("user", this.Client.Accept, user);
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.Client?.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ImsiServiceClient() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		/// <summary>
		/// Dispose of any resources.
		/// </summary>
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support

		/// <summary>
		/// Retrieves a specified role from the AMI.
		/// </summary>
		/// <param name="query">The query expression to use to find the role.</param>
		/// <returns>Returns a collection of roles which match the specified query parameters.</returns>
		public AmiCollection<SecurityRoleInfo> FindRole(Expression<Func<SecurityRole, bool>> query)
        {
            var queryParms = QueryExpressionBuilder.BuildQuery(query).ToArray();
            return this.Client.Get<AmiCollection<SecurityRoleInfo>>("role", queryParms);
        }

		/// <summary>
		/// Retrieves a specified policy from the AMI.
		/// </summary>
		/// <param name="query">The query expression to use to find the policy.</param>
		/// <returns>Returns a collection of policies which match the specified query parameters.</returns>
		public AmiCollection<SecurityPolicyInfo> FindPolicy(Expression<Func<SecurityPolicy, bool>> query)
        {
            var queryParms = QueryExpressionBuilder.BuildQuery(query).ToArray();
            return this.Client.Get<AmiCollection<SecurityPolicyInfo>>("policy", queryParms);
        }

		/// <summary>
		/// Retrieves a specified user from the AMI.
		/// </summary>
		/// <param name="query">The query expression to use to find the user.</param>
		/// <returns>Returns a collection of users which match the specified query parameters.</returns>
		public AmiCollection<SecurityUserInfo> FindUser(Expression<Func<SecurityUserInfo, bool>> query)
		{
			var queryParms = QueryExpressionBuilder.BuildQuery(query).ToArray();
			return this.Client.Get<AmiCollection<SecurityUserInfo>>("user", queryParms);
		}
    }
}
