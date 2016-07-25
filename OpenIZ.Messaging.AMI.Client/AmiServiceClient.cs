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

using OpenIZ.Core.Http;
using OpenIZ.Core.Interop.Clients;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Security;
using System;
using System.Linq;
using System.Linq.Expressions;

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

		/// <summary>
		/// Accepts a certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request.</param>
		/// <returns>Returns the submission result.</returns>
		public SubmissionResult AcceptCertificateSigningRequest(string id)
		{
			return this.Client.Put<object, SubmissionResult>(string.Format("csr/accept/{0}", id), this.Client.Accept, null);
		}

		/// <summary>
		/// Creates a device in the IMS.
		/// </summary>
		/// <param name="device">The device to be created.</param>
		/// <returns>Returns the newly created device.</returns>
		public SecurityDevice CreateDevice(SecurityDevice device)
		{
			return this.Client.Post<SecurityDevice, SecurityDevice>("device/create", this.Client.Accept, device);
		}

		/// <summary>
		/// Creates a place in the IMS.
		/// </summary>
		/// <param name="place">The place to be created.</param>
		/// <returns>Returns the newly created place.</returns>
		public Place CreatePlace(Place place)
		{
			return this.Client.Post<Place, Place>("place/create", this.Client.Accept, place);
		}

		/// <summary>
		/// Creates a role in the IMS.
		/// </summary>
		/// <param name="role">The role to be created.</param>
		/// <returns>Returns the newly created role.</returns>
		public SecurityRoleInfo CreateRole(SecurityRoleInfo role)
		{
			return this.Client.Post<SecurityRoleInfo, SecurityRoleInfo>("role/create", this.Client.Accept, role);
		}

		/// <summary>
		/// Creates a user in the IMS.
		/// </summary>
		/// <param name="user">The user to be created.</param>
		/// <returns>Returns the newly created user.</returns>
		public SecurityUserInfo CreateUser(SecurityUserInfo user)
		{
			return this.Client.Post<SecurityUserInfo, SecurityUserInfo>("user/create", this.Client.Accept, user);
		}

		/// <summary>
		/// Deletes a device.
		/// </summary>
		/// <param name="id">The id of the device to be deleted.</param>
		/// <returns>Returns the deleted device.</returns>
		public SecurityDevice DeleteDevice(string id)
		{
			return this.Client.Delete<SecurityDevice>(string.Format("device/delete/{0}", id));
		}

		/// <summary>
		/// Deletes a place.
		/// </summary>
		/// <param name="id">The id of the place to be deleted.</param>
		/// <returns>Returns the deleted place.</returns>
		public Place DeletePlace(string id)
		{
			return this.Client.Delete<Place>(string.Format("place/delete/{0}", id));
		}

		/// <summary>
		/// Deletes a security policy.
		/// </summary>
		/// <param name="id">The id of the policy to be deleted.</param>
		/// <returns>Returns the deleted policy.</returns>
		public SecurityPolicyInfo DeletePolicy(string id)
		{
			return this.Client.Delete<SecurityPolicyInfo>(string.Format("policy/delete/{0}", id));
		}

		/// <summary>
		/// Deletes a security role.
		/// </summary>
		/// <param name="id">The id of the role to be deleted.</param>
		/// <returns>Returns the deleted role.</returns>
		public SecurityRoleInfo DeleteRole(string id)
		{
			return this.Client.Delete<SecurityRoleInfo>(string.Format("role/delete/{0}", id));
		}

		/// <summary>
		/// Deletes a security user.
		/// </summary>
		/// <param name="id">The id of the user to be deleted.</param>
		/// <returns>Returns the deleted user.</returns>
		public SecurityUserInfo DeleteUser(string id)
		{
			return this.Client.Delete<SecurityUserInfo>(string.Format("user/delete/{0}", id));
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
		/// Gets a list of certificates.
		/// </summary>
		/// <param name="query">The query expression to use to find the certificates.</param>
		/// <returns>Returns a collection of certificates which match the specified query.</returns>
		AmiCollection<X509Certificate2Info> GetCertificates(Expression<Func<X509Certificate2Info, bool>> query)
		{
			return this.Client.Get<AmiCollection<X509Certificate2Info>>("certificates", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets a list of concepts.
		/// </summary>
		/// <param name="query">The query expression to use to find the concepts.</param>
		/// <returns>Returns a collection of concepts which match the specified query.</returns>
		public AmiCollection<Concept> GetConcepts(Expression<Func<Concept, bool>> query)
		{
			return this.Client.Get<AmiCollection<Concept>>("concepts", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets a list of concept sets.
		/// </summary>
		/// <param name="query">The query expression to use to find the concept sets.</param>
		/// <returns>Returns a collection of concept sets which match the specified query.</returns>
		public AmiCollection<ConceptSet> GetConceptSets(Expression<Func<ConceptSet, bool>> query)
		{
			return this.Client.Get<AmiCollection<ConceptSet>>("conceptsets", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets a certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request to be retrieved.</param>
		/// <returns>Returns a certificate signing request.</returns>
		public SubmissionResult GetCertificateSigningRequest(string id)
		{
			return this.Client.Get<SubmissionResult>(string.Format("csr/{0}", id));
		}

		/// <summary>
		/// Gets a list of certificate signing requests.
		/// </summary>
		/// <param name="query">The query expression to use to find the certificate signing requests.</param>
		/// <returns>Returns a collection of certificate signing requests which match the specified query.</returns>
		public AmiCollection<SubmissionInfo> GetCertificateSigningRequests(Expression<Func<SubmissionInfo, bool>> query)
		{
			return this.Client.Get<AmiCollection<SubmissionInfo>>("csrs", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets a list of devices.
		/// </summary>
		/// <param name="query">The query expression to use to find the devices.</param>
		/// <returns>Returns a collection of devices which match the specified query.</returns>
		public AmiCollection<SecurityDevice> GetDevices(Expression<Func<SecurityDevice, bool>> query)
		{
			return this.Client.Get<AmiCollection<SecurityDevice>>("devices", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets a list of places.
		/// </summary>
		/// <param name="query">The query expression to use to find the places.</param>
		/// <returns>Returns a collection of places which match the specified query.</returns>
		public AmiCollection<Place> GetPlaces(Expression<Func<Place, bool>> query)
		{
			return this.Client.Get<AmiCollection<Place>>("places", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Retrieves a specified policy.
		/// </summary>
		/// <param name="query">The query expression to use to find the policy.</param>
		/// <returns>Returns a collection of policies which match the specified query parameters.</returns>
		public AmiCollection<SecurityPolicyInfo> GetPolicies(Expression<Func<SecurityPolicy, bool>> query)
		{
			var queryParms = QueryExpressionBuilder.BuildQuery(query).ToArray();
			return this.Client.Get<AmiCollection<SecurityPolicyInfo>>("policies", queryParms);
		}

		/// <summary>
		/// Gets a specific policy.
		/// </summary>
		/// <param name="id">The id of the policy to be retrieved.</param>
		/// <returns>Returns the specific policy.</returns>
		public SecurityPolicyInfo GetPolicy(string id)
		{
			return this.Client.Get<SecurityPolicyInfo>(string.Format("policy/{0}", id));
		}

		/// <summary>
		/// Gets a specific role.
		/// </summary>
		/// <param name="id">The id of the role to be retrieved.</param>
		/// <returns>Returns the specified user.</returns>
		public SecurityRoleInfo GetRole(string id)
		{
			return this.Client.Get<SecurityRoleInfo>(string.Format("role/{0}", id));
		}

		/// <summary>
		/// Retrieves a specified role.
		/// </summary>
		/// <param name="query">The query expression to use to find the role.</param>
		/// <returns>Returns a collection of roles which match the specified query parameters.</returns>
		public AmiCollection<SecurityRoleInfo> GetRoles(Expression<Func<SecurityRole, bool>> query)
		{
			var queryParms = QueryExpressionBuilder.BuildQuery(query).ToArray();
			return this.Client.Get<AmiCollection<SecurityRoleInfo>>("roles", queryParms);
		}

		/// <summary>
		/// Gets a specific user.
		/// </summary>
		/// <param name="id">The id of the user to be retrieved.</param>
		/// <returns>Returns the specified user.</returns>
		public SecurityUserInfo GetUser(string id)
		{
			return this.Client.Get<SecurityUserInfo>(string.Format("user/{0}", id));
		}

		/// <summary>
		/// Retrieves a specified user.
		/// </summary>
		/// <param name="query">The query expression to use to find the user.</param>
		/// <returns>Returns a collection of users which match the specified query parameters.</returns>
		public AmiCollection<SecurityUserInfo> GetUsers(Expression<Func<SecurityUserInfo, bool>> query)
		{
			var queryParms = QueryExpressionBuilder.BuildQuery(query).ToArray();
			return this.Client.Get<AmiCollection<SecurityUserInfo>>("users", queryParms);
		}

		/// <summary>
		/// Submits a certificate signing request to the AMI.
		/// </summary>
		/// <param name="submissionRequest">The certificate signing request.</param>
		/// <returns>Returns the submission result.</returns>
		public SubmissionResult SubmitCertificateSigningRequest(SubmissionRequest submissionRequest)
		{
			return this.Client.Post<SubmissionRequest, SubmissionResult>("csr", this.Client.Accept, submissionRequest);
		}

		/// <summary>
		/// Updates a concept.
		/// </summary>
		/// <param name="concept">The concept containing the updated information.</param>
		/// <returns>Returns the updated concept.</returns>
		public Concept UpdateConcept(Concept concept)
		{
			return this.Client.Put<Concept, Concept>("concept/update", this.Client.Accept, concept);
		}

		/// <summary>
		/// Updates a place.
		/// </summary>
		/// <param name="place">The place containing the updated information.</param>
		/// <returns>Returns the updated place.</returns>
		public Place UpdatePlace(Place place)
		{
			return this.Client.Put<Place, Place>("place/update", this.Client.Accept, place);
		}

		/// <summary>
		/// Updates a user.
		/// </summary>
		/// <param name="id">The id of the user to be updated.</param>
		/// <param name="user">The user containing the updated information.</param>
		/// <returns>Returns the updated user.</returns>
		public SecurityUserInfo UpdateUser(string id, SecurityUserInfo user)
		{
			return this.Client.Put<SecurityUserInfo, SecurityUserInfo>(string.Format("user/update/{0}", id), this.Client.Accept, user);
		}
	}
}