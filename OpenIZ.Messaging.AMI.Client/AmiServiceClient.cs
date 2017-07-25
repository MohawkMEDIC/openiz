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
 * Date: 2016-8-2
 */

using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Http;
using OpenIZ.Core.Interop;
using OpenIZ.Core.Interop.Clients;
using OpenIZ.Core.Model.AMI.Alerting;
using OpenIZ.Core.Model.AMI.Applet;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZ.Core.Model.AMI.Diagnostics;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.IO;
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
		/// Initializes a new instance of the <see cref="AmiServiceClient"/> class
		/// with a specified <see cref="IRestClient"/> instance.
		/// </summary>
		/// <param name="client">The <see cref="IRestClient"/> instance.</param>
		public AmiServiceClient(IRestClient client) : base(client)
		{
			this.Client.Accept = client.Accept ?? "application/xml";
		}

		/// <summary>
		/// Accepts a certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request.</param>
		/// <returns>Returns the submission result.</returns>
		public SubmissionResult AcceptCertificateSigningRequest(string id)
		{
			return this.Client.Put<object, SubmissionResult>($"csr/accept/{id}", this.Client.Accept, null);
		}

		/// <summary>
		/// Gets a specific assigning authority.
		/// </summary>
		/// <param name="id">The id of the assigning authority to retrieve.</param>
		/// <returns>Returns the assigning authority.</returns>
		public AssigningAuthorityInfo AssigningAuthority(string id)
		{
			return this.Client.Get<AssigningAuthorityInfo>($"assigningAuthority/{id}");
		}

		/// <summary>
		/// Creates an alert message.
		/// </summary>
		/// <param name="alertMessageInfo">The alert message to be created.</param>
		/// <returns>Returns the created alert message.</returns>
		public AlertMessageInfo CreateAlert(AlertMessageInfo alertMessageInfo)
		{
			return this.Client.Post<AlertMessageInfo, AlertMessageInfo>("alert", this.Client.Accept, alertMessageInfo);
		}

		/// <summary>
		/// Creates an applet.
		/// </summary>
		/// <returns>Returns the created applet manifest info.</returns>
		public AppletManifestInfo CreateApplet(AppletPackage appletPackage)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				appletPackage.Save(ms);
				ms.Flush();
				return this.Client.Post<byte[], AppletManifestInfo>("applet", "application/octet-stream", ms.ToArray());
			}
		}

		/// <summary>
		/// Creates a security application.
		/// </summary>
		/// <param name="applicationInfo">The security application to be created.</param>
		/// <returns>Returns the created security application.</returns>
		public SecurityApplicationInfo CreateApplication(SecurityApplicationInfo applicationInfo)
		{
			return this.Client.Post<SecurityApplicationInfo, SecurityApplicationInfo>("application", this.Client.Accept, applicationInfo);
		}

		/// <summary>
		/// Creates an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityInfo">The assigning authority to be created.</param>
		/// <returns>Returns the created assigning authority.</returns>
		public AssigningAuthorityInfo CreateAssigningAuthority(AssigningAuthorityInfo assigningAuthorityInfo)
		{
			return this.Client.Post<AssigningAuthorityInfo, AssigningAuthorityInfo>("assigningAuthority", this.Client.Accept, assigningAuthorityInfo);
		}

		/// <summary>
		/// Create audit
		/// </summary>
		public void CreateAudit(AuditInfo audit)
		{
			this.Client.Post<AuditInfo, object>("audit", this.Client.Accept, audit);
		}

		/// <summary>
		/// Creates the code system.
		/// </summary>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the created code system.</returns>
		public CodeSystem CreateCodeSystem(CodeSystem codeSystem)
		{
			return this.Client.Post<CodeSystem, CodeSystem>("codeSystem", this.Client.Accept, codeSystem);
		}

		/// <summary>
		/// Creates a device in the IMS.
		/// </summary>
		/// <param name="device">The device to be created.</param>
		/// <returns>Returns the newly created device.</returns>
		public SecurityDeviceInfo CreateDevice(SecurityDeviceInfo device)
		{
			return this.Client.Post<SecurityDeviceInfo, SecurityDeviceInfo>("device", this.Client.Accept, device);
		}

		/// <summary>
		/// Creates the type of the extension.
		/// </summary>
		/// <param name="extensionType">Type of the extension.</param>
		/// <returns>Returns the created extension type.</returns>
		public ExtensionType CreateExtensionType(ExtensionType extensionType)
		{
			return this.Client.Post<ExtensionType, ExtensionType>("extensionType", this.Client.Accept, extensionType);
		}

		/// <summary>
		/// Creates a policy in the IMS.
		/// </summary>
		/// <param name="policy">The policy to be created.</param>
		/// <returns>Returns the newly created policy.</returns>
		public SecurityPolicyInfo CreatePolicy(SecurityPolicyInfo policy)
		{
			return this.Client.Post<SecurityPolicyInfo, SecurityPolicyInfo>("policy", this.Client.Accept, policy);
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

		/// <summary>
		/// Deletes an applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to be deleted.</param>
		/// <returns>Returns the deleted applet.</returns>
		public bool DeleteApplet(string appletId)
		{
			return this.Client.Delete<object>($"applet/{appletId}") != null;
		}

		/// <summary>
		/// Deletes an application.
		/// </summary>
		/// <param name="applicationId">The id of the application to be deleted.</param>
		/// <returns>Returns the deleted application.</returns>
		public SecurityApplicationInfo DeleteApplication(string applicationId)
		{
			return this.Client.Delete<SecurityApplicationInfo>($"application/{applicationId}");
		}

		/// <summary>
		/// Deletes an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to be deleted.</param>
		/// <returns>Returns the deleted assigning authority.</returns>
		public AssigningAuthorityInfo DeleteAssigningAuthority(string assigningAuthorityId)
		{
			return this.Client.Delete<AssigningAuthorityInfo>($"assigningAuthority/{assigningAuthorityId}");
		}

		/// <summary>
		/// Deletes a specified certificate.
		/// </summary>
		/// <param name="certificateId">The id of the certificate to be deleted.</param>
		/// <param name="reason">The reason the certificate is to be deleted.</param>
		/// <returns>Returns the deletion result.</returns>
		public SubmissionResult DeleteCertificate(string certificateId, RevokeReason reason)
		{
			return this.Client.Delete<SubmissionResult>($"certificate/{certificateId}/revokeReason/{reason}");
		}

		/// <summary>
		/// Deletes the code system.
		/// </summary>
		/// <param name="codeSystemId">The code system identifier.</param>
		/// <returns>Returns the deleted code system.</returns>
		public CodeSystem DeleteCodeSystem(string codeSystemId)
		{
			return this.Client.Delete<CodeSystem>($"codeSystem/{codeSystemId}");
		}

		/// <summary>
		/// Deletes a device.
		/// </summary>
		/// <param name="id">The id of the device to be deleted.</param>
		/// <returns>Returns the deleted device.</returns>
		public SecurityDeviceInfo DeleteDevice(string id)
		{
			return this.Client.Delete<SecurityDeviceInfo>($"device/{id}");
		}

		/// <summary>
		/// Deletes the type of the extension.
		/// </summary>
		/// <param name="extensionTypeId">The extension type identifier.</param>
		/// <returns>Returns the deleted extension type.</returns>
		public ExtensionType DeleteExtensionType(string extensionTypeId)
		{
			return this.Client.Delete<ExtensionType>($"extensionType/{extensionTypeId}");
		}

		/// <summary>
		/// Deletes a security policy.
		/// </summary>
		/// <param name="id">The id of the policy to be deleted.</param>
		/// <returns>Returns the deleted policy.</returns>
		public SecurityPolicyInfo DeletePolicy(string id)
		{
			return this.Client.Delete<SecurityPolicyInfo>($"policy/{id}");
		}

		/// <summary>
		/// Deletes a security role.
		/// </summary>
		/// <param name="id">The id of the role to be deleted.</param>
		/// <returns>Returns the deleted role.</returns>
		public SecurityRoleInfo DeleteRole(string id)
		{
			return this.Client.Delete<SecurityRoleInfo>($"role/{id}");
		}

		/// <summary>
		/// Deletes a security user.
		/// </summary>
		/// <param name="id">The id of the user to be deleted.</param>
		/// <returns>Returns the deleted user.</returns>
		public SecurityUserInfo DeleteUser(string id)
		{
			return this.Client.Delete<SecurityUserInfo>($"user/{id}");
		}

		/// <summary>
		/// Downloads the applet.
		/// </summary>
		/// <param name="appletId">The applet identifier.</param>
		/// <returns>Stream.</returns>
		public Stream DownloadApplet(string appletId)
		{
			return new MemoryStream(this.Client.Get($"applet/{appletId}/pak"));
		}

		/// <summary>
		/// Retrieves the specified role from the AMI
		/// </summary>
		public AmiCollection<SecurityPolicyInfo> FindPolicy(Expression<Func<SecurityPolicy, bool>> query)
		{
			return this.Client.Get<AmiCollection<SecurityPolicyInfo>>("policy", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Retrieves the specified role from the AMI
		/// </summary>
		public AmiCollection<SecurityRoleInfo> FindRole(Expression<Func<SecurityRole, bool>> query)
		{
			return this.Client.Get<AmiCollection<SecurityRoleInfo>>("role", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets a specific alert.
		/// </summary>
		/// <param name="id">The id of the alert to retrieve.</param>
		/// <returns>Returns the alert.</returns>
		public AlertMessageInfo GetAlert(string id)
		{
			return this.Client.Get<AlertMessageInfo>($"alert/{id}");
		}

        /// <summary>
		/// Gets a diagnostic alert.
		/// </summary>
		public DiagnosticReport GetServerDiagnoticReport()
        {
            return this.Client.Get<DiagnosticReport>($"sherlock");
        }

        /// <summary>
        /// Gets a list of alerts.
        /// </summary>
        /// <param name="query">The query expression to use to find the alerts.</param>
        /// <returns>Returns a collection of alerts which match the specified criteria.</returns>
        public AmiCollection<AlertMessageInfo> GetAlerts(Expression<Func<AlertMessage, bool>> query)
		{
			return this.Client.Get<AmiCollection<AlertMessageInfo>>("alert", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets a specific applet.
		/// </summary>
		/// <param name="id">The id of the applet to retrieve.</param>
		/// <returns>Returns the applet.</returns>
		public AppletManifestInfo GetApplet(string id)
		{
			return this.Client.Get<AppletManifestInfo>($"applet/{id}");
		}

		/// <summary>
		/// Gets a list of applets for a specific query.
		/// </summary>
		/// <returns>Returns a list of applet which match the specific query.</returns>
		public AmiCollection<AppletManifestInfo> GetApplets()
		{
			return this.Client.Get<AmiCollection<AppletManifestInfo>>("applet", new KeyValuePair<string, object>("_", DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss")));
		}

		/// <summary>
		/// Gets a specific application.
		/// </summary>
		/// <param name="id">The id of the application to retrieve.</param>
		/// <returns>Returns the application.</returns>
		public SecurityApplicationInfo GetApplication(string id)
		{
			return this.Client.Get<SecurityApplicationInfo>($"application/{id}");
		}

		/// <summary>
		/// Gets a list applications for a specific query.
		/// </summary>
		/// <returns>Returns a list of application which match the specific query.</returns>
		public AmiCollection<SecurityApplicationInfo> GetApplications(Expression<Func<SecurityApplicationInfo, bool>> query)
		{
			return this.Client.Get<AmiCollection<SecurityApplicationInfo>>("application", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets a list of assigning authorities.
		/// </summary>
		/// <param name="query">The query expression to use to find the assigning authorities.</param>
		/// <returns>Returns a collection of assigning authorities which match the specified criteria.</returns>
		public AmiCollection<AssigningAuthorityInfo> GetAssigningAuthorities(Expression<Func<AssigningAuthority, bool>> query)
		{
			return this.Client.Get<AmiCollection<AssigningAuthorityInfo>>("assigningAuthority", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets a list of certificates.
		/// </summary>
		/// <returns>Returns a collection of certificates which match the specified query.</returns>
		public AmiCollection<X509Certificate2Info> GetCertificates()
		{
			return this.Client.Get<AmiCollection<X509Certificate2Info>>("certificate", new KeyValuePair<string, object>("_", DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss")));
		}

		/// <summary>
		/// Gets a certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request to be retrieved.</param>
		/// <returns>Returns a certificate signing request.</returns>
		public SubmissionResult GetCertificateSigningRequest(string id)
		{
			return this.Client.Get<SubmissionResult>($"csr/{id}");
		}

		/// <summary>
		/// Gets a list of certificate signing requests.
		/// </summary>
		/// <param name="query">The query expression to use to find the certificate signing requests.</param>
		/// <returns>Returns a collection of certificate signing requests which match the specified query.</returns>
		public AmiCollection<SubmissionInfo> GetCertificateSigningRequests(Expression<Func<SubmissionInfo, bool>> query)
		{
			return this.Client.Get<AmiCollection<SubmissionInfo>>("csr", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets the code system.
		/// </summary>
		/// <param name="codeSystemId">The code system identifier.</param>
		/// <returns>Returns a code system.</returns>
		public CodeSystem GetCodeSystem(string codeSystemId)
		{
			return this.Client.Get<CodeSystem>($"codeSystem/{codeSystemId}");
		}

		/// <summary>
		/// Gets the code systems.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Returns a list of code systems.</returns>
		public AmiCollection<CodeSystem> GetCodeSystems(Expression<Func<CodeSystem, bool>> query)
		{
			return this.Client.Get<AmiCollection<CodeSystem>>("codeSystem", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets a specific device.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the security device.</returns>
		public SecurityDeviceInfo GetDevice(string id)
		{
			return this.Client.Get<SecurityDeviceInfo>($"device/{id}");
		}

		/// <summary>
		/// Gets a list of devices.
		/// </summary>
		/// <param name="query">The query expression to use to find the devices.</param>
		/// <returns>Returns a collection of devices which match the specified query.</returns>
		public AmiCollection<SecurityDeviceInfo> GetDevices(Expression<Func<SecurityDeviceInfo, bool>> query)
		{
			return this.Client.Get<AmiCollection<SecurityDeviceInfo>>("device", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets the type of the extension.
		/// </summary>
		/// <param name="extensionTypeId">The extension type identifier.</param>
		/// <returns>Returns the extension type, or null if no extension type is found.</returns>
		public ExtensionType GetExtensionType(string extensionTypeId)
		{
			return this.Client.Get<ExtensionType>($"extensionType/{extensionTypeId}");
		}

		/// <summary>
		/// Gets the extension types.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of extension types.</returns>
		public AmiCollection<ExtensionType> GetExtensionTypes(Expression<Func<ExtensionType, bool>> expression)
		{
			return this.Client.Get<AmiCollection<ExtensionType>>("extensionType", QueryExpressionBuilder.BuildQuery(expression).ToArray());
		}

		/// <summary>
		/// Retrieves a specified policy.
		/// </summary>
		/// <param name="query">The query expression to use to find the policy.</param>
		/// <returns>Returns a collection of policies which match the specified query parameters.</returns>
		public AmiCollection<SecurityPolicyInfo> GetPolicies(Expression<Func<SecurityPolicy, bool>> query)
		{
			return this.Client.Get<AmiCollection<SecurityPolicyInfo>>("policy", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets a specific policy.
		/// </summary>
		/// <param name="id">The id of the policy to be retrieved.</param>
		/// <returns>Returns the specific policy.</returns>
		public SecurityPolicyInfo GetPolicy(string id)
		{
			return this.Client.Get<SecurityPolicyInfo>($"policy/{id}");
		}

		/// <summary>
		/// Gets a specific role.
		/// </summary>
		/// <param name="id">The id of the role to be retrieved.</param>
		/// <returns>Returns the specified role.</returns>
		public SecurityRoleInfo GetRole(string id)
		{
			return this.Client.Get<SecurityRoleInfo>($"role/{id}");
		}

		/// <summary>
		/// Retrieves a specified role.
		/// </summary>
		/// <param name="query">The query expression to use to find the role.</param>
		/// <returns>Returns a collection of roles which match the specified query parameters.</returns>
		public AmiCollection<SecurityRoleInfo> GetRoles(Expression<Func<SecurityRole, bool>> query)
		{
			return this.Client.Get<AmiCollection<SecurityRoleInfo>>("role", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets a list of two-factor mechanisms
		/// </summary>
		public AmiCollection<TfaMechanismInfo> GetTwoFactorMechanisms()
		{
			return this.Client.Get<AmiCollection<TfaMechanismInfo>>("tfa", null);
		}

		/// <summary>
		/// Gets a specific user.
		/// </summary>
		/// <param name="id">The id of the user to be retrieved.</param>
		/// <returns>Returns the specified user.</returns>
		public SecurityUserInfo GetUser(string id)
		{
			return this.Client.Get<SecurityUserInfo>($"user/{id}");
		}

		/// <summary>
		/// Retrieves a specified user.
		/// </summary>
		/// <param name="query">The query expression to use to find the user.</param>
		/// <returns>Returns a collection of users which match the specified query parameters.</returns>
		public AmiCollection<SecurityUserInfo> GetUsers(Expression<Func<SecurityUser, bool>> query)
		{
			return this.Client.Get<AmiCollection<SecurityUserInfo>>("user", QueryExpressionBuilder.BuildQuery(query).ToArray());
		}

		/// <summary>
		/// Gets the options for the AMI.
		/// </summary>
		/// <returns>Return the service options for the AMI.</returns>
		public ServiceOptions Options()
		{
			return this.Client.Options<ServiceOptions>("/");
		}

		/// <summary>
		/// Perform a ping
		/// </summary>
		public bool Ping()
		{
			try
			{
				this.Client.Invoke<Object, Object>("PING", "/", null, null);
				return true;
			}
			catch
			{
				return false;
			}
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

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
		// ~AmiServiceClient() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		#endregion IDisposable Support

		/// <summary>
		/// Create security password reset request.
		/// </summary>
		/// <param name="resetInfo">The reset information.</param>
		public void SendTfaSecret(TfaRequestInfo resetInfo)
		{
			this.Client.Post<TfaRequestInfo, object>("tfa", this.Client.Accept, resetInfo);
		}

		/// <summary>
		/// Stats the update via a HEAD command
		/// </summary>
		/// <param name="packageId">The package identifier.</param>
		/// <returns>Returns the applet info.</returns>
		public AppletInfo StatUpdate(String packageId)
		{
			var headers = this.Client.Head($"applet/{packageId}");
			String versionKey = String.Empty,
				packId = String.Empty,
				hash = String.Empty;
			headers.TryGetValue("X-OpenIZ-PakID", out packId);
			headers.TryGetValue("ETag", out versionKey);

			return new AppletInfo()
			{
				Id = packageId,
				Version = versionKey
			};
		}

		/// <summary>
		/// Submits a diagnostic report.
		/// </summary>
		/// <param name="report">The diagnostic report.</param>
		/// <returns>Returns the submitted diagnostic report.</returns>
		public void SubmitAudit(AuditInfo report)
		{
			this.Client.Post<AuditInfo, object>("audit", this.Client.Accept, report);
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
		/// Submits a diagnostic report.
		/// </summary>
		/// <param name="report">The diagnostic report.</param>
		/// <returns>Returns the submitted diagnostic report.</returns>
		public DiagnosticReport SubmitDiagnosticReport(DiagnosticReport report)
		{
			return this.Client.Post<DiagnosticReport, DiagnosticReport>("sherlock", this.Client.Accept, report);
		}

		/// <summary>
		/// Updates an alert.
		/// </summary>
		/// <param name="alertId">The id of the alert to be updated.</param>
		/// <param name="alertMessageInfo">The alert message info containing the updated information.</param>
		/// <returns>Returns the updated alert.</returns>
		public AlertMessageInfo UpdateAlert(string alertId, AlertMessageInfo alertMessageInfo)
		{
			return this.Client.Put<AlertMessageInfo, AlertMessageInfo>($"alert/{alertId}", this.Client.Accept, alertMessageInfo);
		}

		/// <summary>
		/// Updates an applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to be updated.</param>
		/// <param name="appletPackage">The applet containing the updated information.</param>
		/// <returns>Returns the updated applet.</returns>
		public AppletManifestInfo UpdateApplet(string appletId, AppletPackage appletPackage)
		{
			using (var ms = new MemoryStream())
			{
				appletPackage.Save(ms);
				ms.Flush();
				return this.Client.Put<byte[], AppletManifestInfo>($"applet/{appletId}", "application/octet-stream", ms.ToArray());
			}
		}

		/// <summary>
		/// Updates an application.
		/// </summary>
		/// <param name="applicationId">The id of the application to be updated.</param>
		/// <param name="applicationInfo">The application containing the updated information.</param>
		/// <returns>Returns the updated application.</returns>
		public SecurityApplicationInfo UpdateApplication(string applicationId, SecurityApplicationInfo applicationInfo)
		{
			return this.Client.Put<SecurityApplicationInfo, SecurityApplicationInfo>($"application/{applicationId}", this.Client.Accept, applicationInfo);
		}

		/// <summary>
		/// Updates an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to be updated.</param>
		/// <param name="assigningAuthorityInfo">The assigning authority info containing the updated information.</param>
		/// <returns>Returns the updated assigning authority.</returns>
		public AssigningAuthorityInfo UpdateAssigningAuthority(string assigningAuthorityId, AssigningAuthorityInfo assigningAuthorityInfo)
		{
			return this.Client.Put<AssigningAuthorityInfo, AssigningAuthorityInfo>($"assigningAuthority/{assigningAuthorityId}", this.Client.Accept, assigningAuthorityInfo);
		}

		/// <summary>
		/// Updates the code system.
		/// </summary>
		/// <param name="codeSystemId">The code system identifier.</param>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Return the updated code system.</returns>
		public CodeSystem UpdateCodeSystem(string codeSystemId, CodeSystem codeSystem)
		{
			return this.Client.Put<CodeSystem, CodeSystem>($"codeSystem/{codeSystemId}", this.Client.Accept, codeSystem);
		}

		/// <summary>
		/// Updates a device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be updated.</param>
		/// <param name="deviceInfo">The device containing the updated information.</param>
		/// <returns>Returns the updated device.</returns>
		public SecurityDeviceInfo UpdateDevice(string deviceId, SecurityDeviceInfo deviceInfo)
		{
			return this.Client.Put<SecurityDeviceInfo, SecurityDeviceInfo>($"device/{deviceId}", this.Client.Accept, deviceInfo);
		}

		/// <summary>
		/// Updates the type of the extension.
		/// </summary>
		/// <param name="extensionTypeId">The extension type identifier.</param>
		/// <param name="extensionType">Type of the extension.</param>
		/// <returns>Returns the updated extension type.</returns>
		public ExtensionType UpdateExtensionType(string extensionTypeId, ExtensionType extensionType)
		{
			return this.Client.Put<ExtensionType, ExtensionType>($"extensionType/{extensionTypeId}", this.Client.Accept, extensionType);
		}

		/// <summary>
		/// Updates a role.
		/// </summary>
		/// <param name="roleId">The id of the role to be updated.</param>
		/// <param name="roleInfo">The role containing the updated information.</param>
		/// <returns>Returns the updated role.</returns>
		public SecurityRoleInfo UpdateRole(string roleId, SecurityRoleInfo roleInfo)
		{
			return this.Client.Put<SecurityRoleInfo, SecurityRoleInfo>($"role/{roleId}", this.Client.Accept, roleInfo);
		}

		/// <summary>
		/// Updates a user.
		/// </summary>
		/// <param name="id">The id of the user to be updated.</param>
		/// <param name="user">The user containing the updated information.</param>
		/// <returns>Returns the updated user.</returns>
		public SecurityUserInfo UpdateUser(Guid id, SecurityUserInfo user)
		{
			return this.Client.Put<SecurityUserInfo, SecurityUserInfo>($"user/{id}", this.Client.Accept, user);
		}
	}
}