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

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using OpenIZ.Core.Interop;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.Diagnostics;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Security.Claims;
using OpenIZ.Core.Services;
using OpenIZ.Core.Wcf;
using OpenIZ.Messaging.AMI.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Web;
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
		// Trace source
		private TraceSource traceSource = new TraceSource("OpenIZ.Messaging.AMI");

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
        /// Create a diagnostic report
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
        public DiagnosticReport GetServerDiagnosticReport()
        {
            var retVal = new DiagnosticReport();
            retVal.ApplicationInfo = new DiagnosticApplicationInfo(Assembly.GetEntryAssembly());
            retVal.CreatedByKey = Guid.Parse(AuthenticationContext.SystemUserSid);

            retVal.ApplicationInfo.Assemblies = AppDomain.CurrentDomain.GetAssemblies().Select(o => new DiagnosticVersionInfo(o)).ToList();
            retVal.ApplicationInfo.EnvironmentInfo = new DiagnosticEnvironmentInfo()
            {
                Is64Bit = Environment.Is64BitOperatingSystem && Environment.Is64BitProcess,
                OSVersion = String.Format("{0} v{1}", System.Environment.OSVersion.Platform, System.Environment.OSVersion.Version),
                ProcessorCount = Environment.ProcessorCount,
                UsedMemory = GC.GetTotalMemory(false),
                Version = Environment.Version.ToString()
            };
            retVal.ApplicationInfo.ServiceInfo = ApplicationContext.Current.GetServices().OfType<IDaemonService>().Select(o => new DiagnosticServiceInfo(o)).ToList();
            return retVal;
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
				this.traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
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

			// Get endpoints
			serviceOptions.Endpoints = ApplicationContext.Current.GetServices().OfType<IApiEndpointProvider>().Select(o =>
				new ServiceEndpointOptions()
				{
					BaseUrl = o.Url,
					ServiceType = o.ApiType,
					Capabilities = o.Capabilities
				}
			).ToList();

			var config = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.messaging.ami") as AmiConfiguration;
			if (config != null && config.Endpoints != null)
				serviceOptions.Endpoints.AddRange(config.Endpoints);
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
		/// Perform a ping
		/// </summary>
		public void Ping()
		{
			WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
		}

		/// <summary>
		/// Creates security reset information
		/// </summary>
		public void SendTfaSecret(TfaRequestInfo resetInfo)
		{
			var securityRepository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			var securityUser = securityRepository.GetUser(resetInfo.UserName);

			// don't throw an error if the user is not found, just act as if we sent it.
			// this is to make sure that people cannot guess users
			if (securityUser == null)
			{
                this.traceSource.TraceEvent(TraceEventType.Warning, 0, "Attempt to get TFA reset code for {0} which is not a valid user", resetInfo.UserName);
				WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NoContent;
				return;
			}

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