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
 * Date: 2016-8-2
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using MARC.Util.CertificateTools;
using OpenIZ.Core.Interop;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZ.Core.Model.AMI.Diagnostics;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Security.Claims;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.AMI.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.Pkcs;
using System.Security.Permissions;
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
	public partial class AmiBehavior : IAmiContract
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
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
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
		/// Changes the password of a user.
		/// </summary>
		/// <param name="id">The id of the user whose password is to be changed.</param>
		/// <param name="password">The new password of the user.</param>
		/// <returns>Returns the updated user.</returns>
		public SecurityUser ChangePassword(string id, string password)
		{
			Guid userKey = Guid.Empty;

			if (!Guid.TryParse(id, out userKey))
			{
				throw new ArgumentException(string.Format("{0} must be a valid GUID", nameof(id)));
			}

			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			if (securityRepository == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(ISecurityRepositoryService)));
			}

			return securityRepository.ChangePassword(userKey, password);
		}

		/// <summary>
		/// Creates an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityInfo">The assigning authority to be created.</param>
		/// <returns>Returns the created assigning authority.</returns>
		public AssigningAuthorityInfo CreateAssigningAuthority(AssigningAuthorityInfo assigningAuthorityInfo)
		{
			var assigningAuthorityRepositoryService = ApplicationContext.Current.GetService<IAssigningAuthorityRepositoryService>();

			if (assigningAuthorityRepositoryService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IAssigningAuthorityRepositoryService)));
			}

			var createdAssigningAuthority = assigningAuthorityRepositoryService.Insert(assigningAuthorityInfo.AssigningAuthority);

			return new AssigningAuthorityInfo(createdAssigningAuthority);
		}

		/// <summary>
		/// Create a diagnostic report
		/// </summary>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.Login)]
		public DiagnosticReport CreateDiagnosticReport(DiagnosticReport report)
		{
			var persister = ApplicationContext.Current.GetService<IDataPersistenceService<DiagnosticReport>>();
			if (persister == null)
				throw new InvalidOperationException("Cannot find appriopriate persister");
			return persister.Insert(report, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Deletes an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to be deleted.</param>
		/// <returns>Returns the deleted assigning authority.</returns>
		public AssigningAuthorityInfo DeleteAssigningAuthority(string assigningAuthorityId)
		{
			Guid assigningAuthorityKey = Guid.Empty;

			if (!Guid.TryParse(assigningAuthorityId, out assigningAuthorityKey))
			{
				throw new ArgumentException(string.Format("{0} must be a valid GUID", nameof(assigningAuthorityId)));
			}

			var assigningAuthorityService = ApplicationContext.Current.GetService<IAssigningAuthorityRepositoryService>();

			if (assigningAuthorityService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IAssigningAuthorityRepositoryService)));
			}

			return new AssigningAuthorityInfo()
			{
				AssigningAuthority = assigningAuthorityService.Obsolete(assigningAuthorityKey),
				Id = assigningAuthorityKey
			};
		}

		/// <summary>
		/// Deletes a specified certificate.
		/// </summary>
		/// <param name="id">The id of the certificate to be deleted.</param>
		/// <param name="reason">The reason the certificate is to be deleted.</param>
		/// <returns>Returns the deletion result.</returns>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
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
		/// Gets a list of assigning authorities for a specific query.
		/// </summary>
		/// <returns>Returns a list of assigning authorities which match the specific query.</returns>
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

			AmiCollection<AssigningAuthorityInfo> assigningAuthorities = new AmiCollection<AssigningAuthorityInfo>();

			int totalCount = 0;

			assigningAuthorities.CollectionItem = assigningAuthorityRepositoryService.Find(expression, 0, null, out totalCount).Select(a => new AssigningAuthorityInfo(a)).ToList();
			assigningAuthorities.Size = totalCount;

			return assigningAuthorities;
		}

		/// <summary>
		/// Gets a specific assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to retrieve.</param>
		/// <returns>Returns the assigning authority.</returns>
		public AssigningAuthorityInfo GetAssigningAuthority(string assigningAuthorityId)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(assigningAuthorityId, out key))
			{
				throw new ArgumentException($"{nameof(assigningAuthorityId)} must be a valid GUID");
			}

			var assigningAuthorityRepositoryService = ApplicationContext.Current.GetService<IAssigningAuthorityRepositoryService>();

			if (assigningAuthorityRepositoryService == null)
			{
				throw new InvalidOperationException($"{nameof(IAssigningAuthorityRepositoryService)} not found");
			}

			return new AssigningAuthorityInfo(assigningAuthorityRepositoryService.Get(key));
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
					WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
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
		/// Get a list of TFA mechanisms
		/// </summary>
		/// <returns>Returns a list of TFA mechanisms.</returns>
		public AmiCollection<TfaMechanismInfo> GetTfaMechanisms()
		{
			var tfaRelay = ApplicationContext.Current.GetService<ITfaRelayService>();
			if (tfaRelay == null)
				throw new InvalidOperationException("TFA Relay missing");
			return new AmiCollection<TfaMechanismInfo>()
			{
				CollectionItem = tfaRelay.Mechanisms.Select(o => new TfaMechanismInfo()
				{
					Id = o.Id,
					Name = o.Name,
					ChallengeText = o.Challenge
				}).ToList()
			};
		}

		/// <summary>
		/// Gets options for the AMI service.
		/// </summary>
		/// <returns>Returns options for the AMI service.</returns>
		public IdentifiedData Options()
		{
			WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.OK;
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Allow", $"GET, PUT, POST, OPTIONS, HEAD, DELETE{(ApplicationContext.Current.GetService<IPatchService>() != null ? ", PATCH" : null)}");

			if (ApplicationContext.Current.GetService<IPatchService>() != null)
			{
				WebOperationContext.Current.OutgoingResponse.Headers.Add("Accept-Patch", "application/xml+oiz-patch");
			}

			var serviceOptions = new ServiceOptions
			{
				InterfaceVersion = typeof(AmiCollection<>).Assembly.GetName().Version.ToString(),
				Services = new List<ServiceResourceOptions>
				{
					new ServiceResourceOptions()
					{
						ResourceName = null,
						Verbs = new List<string>() { "OPTIONS" }
					},
					new ServiceResourceOptions()
					{
						ResourceName = "time",
						Verbs = new List<string>() { "GET" }
					}
				}
			};

			//foreach (var methodInfo in typeof(IAmiContract).GetMethods().Where(m => m.GetCustomAttribute<WebInvokeAttribute>() != null))
			//{
			//	var webInvoke = methodInfo.GetCustomAttribute<WebInvokeAttribute>();
			//	serviceOptions.Services.Add(new ServiceResourceOptions(methodInfo.GetParameters()[0].ParameterType.Name, new List<string> { webInvoke.Method }));
			//}

			//foreach (var methodInfo in typeof(IAmiContract).GetMethods())
			//{
			//	var webInvoke = methodInfo.GetCustomAttribute<WebInvokeAttribute>();
			//	var webGet = methodInfo.GetCustomAttribute<WebGetAttribute>();

			//	if (webInvoke != null)
			//	{
			//		switch (webInvoke.Method)
			//		{
			//			case "DELETE":
			//				break;
			//			case "POST":
			//				serviceOptions.Services.Add(new ServiceResourceOptions(methodInfo.GetParameters()[0].ParameterType.Name, new List<string> { webInvoke.Method }));
			//				break;
			//			case "PUT":
			//				break;
			//		}
			//	}
			//	else if (webGet != null)
			//	{
			//		serviceOptions.Services.Add(new ServiceResourceOptions(methodInfo.Name, new List<string> { "GET" }));
			//	}
			//}

			return serviceOptions;
		}

		/// <summary>
		/// Rejects a specified certificate signing request.
		/// </summary>
		/// <param name="certId">The id of the certificate signing request to be rejected.</param>
		/// <param name="reason">The reason the certificate signing request is to be rejected.</param>
		/// <returns>Returns the rejection result.</returns>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
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
		/// Creates security reset information
		/// </summary>
		public void SendTfaSecret(TfaRequestInfo resetInfo)
		{
			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			var securityUser = securityRepository.GetUser(resetInfo.UserName);
			if (securityUser == null)
				throw new KeyNotFoundException();

			// Identity provider
			var identityProvider = ApplicationContext.Current.GetService<IIdentityProviderService>();
			var tfaSecret = identityProvider.GenerateTfaSecret(securityUser.UserName);

			// Add a claim
			if (resetInfo.Purpose == "PasswordReset")
			{
				new PolicyPermission(PermissionState.Unrestricted, PermissionPolicyIdentifiers.LoginAsService);
				identityProvider.AddClaim(securityUser.UserName, new System.Security.Claims.Claim(OpenIzClaimTypes.OpenIZPasswordlessAuth, "true"));
			}

			var tfaRelay = ApplicationContext.Current.GetService<ITfaRelayService>();
			if (tfaRelay == null)
				throw new InvalidOperationException("TFA relay not specified");

			// Now issue the TFA secret
			tfaRelay.SendSecret(resetInfo.ResetMechanism, securityUser, resetInfo.Verification, tfaSecret);
            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NoContent;
		}
		/// <summary>
		/// Submits a specific certificate signing request.
		/// </summary>
		/// <param name="s">The certificate signing request.</param>
		/// <returns>Returns the submission result.</returns>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
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
		/// Updates an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to be updated.</param>
		/// <param name="assigningAuthorityInfo">The assigning authority containing the updated information.</param>
		/// <returns>Returns the updated assigning authority.</returns>
		public AssigningAuthorityInfo UpdateAssigningAuthority(string assigningAuthorityId, AssigningAuthorityInfo assigningAuthorityInfo)
		{
			var id = Guid.Empty;

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

		/// <summary>
		/// Creates a query
		/// </summary>
		/// <param name="nvc">The name value collection to use to create the query.</param>
		/// <returns>Returns the created query.</returns>
		private NameValueCollection CreateQuery(System.Collections.Specialized.NameValueCollection nvc)
		{
			var retVal = new NameValueCollection();

			foreach (var k in nvc.AllKeys)
			{
				retVal.Add(k, new List<string>(nvc.GetValues(k)));
			}

			return retVal;
		}
	}
}