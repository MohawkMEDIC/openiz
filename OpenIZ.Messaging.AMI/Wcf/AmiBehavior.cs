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

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using MARC.Util.CertificateTools;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Security;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.AMI.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Pkcs;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.AMI.Wcf
{
	/// <summary>
	/// Represents the administrative contract interface.
	/// </summary>
	[ServiceBehavior(ConfigurationName = "AMI")]
	public class AmiBehavior : IAmiContract
	{
		// Certificate tool
		private CertTool m_certTool;

		// Configuration
		private AmiConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.messaging.ami") as AmiConfiguration;

		// Trace source
		private TraceSource m_traceSource = new TraceSource("OpenIZ.Messaging.AMI");

		/// <summary>
		/// Creates the AMI behavior
		/// </summary>
		public AmiBehavior()
		{
			this.m_certTool = new CertTool();
			this.m_certTool.CertificationAuthorityName = this.m_configuration?.CaConfiguration.Name;
			this.m_certTool.ServerName = this.m_configuration?.CaConfiguration.ServerName;
		}

		/// <summary>
		/// Accepts a certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request to be accepted.</param>
		/// <returns>Returns the acceptance result.</returns>
		public SubmissionResult AcceptCsr(string rawId)
		{
			int id = Int32.Parse(rawId);
			this.m_certTool.Approve(id);
			var submission = this.m_certTool.GetRequestStatus(id);

			var result = new SubmissionResult(submission.Message, submission.RequestId, (SubmissionStatus)submission.Outcome, submission.AuthorityResponse);
			result.Certificate = null;
			return result;
		}

		/// <summary>
		/// Creates a place in the IMS.
		/// </summary>
		/// <param name="place">The place to be created.</param>
		/// <returns>Returns the newly created place.</returns>
		public Place CreatePlace(Place place)
		{
			var placeRepository = ApplicationContext.Current.GetService<IPlaceRepositoryService>();

			if (placeRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} cannot be null", nameof(IPlaceRepositoryService)));
			}

			return placeRepository.Insert(place);
		}

		/// <summary>
		/// Creates a security policy.
		/// </summary>
		/// <param name="policy">The security policy to be created.</param>
		/// <returns>Returns the newly created security policy.</returns>
		public SecurityPolicyInfo CreatePolicy(SecurityPolicyInfo Policy)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates a query
		/// </summary>
		/// <param name="nvc">The name value collection to use to create the query.</param>
		/// <returns>Returns the created query.</returns>
		private NameValueCollection CreateQuery(System.Collections.Specialized.NameValueCollection nvc)
		{
			var retVal = new OpenIZ.Core.Model.Query.NameValueCollection();
			foreach (var k in nvc.AllKeys)
				retVal.Add(k, new List<String>(nvc.GetValues(k)));
			return retVal;
		}

		/// <summary>
		/// Creates a security role.
		/// </summary>
		/// <param name="role">The security role to be created.</param>
		/// <returns>Returns the newly created security role.</returns>
		public SecurityRoleInfo CreateRole(SecurityRoleInfo role)
		{
			var roleRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			var roleToCreate = new SecurityRole()
			{
				Name = role.Name
			};
			return new SecurityRoleInfo(roleRepository.CreateRole(roleToCreate));
		}

		/// <summary>
		/// Creates a security user.
		/// </summary>
		/// <param name="user">The security user to be created.</param>
		/// <returns>Returns the newly created security user.</returns>
		public SecurityUserInfo CreateUser(SecurityUserInfo user)
		{
			var userRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			var roleProviderService = ApplicationContext.Current.GetService<IRoleProviderService>();

			var userToCreate = new Core.Model.Security.SecurityUser()
			{
				UserName = user.UserName,
				Email = user.Email
			};

			if (user.Lockout)
			{
				userToCreate.Lockout = DateTime.UtcNow;
			}

			var securityUser = userRepository.CreateUser(userToCreate, user.Password);

			if (user.Roles != null)
				roleProviderService.AddUsersToRoles(new String[] { user.UserName }, user.Roles.Select(o => o.Name).ToArray(), AuthenticationContext.Current.Principal);

			return new SecurityUserInfo(securityUser);
		}

		/// <summary>
		/// Deletes a specified certificate.
		/// </summary>
		/// <param name="id">The id of the certificate to be deleted.</param>
		/// <param name="reason">The reason the certificate is to be deleted.</param>
		/// <returns>Returns the deletion result.</returns>
		public SubmissionResult DeleteCertificate(string rawId, OpenIZ.Core.Model.AMI.Security.RevokeReason reason)
		{
			int id = Int32.Parse(rawId);
			var result = this.m_certTool.GetRequestStatus(id);

			if (String.IsNullOrEmpty(result.AuthorityResponse))
				throw new InvalidOperationException("Cannot revoke an un-issued certificate");
			// Now get the serial key
			SignedCms importer = new SignedCms();
			importer.Decode(Convert.FromBase64String(result.AuthorityResponse));

			foreach (var cert in importer.Certificates)
				if (cert.Subject != cert.Issuer)
					this.m_certTool.RevokeCertificate(cert.SerialNumber, (MARC.Util.CertificateTools.RevokeReason)reason);

			result.Outcome = SubmitOutcome.Revoked;
			result.AuthorityResponse = null;
			return new SubmissionResult(result.Message, result.RequestId, (SubmissionStatus)result.Outcome, result.AuthorityResponse);
		}

		/// <summary>
		/// Deletes a place.
		/// </summary>
		/// <param name="placeId">The id of the place to be deleted.</param>
		/// <returns>Returns the deleted place.</returns>
		public Place DeletePlace(string placeId)
		{
			Guid placeKey = Guid.Empty;

			if (!Guid.TryParse(placeId, out placeKey))
			{
				throw new ArgumentException(string.Format("{0} must be a valid GUID", nameof(placeId)));
			}

			var placeRepository = ApplicationContext.Current.GetService<IPlaceRepositoryService>();

			if (placeRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IPlaceRepositoryService)));
			}

			return placeRepository.Obsolete(placeKey);
		}

		/// <summary>
		/// Deletes a security policy.
		/// </summary>
		/// <param name="policyId">The id of the policy to be deleted.</param>
		/// <returns>Returns the deleted policy.</returns>
		public SecurityPolicyInfo DeletePolicy(string policyId)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Deletes a security role.
		/// </summary>
		/// <param name="roleId">The id of the role to be deleted.</param>
		/// <returns>Returns the deleted role.</returns>
		public SecurityRoleInfo DeleteRole(string rawRoleId)
		{
			Guid roleId = Guid.Empty;
			if (!Guid.TryParse(rawRoleId, out roleId))
				throw new ArgumentException(nameof(rawRoleId));
			var roleRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			return new SecurityRoleInfo(roleRepository.ObsoleteRole(roleId));
		}

		/// <summary>
		/// Deletes a security user.
		/// </summary>
		/// <param name="userId">The id of the user to be deleted.</param>
		/// <returns>Returns the deleted user.</returns>
		public SecurityUserInfo DeleteUser(string rawUserId)
		{
			Guid userId = Guid.Parse(rawUserId);
			var userRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			return new SecurityUserInfo(userRepository.ObsoleteUser(userId));
		}

		/// <summary>
		/// Gets a specific certificate.
		/// </summary>
		/// <param name="id">The id of the certificate to retrieve.</param>
		/// <returns>Returns the certificate.</returns>
		public byte[] GetCertificate(string rawId)
		{
			int id = Int32.Parse(rawId);
			WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pkcs12";
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", String.Format("attachment; filename=\"crt-{0}.p12\"", id));
			var result = this.m_certTool.GetRequestStatus(id);
			return Encoding.UTF8.GetBytes(result.AuthorityResponse);
		}

		/// <summary>
		/// Gets a list of certificates.
		/// </summary>
		/// <returns>Returns a list of certificates.</returns>
		public AmiCollection<X509Certificate2Info> GetCertificates()
		{
			AmiCollection<X509Certificate2Info> collection = new AmiCollection<X509Certificate2Info>();
			var certs = this.m_certTool.GetCertificates();
			foreach (var cert in certs)
				collection.CollectionItem.Add(new X509Certificate2Info(cert.Attribute));
			return collection;
		}

		/// <summary>
		/// Gets a list of concepts.
		/// </summary>
		/// <returns>Returns a list of concepts.</returns>
		public AmiCollection<Concept> GetConcepts()
		{
			var parameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

			if (parameters.Count == 0)
			{
				throw new ArgumentException(string.Format("{0} cannot be empty", nameof(parameters)));
			}

			var expression = QueryExpressionParser.BuildLinqExpression<Concept>(this.CreateQuery(parameters));

			var conceptRepository = ApplicationContext.Current.GetService<IConceptRepositoryService>();

			if (conceptRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IConceptRepositoryService)));
			}

			return new AmiCollection<Concept>()
			{
				CollectionItem = conceptRepository.FindConcepts(expression).ToList()
			};
		}

		/// <summary>
		/// Gets a list of concept sets.
		/// </summary>
		/// <returns>Returns a list of concept sets.</returns>
		public AmiCollection<ConceptSet> GetConceptSets()
		{
			var parameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

			if (parameters.Count == 0)
			{
				throw new ArgumentException(string.Format("{0} cannot be empty", nameof(parameters)));
			}

			var expression = QueryExpressionParser.BuildLinqExpression<ConceptSet>(this.CreateQuery(parameters));

			var conceptRepository = ApplicationContext.Current.GetService<IConceptRepositoryService>();

			if (conceptRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IConceptRepositoryService)));
			}

			return new AmiCollection<ConceptSet>()
			{
				CollectionItem = conceptRepository.FindConceptSets(expression).ToList()
			};
		}

		/// <summary>
		/// Gets the certificate revocation list.
		/// </summary>
		/// <returns>Returns the certificate revocation list.</returns>
		public byte[] GetCrl()
		{
			WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pkcs7-crl";
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", "attachment; filename=\"openiz.crl\"");
			return Encoding.UTF8.GetBytes(this.m_certTool.GetCRL());
		}

		/// <summary>
		/// Gets a specific certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request to be retrieved.</param>
		/// <returns>Returns the certificate signing request.</returns>
		public SubmissionResult GetCsr(string rawId)
		{
			int id = Int32.Parse(rawId);
			var submission = this.m_certTool.GetRequestStatus(id);

			var result = new SubmissionResult(submission.Message, submission.RequestId, (SubmissionStatus)submission.Outcome, submission.AuthorityResponse);
			return result;
		}

		/// <summary>
		/// Gets a list of submitted certificate signing requests.
		/// </summary>
		/// <returns>Returns a list of certificate signing requests.</returns>
		public AmiCollection<SubmissionInfo> GetCsrs()
		{
			AmiCollection<SubmissionInfo> collection = new AmiCollection<SubmissionInfo>();
			var certs = this.m_certTool.GetCertificates();
			foreach (var cert in certs)
			{
				SubmissionInfo info = new SubmissionInfo();
				foreach (var kv in cert.Attribute)
				{
					string key = kv.Key.Replace("Request.", "");
					PropertyInfo pi = typeof(CertificateInfo).GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
					if (pi != null)
						pi.SetValue(info, kv.Value, null);
				}
				info.XmlStatusCode = (SubmissionStatus)this.m_certTool.GetRequestStatus(Int32.Parse(info.RequestID)).Outcome;
				if (info.XmlStatusCode == SubmissionStatus.Submission)
					collection.CollectionItem.Add(info);
			}
			return collection;
		}

		/// <summary>
		/// Gets a list of devices.
		/// </summary>
		/// <returns>Returns a list of devices.</returns>
		public AmiCollection<SecurityDevice> GetDevices()
		{
			var parameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

			if (parameters.Count == 0)
			{
				throw new ArgumentException(string.Format("{0} cannot be empty", nameof(parameters)));
			}

			var expression = QueryExpressionParser.BuildLinqExpression<ConceptSet>(this.CreateQuery(parameters));

			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(ISecurityRepositoryService)));
			}

			return new AmiCollection<SecurityDevice>
			{
			};
		}

		/// <summary>
		/// Gets a list of places.
		/// </summary>
		/// <returns>Returns a list of places.</returns>
		public AmiCollection<Place> GetPlaces()
		{
			var parameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

			if (parameters.Count == 0)
			{
				throw new ArgumentException(string.Format("{0} cannot be empty", nameof(parameters)));
			}

			var expression = QueryExpressionParser.BuildLinqExpression<Place>(this.CreateQuery(parameters));

			var placeRepository = ApplicationContext.Current.GetService<IPlaceRepositoryService>();

			if (placeRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IPlaceRepositoryService)));
			}

			return new AmiCollection<Place>()
			{
				CollectionItem = placeRepository.Find(expression).ToList()
			};
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
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a specific security role.
		/// </summary>
		/// <param name="roleId">The id of the security role to be retrieved.</param>
		/// <returns>Returns the security role.</returns>
		public SecurityRoleInfo GetRole(string rawRoleId)
		{
			Guid roleId = Guid.Empty;
			if (!Guid.TryParse(rawRoleId, out roleId))
				throw new ArgumentException(nameof(rawRoleId));
			var roleRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			return new SecurityRoleInfo(roleRepository.GetRole(roleId));
		}

		/// <summary>
		/// Gets a list of security roles.
		/// </summary>
		/// <returns>Returns a list of security roles.</returns>
		public AmiCollection<SecurityRoleInfo> GetRoles()
		{
			var expression = QueryExpressionParser.BuildLinqExpression<SecurityRole>(this.CreateQuery(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters));
			var userRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			return new AmiCollection<SecurityRoleInfo>() { CollectionItem = userRepository.FindRoles(expression).Select(o => new SecurityRoleInfo(o)).ToList() };
		}

		/// <summary>
		/// Gets the schema for the administrative interface.
		/// </summary>
		/// <param name="schemaId">The id of the schema to be retrieved.</param>
		/// <returns>Returns the administrative interface schema.</returns>
		public XmlSchema GetSchema(int schemaId)
		{
			try
			{
				XmlSchemas schemaCollection = new XmlSchemas();

				XmlReflectionImporter importer = new XmlReflectionImporter("http://openiz.org/ami");
				XmlSchemaExporter exporter = new XmlSchemaExporter(schemaCollection);

				foreach (var cls in typeof(IAmiContract).GetCustomAttributes<ServiceKnownTypeAttribute>().Select(o => o.Type))
					exporter.ExportTypeMapping(importer.ImportTypeMapping(cls, "http://openiz.org/ami"));

				if (schemaId > schemaCollection.Count)
				{
					WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
					return null;
				}
				else
				{
					WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
					WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
					return schemaCollection[schemaId];
				}
			}
			catch (Exception e)
			{
				WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
				this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
				return null;
			}
		}

		/// <summary>
		/// Gets a specific security user.
		/// </summary>
		/// <param name="userId">The id of the security user to be retrieved.</param>
		/// <returns>Returns the security user.</returns>
		public SecurityUserInfo GetUser(string rawUserId)
		{
			Guid userId = Guid.Empty;
			if (!Guid.TryParse(rawUserId, out userId))
				throw new ArgumentException(nameof(rawUserId));
			var userRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			return new SecurityUserInfo(userRepository.GetUser(userId));
		}

		/// <summary>
		/// Gets a list of security users.
		/// </summary>
		/// <returns>Returns a list of security users.</returns>
		public AmiCollection<SecurityUserInfo> GetUsers()
		{
			var expression = QueryExpressionParser.BuildLinqExpression<SecurityUser>(this.CreateQuery(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters));
			var userRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			return new AmiCollection<SecurityUserInfo>() { CollectionItem = userRepository.FindUsers(expression).Select(o => new SecurityUserInfo(o)).ToList() };
		}

		/// <summary>
		/// Rejects a specified certificate signing request.
		/// </summary>
		/// <param name="certId">The id of the certificate signing request to be rejected.</param>
		/// <param name="reason">The reason the certificate signing request is to be rejected.</param>
		/// <returns>Returns the rejection result.</returns>
		public SubmissionResult RejectCsr(string rawId, OpenIZ.Core.Model.AMI.Security.RevokeReason reason)
		{
			int id = Int32.Parse(rawId);
			this.m_certTool.DenyRequest(id);
			var status = this.m_certTool.GetRequestStatus(id);

			var result = new SubmissionResult(status.Message, status.RequestId, (SubmissionStatus)status.Outcome, status.AuthorityResponse);
			result.Certificate = null;
			return result;
		}

		/// <summary>
		/// Submits a specific certificate signing request.
		/// </summary>
		/// <param name="s">The certificate signing request.</param>
		/// <returns>Returns the submission result.</returns>
		public SubmissionResult SubmitCsr(SubmissionRequest s)
		{
			var submission = this.m_certTool.SubmitRequest(s.CmcRequest, s.AdminContactName, s.AdminAddress);

			var result = new SubmissionResult(submission.Message, submission.RequestId, (SubmissionStatus)submission.Outcome, submission.AuthorityResponse);
			if (this.m_configuration.CaConfiguration.AutoApprove)
				return this.AcceptCsr(result.RequestId.ToString());
			else
				return result;
		}

		/// <summary>
		/// Updates a concept.
		/// </summary>
		/// <param name="concept">The concept containing the updated model.</param>
		/// <returns>Returns the newly updated concept.</returns>
		public Concept UpdateConcept(Concept concept)
		{
			var conceptRepository = ApplicationContext.Current.GetService<IConceptRepositoryService>();

			if (conceptRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IConceptRepositoryService)));
			}

			return conceptRepository.SaveConcept(concept);
		}

		/// <summary>
		/// Updates a place.
		/// </summary>
		/// <param name="place">The place containing the update information.</param>
		/// <returns>Returns the updated place.</returns>
		public Place UpdatePlace(Place place)
		{
			var placeRepository = ApplicationContext.Current.GetService<IPlaceRepositoryService>();

			if (placeRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IPlaceRepositoryService)));
			}

			return placeRepository.Save(place);
		}

		/// <summary>
		/// Updates a security user.
		/// </summary>
		/// <param name="userId">The id of the security user to be updated.</param>
		/// <param name="info">The security user containing the updated information.</param>
		/// <returns>Returns the updated security user.</returns>
		public SecurityUserInfo UpdateUser(string rawUserId, SecurityUserInfo info)
		{
			Guid userId = Guid.Parse(rawUserId);
			// First change password if needed
			var userRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			if (!String.IsNullOrEmpty(info.Password))
				userRepository.ChangePassword(userId, info.Password);

			SecurityUserInfo userInfo = new SecurityUserInfo(userRepository.SaveUser(new Core.Model.Security.SecurityUser()
			{
				Key = userId,
				Email = info.Email
			}));

			if (info.Lockout)
			{
				userInfo.Lockout = true;
			}

			// First, we remove the roles
			if (userInfo.Roles != null)
			{
				var irps = ApplicationContext.Current.GetService<IRoleProviderService>();
				irps.RemoveUsersFromRoles(new String[] { userInfo.UserName }, userInfo.Roles.Select(o => o.Name).ToArray(), AuthenticationContext.Current.Principal);
				irps.AddUsersToRoles(new String[] { userInfo.UserName }, userInfo.Roles.Select(o => o.Name).ToArray(), AuthenticationContext.Current.Principal);
			}

			return userInfo;
		}
	}
}