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
using OpenIZ.Core.Interop;
using OpenIZ.Core.Wcf;
using OpenIZ.Core.Wcf.Behavior;
using OpenIZ.Core.Wcf.Security;
using OpenIZ.Messaging.AMI.Configuration;
using OpenIZ.Messaging.AMI.Wcf;
using OpenIZ.Messaging.AMI.Wcf.Behavior;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.AMI
{
	/// <summary>
	/// AMI Message handler
	/// </summary>
    [Description("AMI Message Service")]
	public class AmiMessageHandler : IDaemonService, IApiEndpointProvider
	{
		/// <summary>
		/// The internal reference to the trace source.
		/// </summary>
		private readonly TraceSource tracer = new TraceSource("OpenIZ.Messaging.AMI");

		/// <summary>
		/// The internal reference to the AMI configuration.
		/// </summary>
		private AmiConfiguration configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.messaging.ami") as AmiConfiguration;

		// web host
		private WebServiceHost m_webHost;

		/// <summary>
		/// Fired when the object is starting up.
		/// </summary>
		public event EventHandler Started;

		/// <summary>
		/// Fired when the object is starting.
		/// </summary>
		public event EventHandler Starting;

		/// <summary>
		/// Fired when the service has stopped.
		/// </summary>
		public event EventHandler Stopped;

		/// <summary>
		/// Fired when the service is stopping.
		/// </summary>
		public event EventHandler Stopping;

		/// <summary>
		/// Gets the API type
		/// </summary>
		public ServiceEndpointType ApiType
		{
			get
			{
				return ServiceEndpointType.AdministrationIntegrationService;
			}
		}

		/// <summary>
		/// Capabilities
		/// </summary>
		public ServiceEndpointCapabilities Capabilities
		{
			get
			{
				var caps = ServiceEndpointCapabilities.Compression;
				if (this.m_webHost.Description.Behaviors.OfType<ServiceCredentials>().Any(o => o.UserNameAuthentication?.CustomUserNamePasswordValidator != null))
					caps |= ServiceEndpointCapabilities.BasicAuth;
				if (this.m_webHost.Description.Behaviors.OfType<ServiceAuthorizationBehavior>().Any(o => o.ServiceAuthorizationManager is JwtTokenServiceAuthorizationManager))
					caps |= ServiceEndpointCapabilities.BearerAuth;

				return caps;
			}
		}

		/// <summary>
		/// True if running
		/// </summary>
		public bool IsRunning
		{
			get
			{
				return this.m_webHost?.State == CommunicationState.Opened;
			}
		}

		/// <summary>
		/// URL of the service
		/// </summary>
		public string[] Url
		{
			get
			{
				return this.m_webHost.Description.Endpoints.OfType<ServiceEndpoint>().Select(o => o.Address.Uri.ToString()).ToArray();
			}
		}

		/// <summary>
		/// Start the service
		/// </summary>
		public bool Start()
		{
            // Don't startup unless in OpenIZ
            if (Assembly.GetEntryAssembly().GetName().Name != "OpenIZ")
                return true;

            try
            {
				this.Starting?.Invoke(this, EventArgs.Empty);

				this.m_webHost = new WebServiceHost(typeof(AmiBehavior));

				foreach (ServiceEndpoint endpoint in this.m_webHost.Description.Endpoints)
				{
					this.tracer.TraceInformation("Starting AMI on {0}...", endpoint.Address);
					(endpoint.Binding as WebHttpBinding).ContentTypeMapper = new AmiContentTypeHandler();

					endpoint.EndpointBehaviors.Add(new AmiRestEndpointBehavior());
					endpoint.EndpointBehaviors.Add(new WcfErrorEndpointBehavior());
				}

				// Start the webhost
				this.m_webHost.Open();

				this.Started?.Invoke(this, EventArgs.Empty);
				return true;
			}
			catch (Exception e)
			{
				this.tracer.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
				return false;
			}
		}

		/// <summary>
		/// Stop the IMSI service
		/// </summary>
		public bool Stop()
		{
			this.Stopping?.Invoke(this, EventArgs.Empty);

			if (this.m_webHost != null)
			{
				this.m_webHost.Close();
				this.m_webHost = null;
			}

			this.Stopped?.Invoke(this, EventArgs.Empty);

			return true;
		}
	}
}