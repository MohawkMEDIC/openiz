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

using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Interop;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.AMI.Alerting;
using OpenIZ.Core.Model.AMI.Applet;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.BusinessRules;
using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZ.Core.Model.AMI.Diagnostics;
using OpenIZ.Core.Model.AMI.Logging;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using SwaggerWcf.Attributes;
using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Schema;

namespace OpenIZ.Messaging.AMI.Wcf
{
	/// <summary>
	/// Represents the administrative contract interface.
	/// </summary>
	[ServiceContract(ConfigurationName = "AMI_1.0", Name = "AMI"), XmlSerializerFormat]
	[ServiceKnownType(typeof(Entity))]
	[ServiceKnownType(typeof(ExtensionType))]
	[ServiceKnownType(typeof(AlertMessage))]
	[ServiceKnownType(typeof(AlertMessageInfo))]
	[ServiceKnownType(typeof(SecurityApplication))]
	[ServiceKnownType(typeof(SecurityApplicationInfo))]
	[ServiceKnownType(typeof(TfaRequestInfo))]
	[ServiceKnownType(typeof(SecurityDevice))]
	[ServiceKnownType(typeof(SecurityDeviceInfo))]
	[ServiceKnownType(typeof(SecurityPolicy))]
	[ServiceKnownType(typeof(SecurityPolicyInfo))]
	[ServiceKnownType(typeof(SecurityRole))]
	[ServiceKnownType(typeof(SecurityRoleInfo))]
	[ServiceKnownType(typeof(SecurityUser))]
	[ServiceKnownType(typeof(AuditInfo))]
	[ServiceKnownType(typeof(SecurityUserInfo))]
	[ServiceKnownType(typeof(AppletManifest))]
	[ServiceKnownType(typeof(AppletManifestInfo))]
	[ServiceKnownType(typeof(DeviceEntity))]
	[ServiceKnownType(typeof(DiagnosticApplicationInfo))]
	[ServiceKnownType(typeof(DiagnosticAttachmentInfo))]
	[ServiceKnownType(typeof(DiagnosticBinaryAttachment))]
	[ServiceKnownType(typeof(DiagnosticTextAttachment))]
	[ServiceKnownType(typeof(DiagnosticEnvironmentInfo))]
	[ServiceKnownType(typeof(DiagnosticReport))]
	[ServiceKnownType(typeof(DiagnosticSyncInfo))]
	[ServiceKnownType(typeof(DiagnosticVersionInfo))]
	[ServiceKnownType(typeof(SubmissionInfo))]
	[ServiceKnownType(typeof(SubmissionResult))]
	[ServiceKnownType(typeof(ApplicationEntity))]
	[ServiceKnownType(typeof(SubmissionRequest))]
	[ServiceKnownType(typeof(ServiceOptions))]
	[ServiceKnownType(typeof(X509Certificate2Info))]
	[ServiceKnownType(typeof(AssigningAuthorityInfo))]
	[ServiceKnownType(typeof(CodeSystem))]
	[ServiceKnownType(typeof(LogFileInfo))]
	[ServiceKnownType(typeof(AmiCollection<SubmissionInfo>))]
	[ServiceKnownType(typeof(AmiCollection<ExtensionType>))]
	[ServiceKnownType(typeof(AmiCollection<AppletManifestInfo>))]
	[ServiceKnownType(typeof(AmiCollection<SecurityApplicationInfo>))]
	[ServiceKnownType(typeof(AmiCollection<SecurityDeviceInfo>))]
	[ServiceKnownType(typeof(AmiCollection<SecurityRoleInfo>))]
	[ServiceKnownType(typeof(AmiCollection<SecurityPolicyInfo>))]
	[ServiceKnownType(typeof(AmiCollection<TfaMechanismInfo>))]
	[ServiceKnownType(typeof(AmiCollection<TfaRequestInfo>))]
	[ServiceKnownType(typeof(AmiCollection<BusinessRuleInfo>))]
	[ServiceKnownType(typeof(AmiCollection<AssigningAuthorityInfo>))]
	[ServiceKnownType(typeof(AmiCollection<SecurityDevice>))]
	[ServiceKnownType(typeof(AmiCollection<AlertMessageInfo>))]
	[ServiceKnownType(typeof(AmiCollection<SecurityUserInfo>))]
	[ServiceKnownType(typeof(AmiCollection<LogFileInfo>))]
	[ServiceKnownType(typeof(AmiCollection<CodeSystem>))]
	[ServiceKnownType(typeof(AmiCollection<X509Certificate2Info>))]
	public interface IAmiContract
	{
		/// <summary>
		/// Accepts a certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request to be accepted.</param>
		/// <returns>Returns the acceptance result.</returns>
		[WebInvoke(UriTemplate = "/csr/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		[SwaggerWcfPath("Accepts CSR", "Accepts a Certificate Signing Request")]
		SubmissionResult AcceptCsr(string id);

		/// <summary>
		/// Changes the password of a user.
		/// </summary>
		/// <param name="id">The id of the user whose password is to be changed.</param>
		/// <param name="password">The new password of the user.</param>
		/// <returns>Returns the updated user.</returns>
		[WebInvoke(UriTemplate = "/changepassword/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		[Obsolete("Use UpdateUser instead as this RPC style call may be removed in the future")]
		SecurityUser ChangePassword(String id, string password);

		/// <summary>
		/// Creates an alert.
		/// </summary>
		/// <param name="alertMessageInfo">The alert message to be created.</param>
		/// <returns>Returns the created alert.</returns>
		[WebInvoke(UriTemplate = "/alert", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		[SwaggerWcfPath("Create Alert", "Creates an alert in the system. This alert can be sent to mutiple users or all users")]
		AlertMessageInfo CreateAlert(AlertMessageInfo alertMessageInfo);

		/// <summary>
		/// Creates an applet.
		/// </summary>
		/// <param name="pakData">The pak data.</param>
		/// <returns>Returns the created applet manifest info.</returns>
		[WebInvoke(UriTemplate = "/applet", Method = "POST")]
		[SwaggerWcfPath("Create Applet", "Creates an applet. An applet represents a html user interface, which can include optional business rules")]
		AppletManifestInfo CreateApplet(Stream pakData);

		/// <summary>
		/// Creates a security application.
		/// </summary>
		/// <param name="applicationInfo">The security application to be created.</param>
		/// <returns>Returns the created security application.</returns>
		[WebInvoke(UriTemplate = "/application", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		[SwaggerWcfPath("Create Application", "Creates an application. An application represents a entity which can be a third party application or service")]
		SecurityApplicationInfo CreateApplication(SecurityApplicationInfo applicationInfo);

		/// <summary>
		/// Creates an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityInfo">The assigning authority to be created.</param>
		/// <returns>Returns the created assigning authority.</returns>
		[WebInvoke(UriTemplate = "/assigningAuthority", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		[SwaggerWcfPath("Create Assigning Authority", "Creates an assigning authority. An assigning authority represents an authority which can be used to assign identifiers to entities")]
		AssigningAuthorityInfo CreateAssigningAuthority(AssigningAuthorityInfo assigningAuthorityInfo);

		/// <summary>
		/// Creates the code system.
		/// </summary>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the created code system.</returns>
		[WebInvoke(UriTemplate = "/codeSystem", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		[SwaggerWcfPath("Create Code System", "Creates a code system. A code system represents a system or collection of concept representations")]
		CodeSystem CreateCodeSystem(CodeSystem codeSystem);

		/// <summary>
		/// Creates a device in the IMS.
		/// </summary>
		/// <param name="deviceInfo">The device to be created.</param>
		/// <returns>Returns the newly created device.</returns>
		[WebInvoke(UriTemplate = "/device", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		[SwaggerWcfPath("Create Device", "Creates a device. A device represents a physical device such as a tablet or a desktop computer")]
		SecurityDeviceInfo CreateDevice(SecurityDeviceInfo deviceInfo);

		/// <summary>
		/// Creates a diagnostic report.
		/// </summary>
		/// <param name="report">The diagnostic report to be created.</param>
		/// <returns>Returns the created diagnostic report.</returns>
		[WebInvoke(UriTemplate = "/sherlock", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		[SwaggerWcfPath("Create Diagnostic Report", "Creates a diagnostic report. A diagnostic report contains logs and configuration information used to debug and resolve issues")]
		DiagnosticReport CreateDiagnosticReport(DiagnosticReport report);

		/// <summary>
		/// Creates the type of the extension.
		/// </summary>
		/// <param name="extensionType">Type of the extension.</param>
		/// <returns>Returns the created extension type.</returns>
		[WebInvoke(UriTemplate = "/extensionType", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		[SwaggerWcfPath("Create Extension Type", "Creates an extension type. An extension type represents a handler which instructs the system how to handle different extensions which are attached to a(n) act(s) or an entity")]
		ExtensionType CreateExtensionType(ExtensionType extensionType);

		/// <summary>
		/// Creates a security policy.
		/// </summary>
		/// <param name="policy">The security policy to be created.</param>
		/// <returns>Returns the newly created security policy.</returns>
		[WebInvoke(UriTemplate = "/policy", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		[SwaggerWcfPath("Create Policy", "Creates a policy. A policy represents a structured governance of data and information")]
		SecurityPolicyInfo CreatePolicy(SecurityPolicyInfo policy);

		/// <summary>
		/// Creates a security role.
		/// </summary>
		/// <param name="role">The security role to be created.</param>
		/// <returns>Returns the newly created security role.</returns>
		[WebInvoke(UriTemplate = "/role", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		[SwaggerWcfPath("Create Role", "Creates a role. A role represents a group or category to which a user belongs")]
		SecurityRoleInfo CreateRole(SecurityRoleInfo role);

		/// <summary>
		/// Creates a security user.
		/// </summary>
		/// <param name="user">The security user to be created.</param>
		/// <returns>Returns the newly created security user.</returns>
		[WebInvoke(UriTemplate = "/user", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		[SwaggerWcfPath("Create User", "Creates a user. A user represents a human user, application user or system user")]
		SecurityUserInfo CreateUser(SecurityUserInfo user);

		/// <summary>
		/// Deletes an applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to be deleted.</param>
		/// <returns>Returns the deleted applet.</returns>
		[WebInvoke(UriTemplate = "/applet/{appletId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		[SwaggerWcfPath("Delete Applet", "Deletes an applet")]
		void DeleteApplet(string appletId);

		/// <summary>
		/// Deletes an application.
		/// </summary>
		/// <param name="applicationId">The id of the application to be deleted.</param>
		/// <returns>Returns the deleted application.</returns>
		[WebInvoke(UriTemplate = "/application/{applicationId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		[SwaggerWcfPath("Delete Application", "Deletes an application")]
		SecurityApplicationInfo DeleteApplication(string applicationId);

		/// <summary>
		/// Deletes an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to be deleted.</param>
		/// <returns>Returns the deleted assigning authority.</returns>
		[WebInvoke(UriTemplate = "/assigningAuthority/{assigningAuthorityId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		[SwaggerWcfPath("Delete Assignging Authority", "Deletes an assigning authority")]
		AssigningAuthorityInfo DeleteAssigningAuthority(string assigningAuthorityId);

		/// <summary>
		/// Deletes a specified certificate.
		/// </summary>
		/// <param name="id">The id of the certificate to be deleted.</param>
		/// <param name="reason">The reason the certificate is to be deleted.</param>
		/// <returns>Returns the deletion result.</returns>
		[WebInvoke(UriTemplate = "/certificate/{id}/revokeReason/{reason}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		[SwaggerWcfPath("Delete Certificate", "Deletes a certificate")]
		SubmissionResult DeleteCertificate(string id, string reason);

		/// <summary>
		/// Deletes the code system.
		/// </summary>
		/// <param name="codeSystemId">The code system identifier.</param>
		/// <returns>Returns the deleted code system.</returns>
		[WebInvoke(UriTemplate = "/codeSystem/{codeSystemId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		[SwaggerWcfPath("Delete Code System", "Deletes a code system")]
		CodeSystem DeleteCodeSystem(string codeSystemId);

		/// <summary>
		/// Deletes a device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be deleted.</param>
		/// <returns>Returns the deleted device.</returns>
		[WebInvoke(UriTemplate = "/device/{deviceId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		[SwaggerWcfPath("Delete Device", "Deletes a device")]
		SecurityDeviceInfo DeleteDevice(string deviceId);

		/// <summary>
		/// Deletes the type of the extension.
		/// </summary>
		/// <param name="extensionTypeId">The extension type identifier.</param>
		/// <returns>Returns the deleted extension type.</returns>
		[WebInvoke(UriTemplate = "/extensionType/{extensionTypeId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		[SwaggerWcfPath("Delete Extension Type", "Deletes an extension type")]
		ExtensionType DeleteExtensionType(string extensionTypeId);

		/// <summary>
		/// Deletes a security policy.
		/// </summary>
		/// <param name="policyId">The id of the policy to be deleted.</param>
		/// <returns>Returns the deleted policy.</returns>
		[WebInvoke(UriTemplate = "/policy/{policyId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		[SwaggerWcfPath("Delete Policy", "Deletes a policy")]
		SecurityPolicyInfo DeletePolicy(string policyId);

		/// <summary>
		/// Deletes a security role.
		/// </summary>
		/// <param name="roleId">The id of the role to be deleted.</param>
		/// <returns>Returns the deleted role.</returns>
		[WebInvoke(UriTemplate = "/role/{roleId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		[SwaggerWcfPath("Delete Role", "Deletes a role")]
		SecurityRoleInfo DeleteRole(string roleId);

		/// <summary>
		/// Deletes a security user.
		/// </summary>
		/// <param name="userId">The id of the user to be deleted.</param>
		/// <returns>Returns the deleted user.</returns>
		[WebInvoke(UriTemplate = "/user/{userId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		[SwaggerWcfPath("Delete User", "Deletes a user")]
		SecurityUserInfo DeleteUser(string userId);

		/// <summary>
		/// Downloads the applet.
		/// </summary>
		/// <param name="appletId">The applet identifier.</param>
		/// <returns>Stream.</returns>
		[WebGet(UriTemplate = "/applet/{appletId}/pak")]
		[SwaggerWcfPath("Download Applet", "Downloads an applet")]
		Stream DownloadApplet(string appletId);

		/// <summary>
		/// Gets a specific alert.
		/// </summary>
		/// <param name="alertId">The id of the alert to retrieve.</param>
		/// <returns>Returns the alert.</returns>
		[WebGet(UriTemplate = "/alert/{alertId}", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Alert", "Retrieves an alert by id")]
		AlertMessageInfo GetAlert(string alertId);

		/// <summary>
		/// Gets a list of alert for a specific query.
		/// </summary>
		/// <returns>Returns a list of alert which match the specific query.</returns>
		[WebGet(UriTemplate = "/alert", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Alerts", "Retrieves alerts based on a query")]
		AmiCollection<AlertMessageInfo> GetAlerts();

		/// <summary>
		/// Gets a specific applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to retrieve.</param>
		/// <returns>Returns the applet.</returns>
		[WebGet(UriTemplate = "/applet/{appletId}", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Applet", "Retrieves an applet by id")]
		AppletManifestInfo GetApplet(string appletId);

		/// <summary>
		/// Gets a list of applets for a specific query.
		/// </summary>
		/// <returns>Returns a list of applet which match the specific query.</returns>
		[WebGet(UriTemplate = "/applet", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Applet", "Retrieves applets based on a query")]
		AmiCollection<AppletManifestInfo> GetApplets();

		/// <summary>
		/// Gets a specific application.
		/// </summary>
		/// <param name="applicationId">The id of the application to retrieve.</param>
		/// <returns>Returns the application.</returns>
		[WebGet(UriTemplate = "/application/{applicationId}", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Application", "Retrieve an application by id")]
		SecurityApplicationInfo GetApplication(string applicationId);

		/// <summary>
		/// Gets a list applications for a specific query.
		/// </summary>
		/// <returns>Returns a list of application which match the specific query.</returns>
		[WebGet(UriTemplate = "/application", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Applications", "Retrieves applications based on a query")]
		AmiCollection<SecurityApplicationInfo> GetApplications();

		/// <summary>
		/// Gets a list of assigning authorities for a specific query.
		/// </summary>
		/// <returns>Returns a list of assigning authorities which match the specific query.</returns>
		[WebGet(UriTemplate = "/assigningAuthority", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Assigning Authorities", "Retrieves assigning authorities based on a query")]
		AmiCollection<AssigningAuthorityInfo> GetAssigningAuthorities();

		/// <summary>
		/// Gets a specific assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to retrieve.</param>
		/// <returns>Returns the assigning authority.</returns>
		[WebGet(UriTemplate = "/assigningAuthority/{assigningAuthorityId}", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Assigning Authority", "Retrieves an assigning authority by id")]
		AssigningAuthorityInfo GetAssigningAuthority(string assigningAuthorityId);

		/// <summary>
		/// Gets a specific certificate.
		/// </summary>
		/// <param name="id">The id of the certificate to retrieve.</param>
		/// <returns>Returns the certificate.</returns>
		[WebGet(UriTemplate = "/certificate/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Certificate", "Retrieve a certificate by id")]
		byte[] GetCertificate(string id);

		/// <summary>
		/// Gets a list of certificates.
		/// </summary>
		/// <returns>Returns a list of certificates.</returns>
		[WebGet(UriTemplate = "/certificate", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Certificates", "Retrieves certificates based on a query")]
		AmiCollection<X509Certificate2Info> GetCertificates();

		/// <summary>
		/// Gets the code system.
		/// </summary>
		/// <param name="codeSystemId">The code system identifier.</param>
		/// <returns>Returns a code system.</returns>
		[WebGet(UriTemplate = "/codeSystem/{codeSystemId}", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Code System", "Retrieve a code system by id")]
		CodeSystem GetCodeSystem(string codeSystemId);

		/// <summary>
		/// Gets the code systems.
		/// </summary>
		/// <returns>Returns a list of code systems.</returns>
		[WebGet(UriTemplate = "/codeSystem", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Code Systems", "Retrieves code systems based on a query")]
		AmiCollection<CodeSystem> GetCodeSystems();

		/// <summary>
		/// Gets the certificate revocation list.
		/// </summary>
		/// <returns>Returns the certificate revocation list.</returns>
		[WebGet(UriTemplate = "/crl", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Certificate Revocation List", "Retrieves the certificate revocation list")]
		byte[] GetCrl();

		/// <summary>
		/// Gets a specific certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request to be retrieved.</param>
		/// <returns>Returns the certificate signing request.</returns>
		[WebGet(UriTemplate = "/csr/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get CSR", "Retrieves a certificate signing request by id")]
		SubmissionResult GetCsr(string id);

		/// <summary>
		/// Gets a list of submitted certificate signing requests.
		/// </summary>
		/// <returns>Returns a list of certificate signing requests.</returns>
		[WebGet(UriTemplate = "/csr", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get CSRs", "Retrieves certificate signing requests based on a query")]
		AmiCollection<SubmissionInfo> GetCsrs();

		/// <summary>
		/// Gets a specific device.
		/// </summary>
		/// <param name="deviceId">The id of the security device to be retrieved.</param>
		/// <returns>Returns the security device.</returns>
		[WebGet(UriTemplate = "/device/{deviceId}", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Device", "Retrieves a device by id")]
		SecurityDeviceInfo GetDevice(string deviceId);

		/// <summary>
		/// Gets a list of devices.
		/// </summary>
		/// <returns>Returns a list of devices.</returns>
		[WebGet(UriTemplate = "/device", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Device", "Retrieves devices based on a query")]
		AmiCollection<SecurityDeviceInfo> GetDevices();

		/// <summary>
		/// Gets the type of the extension.
		/// </summary>
		/// <param name="extensionTypeId">The extension type identifier.</param>
		/// <returns>Returns the extension type, or null if no extension type is found.</returns>
		[WebGet(UriTemplate = "/extensionType/{extensionTypeId}", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Extension Type", "Retrieves an extension type by id")]
		ExtensionType GetExtensionType(string extensionTypeId);

		/// <summary>
		/// Gets the extension types.
		/// </summary>
		/// <returns>Returns a list of extension types.</returns>
		[WebGet(UriTemplate = "/extensionType", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Extension Type", "Retrieves extension types based on a query")]
		AmiCollection<ExtensionType> GetExtensionTypes();

		/// <summary>
		/// Gets a list of policies.
		/// </summary>
		/// <returns>Returns a list of policies.</returns>
		[WebGet(UriTemplate = "/policy", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Policy", "Retrieves policies based on a query")]
		AmiCollection<SecurityPolicyInfo> GetPolicies();

		/// <summary>
		/// Gets a specific security policy.
		/// </summary>
		/// <param name="policyId">The id of the security policy to be retrieved.</param>
		/// <returns>Returns the security policy.</returns>
		[WebGet(UriTemplate = "/policy/{policyId}", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Policy", "Retrieves a policy by id")]
		SecurityPolicyInfo GetPolicy(string policyId);

		/// <summary>
		/// Gets a specific security role.
		/// </summary>
		/// <param name="roleId">The id of the security role to be retrieved.</param>
		/// <returns>Returns the security role.</returns>
		[WebGet(UriTemplate = "/role/{roleId}", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Role", "Retrieves a role by id")]
		SecurityRoleInfo GetRole(string roleId);

		/// <summary>
		/// Gets a list of security roles.
		/// </summary>
		/// <returns>Returns a list of security roles.</returns>
		[WebGet(UriTemplate = "/role", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Role", "Retrieves roles based on a query")]
		AmiCollection<SecurityRoleInfo> GetRoles();

		/// <summary>
		/// Gets the schema for the administrative interface.
		/// </summary>
		/// <param name="schemaId">The id of the schema to be retrieved.</param>
		/// <returns>Returns the administrative interface schema.</returns>
		[WebGet(UriTemplate = "/?xsd={schemaId}")]
		[SwaggerWcfPath("Get Server Schema", "Gets a complete schema of the AMI objects supported by this interface")]
		XmlSchema GetSchema(int schemaId);

		/// <summary>
		/// Gets a server diagnostic report.
		/// </summary>
		/// <returns>Returns the created diagnostic report.</returns>
		[WebGet(UriTemplate = "/sherlock", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get Diagnostic Report", "A diagnostic report contains logs and configuration information used to debug and resolve issues")]
		DiagnosticReport GetServerDiagnosticReport();

		/// <summary>
		/// Gets the list of TFA mechanisms.
		/// </summary>
		/// <returns>Returns a list of TFA mechanisms.</returns>
		[WebGet(UriTemplate = "/tfa")]
		[SwaggerWcfPath("Get TFA Mechanism", "Retrieves a list of supported TFA mechanisms")]
		AmiCollection<TfaMechanismInfo> GetTfaMechanisms();

		/// <summary>
		/// Gets a specific security user.
		/// </summary>
		/// <param name="userId">The id of the security user to be retrieved.</param>
		/// <returns>Returns the security user.</returns>
		[WebGet(UriTemplate = "/user/{userId}", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get User", "Retrieves a user by id")]
		SecurityUserInfo GetUser(string userId);

		/// <summary>
		/// Gets a list of security users.
		/// </summary>
		/// <returns>Returns a list of security users.</returns>
		[WebGet(UriTemplate = "/user", BodyStyle = WebMessageBodyStyle.Bare)]
		[SwaggerWcfPath("Get User", "Retrieves users based on a query")]
		AmiCollection<SecurityUserInfo> GetUsers();

		/// <summary>
		/// Return just the headers of the applet id
		/// </summary>
		[WebInvoke(Method = "HEAD", UriTemplate = "/applet/{appletId}")]
		void HeadApplet(string appletId);

		/// <summary>
		/// Gets options for the AMI service.
		/// </summary>
		/// <returns>Returns options for the AMI service.</returns>
		[WebInvoke(UriTemplate = "/", Method = "OPTIONS", BodyStyle = WebMessageBodyStyle.Bare)]
		IdentifiedData Options();

		/// <summary>
		/// Ping the service to determine up/down
		/// </summary>
		[WebInvoke(UriTemplate = "/", Method = "PING")]
		void Ping();

		/// <summary>
		/// Rejects a specified certificate signing request.
		/// </summary>
		/// <param name="certId">The id of the certificate signing request to be rejected.</param>
		/// <param name="reason">The reason the certificate signing request is to be rejected.</param>
		/// <returns>Returns the rejection result.</returns>
		[WebInvoke(UriTemplate = "/csr/{certId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		SubmissionResult RejectCsr(string certId, RevokeReason reason);

		/// <summary>
		/// Creates a request that the server issue a reset code
		/// </summary>
		[WebInvoke(UriTemplate = "/tfa", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		void SendTfaSecret(TfaRequestInfo resetInfo);

		/// <summary>
		/// Submits a specific certificate signing request.
		/// </summary>
		/// <param name="s">The certificate signing request.</param>
		/// <returns>Returns the submission result.</returns>
		[WebInvoke(UriTemplate = "/csr", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		SubmissionResult SubmitCsr(SubmissionRequest s);

		/// <summary>
		/// Updates an alert.
		/// </summary>
		/// <param name="alertId">The id of the alert to be updated.</param>
		/// <param name="alert">The alert containing the updated information.</param>
		/// <returns>Returns the updated alert.</returns>
		[WebInvoke(UriTemplate = "/alert/{alertId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		AlertMessageInfo UpdateAlert(string alertId, AlertMessageInfo alert);

		/// <summary>
		/// Updates an applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to be updated.</param>
		/// <param name="appletData">The applet data.</param>
		/// <returns>Returns the updated applet.</returns>
		[WebInvoke(UriTemplate = "/applet/{appletId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		AppletManifestInfo UpdateApplet(string appletId, Stream appletData);  

		/// <summary>
		/// Updates an application.
		/// </summary>
		/// <param name="applicationId">The id of the application to be updated.</param>
		/// <param name="applicationInfo">The application containing the updated information.</param>
		/// <returns>Returns the updated application.</returns>
		[WebInvoke(UriTemplate = "/application/{applicationId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		SecurityApplicationInfo UpdateApplication(string applicationId, SecurityApplicationInfo applicationInfo);

		/// <summary>
		/// Updates an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to be updated.</param>
		/// <param name="assigningAuthorityInfo">The assigning authority containing the updated information.</param>
		/// <returns>Returns the updated assigning authority.</returns>
		[WebInvoke(UriTemplate = "/assigningAuthority/{assigningAuthorityId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		AssigningAuthorityInfo UpdateAssigningAuthority(string assigningAuthorityId, AssigningAuthorityInfo assigningAuthorityInfo);

		/// <summary>
		/// Updates the code system.
		/// </summary>
		/// <param name="codeSystemId">The code system identifier.</param>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Return the updated code system.</returns>
		[WebInvoke(UriTemplate = "/codeSystem/{codeSystemId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		CodeSystem UpdateCodeSystem(string codeSystemId, CodeSystem codeSystem);

		/// <summary>
		/// Updates a device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be updated.</param>
		/// <param name="deviceInfo">The device containing the updated information.</param>
		/// <returns>Returns the updated device.</returns>
		[WebInvoke(UriTemplate = "/device/{deviceId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		SecurityDeviceInfo UpdateDevice(string deviceId, SecurityDeviceInfo deviceInfo);

		/// <summary>
		/// Updates the type of the extension.
		/// </summary>
		/// <param name="extensionTypeId">The extension type identifier.</param>
		/// <param name="extensionType">Type of the extension.</param>
		/// <returns>Returns the updated extension type.</returns>
		[WebInvoke(UriTemplate = "/extensionType/{extensionTypeId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		ExtensionType UpdateExtensionType(string extensionTypeId, ExtensionType extensionType);

		/// <summary>
		/// Updates a policy.
		/// </summary>
		/// <param name="policyId">The id of the policy to be updated.</param>
		/// <param name="policyInfo">The policy containing the updated information.</param>
		/// <returns>Returns the updated policy.</returns>
		[WebInvoke(UriTemplate = "/policy/{policyId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		SecurityPolicyInfo UpdatePolicy(string policyId, SecurityPolicyInfo policyInfo);

		/// <summary>
		/// Updates a role.
		/// </summary>
		/// <param name="roleId">The id of the role to be updated.</param>
		/// <param name="roleInfo">The role containing the updated information.</param>
		/// <returns>Returns the updated role.</returns>
		[WebInvoke(UriTemplate = "/role/{roleId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		SecurityRoleInfo UpdateRole(string roleId, SecurityRoleInfo roleInfo);

		/// <summary>
		/// Updates a security user.
		/// </summary>
		/// <param name="userId">The id of the security user to be retrieved.</param>
		/// <param name="userInfo">The user containing the updated information.</param>
		/// <returns>Returns the security user.</returns>
		[WebInvoke(UriTemplate = "/user/{userId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		SecurityUserInfo UpdateUser(string userId, SecurityUserInfo userInfo);

		#region Logging

		/// <summary>
		/// Gets a specific log file.
		/// </summary>
		/// <param name="logId">The log identifier.</param>
		/// <returns>Returns the log file information.</returns>
		[WebGet(UriTemplate = "/log/{logId}")]
		LogFileInfo GetLog(String logId);

		/// <summary>
		/// Get log files on the server and their sizes.
		/// </summary>
		/// <returns>Returns a collection of log files.</returns>
		[WebGet(UriTemplate = "/log")]
		AmiCollection<LogFileInfo> GetLogs();

		#endregion Logging

		#region Auditing

		/// <summary>
		/// Create audit in the IMS' audit repository.
		/// </summary>
		/// <param name="audit">The audit to save.</param>
		[WebInvoke(UriTemplate = "/audit", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		void CreateAudit(AuditInfo audit);

		#endregion Auditing
	}
}