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
 * Date: 2016-6-22
 */
using OpenIZ.Core.Model.AMI;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using System;
using System.Linq.Expressions;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Schema;

namespace OpenIZ.Messaging.AMI.Wcf
{
	/// <summary>
	/// Administrative management interface contract
	/// </summary>
	[ServiceContract(ConfigurationName = "AMI_1.0", Name = "AMI"), XmlSerializerFormat]
	[ServiceKnownType(typeof(Place))]
	[ServiceKnownType(typeof(Entity))]
	[ServiceKnownType(typeof(Concept))]
	[ServiceKnownType(typeof(ConceptSet))]
	[ServiceKnownType(typeof(Organization))]
	[ServiceKnownType(typeof(DeviceEntity))]
	[ServiceKnownType(typeof(ReferenceTerm))]
	[ServiceKnownType(typeof(SubmissionInfo))]
	[ServiceKnownType(typeof(SecurityUserInfo))]
	[ServiceKnownType(typeof(SecurityRoleInfo))]
	[ServiceKnownType(typeof(SubmissionResult))]
	[ServiceKnownType(typeof(ApplicationEntity))]
	[ServiceKnownType(typeof(SubmissionRequest))]
	[ServiceKnownType(typeof(X509Certificate2Info))]
	[ServiceKnownType(typeof(AmiCollection<SubmissionInfo>))]
	[ServiceKnownType(typeof(AmiCollection<SecurityUserInfo>))]
	[ServiceKnownType(typeof(AmiCollection<X509Certificate2Info>))]
	public interface IAmiContract
	{
		/// <summary>
		/// Accept the CSR
		/// </summary>
		[WebInvoke(UriTemplate = "/csr/accept/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		SubmissionResult AcceptCsr(string id);

		/// <summary>
		/// Create a security Policy
		/// </summary>
		[WebInvoke(UriTemplate = "/policy/create", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		SecurityPolicyInfo CreatePolicy(SecurityPolicyInfo Policy);

		/// <summary>
		/// Create a security Role
		/// </summary>
		[WebInvoke(UriTemplate = "/role/create", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		SecurityRoleInfo CreateRole(SecurityRoleInfo Role);

		/// <summary>
		/// Create a security user
		/// </summary>
		[WebInvoke(UriTemplate = "/user/create", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		SecurityUserInfo CreateUser(SecurityUserInfo user);

		/// <summary>
		/// Reject the specified CSR
		/// </summary>
		[WebInvoke(UriTemplate = "/certificate/delete/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		SubmissionResult DeleteCertificate(string id, RevokeReason reason);

		/// <summary>
		/// Delete a security Policy
		/// </summary>
		[WebInvoke(UriTemplate = "/policy/delete/{policyId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		SecurityPolicyInfo DeletePolicy(string policyId);

		/// <summary>
		/// Delete a security Role
		/// </summary>
		[WebInvoke(UriTemplate = "/role/delete/{roleId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		SecurityRoleInfo DeleteRole(string roleId);

		/// <summary>
		/// Delete a security user
		/// </summary>
		[WebInvoke(UriTemplate = "/user/delete/{userId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		SecurityUserInfo DeleteUser(string userId);

		/// <summary>
		/// Get the specified certificate
		/// </summary>
		[WebGet(UriTemplate = "/certificate/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
		byte[] GetCertificate(string id);

		/// <summary>
		/// Get specified certificates
		/// </summary>
		[WebGet(UriTemplate = "/certificates", BodyStyle = WebMessageBodyStyle.Bare)]
		AmiCollection<X509Certificate2Info> GetCertificates();

		/// <summary>
		/// Gets a list of concepts.
		/// </summary>
		/// <returns>Returns a list of concepts.</returns>
		[WebGet(UriTemplate = "/concepts", BodyStyle = WebMessageBodyStyle.Bare)]
		AmiCollection<Concept> GetConcepts();

		/// <summary>
		/// Gets a list of concept sets.
		/// </summary>
		/// <returns>Returns a list of concept sets.</returns>
		[WebGet(UriTemplate = "/conceptsets", BodyStyle = WebMessageBodyStyle.Bare)]
		AmiCollection<ConceptSet> GetConceptSets();

		/// <summary>
		/// Get the CRL
		/// </summary>
		/// <returns></returns>
		[WebGet(UriTemplate = "/crl", BodyStyle = WebMessageBodyStyle.Bare)]
		byte[] GetCrl();

		/// <summary>
		/// Gets the specified CSR request
		/// </summary>
		[WebGet(UriTemplate = "/csr/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
		SubmissionResult GetCsr(string id);

		/// <summary>
		/// Get the submissions
		/// </summary>
		[WebGet(UriTemplate = "/csrs", BodyStyle = WebMessageBodyStyle.Bare)]
		AmiCollection<SubmissionInfo> GetCsrs();

		/// <summary>
		/// Gets a list of devices.
		/// </summary>
		/// <returns>Returns a list of devices.</returns>
		[WebGet(UriTemplate = "/devices", BodyStyle = WebMessageBodyStyle.Bare)]
		AmiCollection<SecurityDevice> GetDevices();

		/// <summary>
		/// Security Policy information
		/// </summary>
		/// <returns></returns>
		[WebGet(UriTemplate = "/policies", BodyStyle = WebMessageBodyStyle.Bare)]
		AmiCollection<SecurityPolicyInfo> GetPolicies();

		/// <summary>
		/// Get a security Policy
		/// </summary>
		[WebGet(UriTemplate = "/policy/{policyId}", BodyStyle = WebMessageBodyStyle.Bare)]
		SecurityPolicyInfo GetPolicy(string policyId);

		/// <summary>
		/// Get a security Role
		/// </summary>
		[WebGet(UriTemplate = "/role/{roleId}", BodyStyle = WebMessageBodyStyle.Bare)]
		SecurityRoleInfo GetRole(string roleId);

		/// <summary>
		/// Security Role information
		/// </summary>
		/// <returns></returns>
		[WebGet(UriTemplate = "/roles", BodyStyle = WebMessageBodyStyle.Bare)]
		AmiCollection<SecurityRoleInfo> GetRoles();

		/// <summary>
		/// Get the schema
		/// </summary>
		[WebGet(UriTemplate = "/?xsd={schemaId}")]
		XmlSchema GetSchema(int schemaId);

		/// <summary>
		/// Get a security user
		/// </summary>
		[WebGet(UriTemplate = "/user/{userId}", BodyStyle = WebMessageBodyStyle.Bare)]
		SecurityUserInfo GetUser(string userId);

		/// <summary>
		/// Security user information
		/// </summary>
		/// <returns></returns>
		[WebGet(UriTemplate = "/users", BodyStyle = WebMessageBodyStyle.Bare)]
		AmiCollection<SecurityUserInfo> GetUsers();

		/// <summary>
		/// Reject the specified CSR
		/// </summary>
		[WebInvoke(UriTemplate = "/csr/{certId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		SubmissionResult RejectCsr(string certId, RevokeReason reason);

		/// <summary>
		/// Submit the specified CSR
		/// </summary>
		[WebInvoke(UriTemplate = "/csr/submit", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		SubmissionResult SubmitCsr(SubmissionRequest s);

		/// <summary>
		/// Updates a concept.
		/// </summary>
		/// <param name="rawConceptId">The id of the concept to be updated.</param>
		/// <param name="concept">The concept containing the updated model.</param>
		/// <returns>Returns the newly updated concept.</returns>
		[WebInvoke(UriTemplate = "/concept/update/{rawConceptId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		Concept UpdateConcept(string rawConceptId, Concept concept);

		/// <summary>
		/// Get a security user
		/// </summary>
		[WebInvoke(UriTemplate = "/user/update/{userId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		SecurityUserInfo UpdateUser(string userId, SecurityUserInfo info);

	}
}