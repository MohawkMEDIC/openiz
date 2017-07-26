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
 * User: justi
 * Date: 2016-11-30
 */
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Interop;
using OpenIZ.Core.Wcf;
using OpenIZ.Core.Wcf.Behavior;
using OpenIZ.Core.Wcf.Security;
using OpenIZ.Messaging.GS1.Wcf;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.GS1
{
	/// <summary>
	/// Stock service message handler
	/// </summary>
    [Description("GS1 Stock Service")]
	public class StockServiceMessageHandler : IMessageHandlerService, IApiEndpointProvider
	{
		// IMSI Trace host
		private readonly TraceSource traceSource = new TraceSource("OpenIZ.Messaging.GS1");

		// web host
		private WebServiceHost webHost;

		/// <summary>
		/// Fired when the object is starting up
		/// </summary>
		public event EventHandler Started;

		/// <summary>
		/// Fired when the object is starting
		/// </summary>
		public event EventHandler Starting;

		/// <summary>
		/// Fired when the service has stopped
		/// </summary>
		public event EventHandler Stopped;

		/// <summary>
		/// Fired when the service is stopping
		/// </summary>
		public event EventHandler Stopping;

		/// <summary>
		/// True if running
		/// </summary>
		public bool IsRunning => this.webHost?.State == CommunicationState.Opened;


        /// <summary>
        /// Gets the API type
        /// </summary>
        public ServiceEndpointType ApiType
        {
            get
            {
                return ServiceEndpointType.Gs1StockInterface;
            }
        }

        /// <summary>
        /// URL of the service
        /// </summary>
        public string[] Url
        {
            get
            {
                return this.webHost.Description.Endpoints.OfType<ServiceEndpoint>().Select(o => o.Address.Uri.ToString()).ToArray();
            }
        }

        /// <summary>
        /// Capabilities
        /// </summary>
        public ServiceEndpointCapabilities Capabilities
        {
            get
            {
                var caps = ServiceEndpointCapabilities.None;
                if (this.webHost.Description.Behaviors.OfType<ServiceCredentials>().Any(o => o.UserNameAuthentication?.CustomUserNamePasswordValidator != null))
                    caps |= ServiceEndpointCapabilities.BasicAuth;
                if (this.webHost.Description.Behaviors.OfType<ServiceAuthorizationBehavior>().Any(o => o.ServiceAuthorizationManager is JwtTokenServiceAuthorizationManager))
                    caps |= ServiceEndpointCapabilities.BearerAuth;

                return caps;
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

				this.webHost = new WebServiceHost(typeof(StockServiceBehavior));
				foreach (ServiceEndpoint endpoint in this.webHost.Description.Endpoints)
				{
					this.traceSource.TraceInformation("Starting GS1 on {0}...", endpoint.Address);
                    endpoint.EndpointBehaviors.Add(new WcfErrorEndpointBehavior());
                }
				// Start the webhost
				this.webHost.Open();

				this.Started?.Invoke(this, EventArgs.Empty);
				return true;
			}
			catch (Exception e)
			{
				this.traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
				return false;
			}
		}

		/// <summary>
		/// Stop the IMSI service
		/// </summary>
		public bool Stop()
		{
			this.Stopping?.Invoke(this, EventArgs.Empty);

			if (this.webHost != null)
			{
				this.webHost.Close();
				this.webHost = null;
			}

			this.Stopped?.Invoke(this, EventArgs.Empty);

			return true;
		}
	}
}