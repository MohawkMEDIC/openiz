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
 * User: Nityan
 * Date: 2017-3-31
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Wcf.Behavior;
using OpenIZ.Messaging.CSD.Configuration;

namespace OpenIZ.Messaging.CSD
{
	/// <summary>
	/// Represents a CSD message handler service.
	/// </summary>
	/// <seealso cref="MARC.HI.EHRS.SVC.Core.Services.IMessageHandlerService" />
	public class CsdMessageHandler : IMessageHandlerService
	{
		/// <summary>
		/// The internal reference to the configuration.
		/// </summary>
		private readonly CsdConfiguration configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.messaging.csd") as CsdConfiguration;

		/// <summary>
		/// The internal reference to the trace source.
		/// </summary>
		private readonly TraceSource traceSource = new TraceSource("OpenIZ.Messaging.CSD");

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
		/// Gets the running state of the message handler.
		/// </summary>
		/// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
		public bool IsRunning
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Starts the service. Returns true if the service started successfully.
		/// </summary>
		/// <returns>Returns true if the service started successfully.</returns>
		public bool Start()
		{
			try
			{
				this.Starting?.Invoke(this, EventArgs.Empty);

				traceSource.TraceEvent(TraceEventType.Information, 0, "Starting CSD message handler...");

				this.Started?.Invoke(this, EventArgs.Empty);

				traceSource.TraceEvent(TraceEventType.Information, 0, "Started CSD message handler");
			}
			catch (Exception e)
			{
				throw;
			}

			throw new NotImplementedException();
		}

		/// <summary>
		/// Stops the service. Returns true if the service stopped successfully.
		/// </summary>
		/// <returns>Returns true if the service stopped successfully.</returns>
		public bool Stop()
		{
			try
			{
				this.Stopping?.Invoke(this, EventArgs.Empty);

				traceSource.TraceEvent(TraceEventType.Information, 0, "Stopping CSD message handler...");

				this.Stopped?.Invoke(this, EventArgs.Empty);

				traceSource.TraceEvent(TraceEventType.Information, 0, "Stopped CSD message handler");
			}
			catch (Exception e)
			{
				throw;
			}

			throw new NotImplementedException();
		}
	}
}
