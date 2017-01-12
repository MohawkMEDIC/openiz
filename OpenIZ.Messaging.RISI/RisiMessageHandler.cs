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
 * Date: 2016-8-28
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Wcf.Behavior;
using OpenIZ.Messaging.RISI.Configuration;
using OpenIZ.Messaging.RISI.Wcf;
using System;
using System.Diagnostics;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using OpenIZ.Messaging.RISI.Wcf.Behavior;

namespace OpenIZ.Messaging.RISI
{
	/// <summary>
	/// Represents a message handler for reporting services.
	/// </summary>
	public class RisiMessageHandler : IDaemonService
	{
		/// <summary>
		/// The internal reference to the trace source.
		/// </summary>
		private readonly TraceSource traceSource = new TraceSource("OpenIZ.Messaging.RISI");

		/// <summary>
		/// The internal reference to the configuration.
		/// </summary>
		private readonly RisiConfiguration configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.messaging.risi") as RisiConfiguration;

		/// <summary>
		/// The internal reference to the web host.
		/// </summary>
		private WebServiceHost webHost;

		/// <summary>
		/// Gets the running state of the message handler.
		/// </summary>
		public bool IsRunning => this.webHost?.State == System.ServiceModel.CommunicationState.Opened;

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
		/// Starts the service. Returns true if the service started successfully.
		/// </summary>
		/// <returns>Returns true if the service started successfully.</returns>
		public bool Start()
		{
			try
			{
				this.Starting?.Invoke(this, EventArgs.Empty);

				this.webHost = new WebServiceHost(typeof(RisiBehavior));

				foreach (var endpoint in this.webHost.Description.Endpoints)
				{
					this.traceSource.TraceInformation("Starting RISI on {0}...", endpoint.Address);
					endpoint.EndpointBehaviors.Add(new RisiRestEndpointBehavior());
					endpoint.EndpointBehaviors.Add(new WcfErrorEndpointBehavior());
				}

				// Start the webhost
				this.webHost.Open();

				this.traceSource.TraceEvent(TraceEventType.Information, 0, $"RISI configuration loaded, using { this.configuration.Handler.AssemblyQualifiedName } report engine at address { this.configuration.Address }");

				this.traceSource.TraceEvent(TraceEventType.Information, 0, $"Loading service provider { this.configuration.Handler.AssemblyQualifiedName }");
				ApplicationContext.Current.AddServiceProvider(this.configuration.Handler);

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
		/// Stops the service. Returns true if the service stopped successfully.
		/// </summary>
		/// <returns>Returns true if the service stopped successfully.</returns>
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