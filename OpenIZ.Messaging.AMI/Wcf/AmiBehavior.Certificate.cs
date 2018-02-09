/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.Util.CertificateTools;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Messaging.AMI.Configuration;
using SwaggerWcf.Attributes;
using System;
using System.Reflection;
using System.Security.Cryptography.Pkcs;
using System.Security.Permissions;
using System.ServiceModel.Web;
using System.Text;

namespace OpenIZ.Messaging.AMI.Wcf
{
	/// <summary>
	/// Represents the administrative contract interface.
	/// </summary>
	public partial class AmiBehavior
	{
		// Certificate tool
		private readonly CertTool certTool;

		// Configuration
		private readonly AmiConfiguration configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.messaging.ami") as AmiConfiguration;

		/// <summary>
		/// Creates the AMI behavior
		/// </summary>
		public AmiBehavior()
		{
			this.certTool = new CertTool
			{
				CertificationAuthorityName = this.configuration?.CaConfiguration.Name,
				ServerName = this.configuration?.CaConfiguration.ServerName
			};
		}

		/// <summary>
		/// Accepts a certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request to be accepted.</param>
		/// <returns>Returns the acceptance result.</returns>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(409, "You are attempting to create a resource that already exists")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(201, "The object was created successfully")]
		public SubmissionResult AcceptCsr(string rawId)
		{
			int id = Int32.Parse(rawId);
			this.certTool.Approve(id);
			var submission = this.certTool.GetRequestStatus(id);

			var result = new SubmissionResult(submission.Message, submission.RequestId, (SubmissionStatus)submission.Outcome, submission.AuthorityResponse);
			result.Certificate = null;
			return result;
		}

		/// <summary>
		/// Deletes a specified certificate.
		/// </summary>
		/// <param name="rawId">The raw identifier.</param>
		/// <param name="reason">The reason the certificate is to be deleted.</param>
		/// <returns>Returns the deletion result.</returns>
		/// <exception cref="System.InvalidOperationException">Cannot revoke an un-issued certificate</exception>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(409, "You are attempting to perform an obsolete on an old version of the resource, or the conditional HTTP headers don't match the current version of the resource")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The object was obsoleted successfully")]
		public SubmissionResult DeleteCertificate(string rawId, String strReason)
		{
			// Revoke reason
			var reason = (OpenIZ.Core.Model.AMI.Security.RevokeReason)Enum.Parse(typeof(OpenIZ.Core.Model.AMI.Security.RevokeReason), strReason);
			int id = Int32.Parse(rawId);
			var result = this.certTool.GetRequestStatus(id);

			if (String.IsNullOrEmpty(result.AuthorityResponse))
				throw new InvalidOperationException("Cannot revoke an un-issued certificate");
			// Now get the serial key
			SignedCms importer = new SignedCms();
			importer.Decode(Convert.FromBase64String(result.AuthorityResponse));

			foreach (var cert in importer.Certificates)
				if (cert.Subject != cert.Issuer)
					this.certTool.RevokeCertificate(cert.SerialNumber, (MARC.Util.CertificateTools.RevokeReason)reason);

			result.Outcome = SubmitOutcome.Revoked;
			result.AuthorityResponse = null;
			return new SubmissionResult(result.Message, result.RequestId, (SubmissionStatus)result.Outcome, result.AuthorityResponse);
		}

		/// <summary>
		/// Gets a specific certificate.
		/// </summary>
		/// <param name="rawId">The raw identifier.</param>
		/// <returns>Returns the certificate.</returns>
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The operation was successful, and the most recent version of the resource is in the response")]
		public byte[] GetCertificate(string rawId)
		{
			var id = int.Parse(rawId);

			WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pkcs12";
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", $"attachment; filename=\"crt-{id}.p12\"");

			var result = this.certTool.GetRequestStatus(id);

			return Encoding.UTF8.GetBytes(result.AuthorityResponse);
		}

		/// <summary>
		/// Gets a list of certificates.
		/// </summary>
		/// <returns>Returns a list of certificates.</returns>
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The operation was successful, and the most recent version of the resource is in the response")]
		public AmiCollection<X509Certificate2Info> GetCertificates()
		{
			var collection = new AmiCollection<X509Certificate2Info>();

			var certs = this.certTool.GetCertificates();

			foreach (var cert in certs)
			{
				collection.CollectionItem.Add(new X509Certificate2Info(cert.Attribute));
			}

			return collection;
		}

		/// <summary>
		/// Gets the certificate revocation list.
		/// </summary>
		/// <returns>Returns the certificate revocation list.</returns>
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The operation was successful, and the most recent version of the resource is in the response")]
		public byte[] GetCrl()
		{
			WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pkcs7-crl";
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", "attachment; filename=\"openiz.crl\"");
			return Encoding.UTF8.GetBytes(this.certTool.GetCRL());
		}

		/// <summary>
		/// Gets a specific certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request to be retrieved.</param>
		/// <returns>Returns the certificate signing request.</returns>
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The operation was successful, and the most recent version of the resource is in the response")]
		public SubmissionResult GetCsr(string rawId)
		{
			int id = Int32.Parse(rawId);
			var submission = this.certTool.GetRequestStatus(id);

			var result = new SubmissionResult(submission.Message, submission.RequestId, (SubmissionStatus)submission.Outcome, submission.AuthorityResponse);
			return result;
		}

		/// <summary>
		/// Gets a list of submitted certificate signing requests.
		/// </summary>
		/// <returns>Returns a list of certificate signing requests.</returns>
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The operation was successful, and the most recent version of the resource is in the response")]
		public AmiCollection<SubmissionInfo> GetCsrs()
		{
			AmiCollection<SubmissionInfo> collection = new AmiCollection<SubmissionInfo>();
			var certs = this.certTool.GetCertificates();
			foreach (var cert in certs)
			{
				SubmissionInfo info = new SubmissionInfo();
				foreach (var kv in cert.Attribute)
				{
					var key = kv.Key.Replace("Request.", "");
					var pi = typeof(CertificateInfo).GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
					pi?.SetValue(info, kv.Value, null);
				}
				info.XmlStatusCode = (SubmissionStatus)this.certTool.GetRequestStatus(Int32.Parse(info.RequestID)).Outcome;
				if (info.XmlStatusCode == SubmissionStatus.Submission)
					collection.CollectionItem.Add(info);
			}
			return collection;
		}

		/// <summary>
		/// Rejects a specified certificate signing request.
		/// </summary>
		/// <param name="certId">The id of the certificate signing request to be rejected.</param>
		/// <param name="reason">The reason the certificate signing request is to be rejected.</param>
		/// <returns>Returns the rejection result.</returns>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The operation was successful, and the most recent version of the resource is in the response")]
		public SubmissionResult RejectCsr(string rawId, OpenIZ.Core.Model.AMI.Security.RevokeReason reason)
		{
			int id = Int32.Parse(rawId);
			this.certTool.DenyRequest(id);
			var status = this.certTool.GetRequestStatus(id);

			var result = new SubmissionResult(status.Message, status.RequestId, (SubmissionStatus)status.Outcome, status.AuthorityResponse);
			result.Certificate = null;
			return result;
		}

		/// <summary>
		/// Submits a specific certificate signing request.
		/// </summary>
		/// <param name="s">The certificate signing request.</param>
		/// <returns>Returns the submission result.</returns>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(409, "You are attempting to create a resource that already exists")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(201, "The object was created successfully")]
		public SubmissionResult SubmitCsr(SubmissionRequest s)
		{
			var submission = this.certTool.SubmitRequest(s.CmcRequest, s.AdminContactName, s.AdminAddress);

			var result = new SubmissionResult(submission.Message, submission.RequestId, (SubmissionStatus)submission.Outcome, submission.AuthorityResponse);
			if (this.configuration.CaConfiguration.AutoApprove)
				return this.AcceptCsr(result.RequestId.ToString());
			else
				return result;
		}
	}
}