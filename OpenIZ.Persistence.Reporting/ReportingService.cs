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
 * Date: 2017-1-7
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Persistence.Reporting.Configuration;
using System;
using System.Diagnostics;

namespace OpenIZ.Persistence.Reporting
{
	/// <summary>
	/// Represents a message handler for reporting services.
	/// </summary>
	public class ReportingService : IDaemonService
	{
		/// <summary>
		/// The internal reference to the trace source.
		/// </summary>
		private readonly TraceSource traceSource = new TraceSource("OpenIZ.Persistence.Reporting");

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
		/// The internal reference to the configuration.
		/// </summary>
		public static ReportingConfiguration Configuration => ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.persistence.reporting") as ReportingConfiguration;

		/// <summary>
		/// Gets the running state of the message handler.
		/// </summary>
		public bool IsRunning => false;

		/// <summary>
		/// Starts the service. Returns true if the service started successfully.
		/// </summary>
		/// <returns>Returns true if the service started successfully.</returns>
		public bool Start()
		{
			try
			{
				this.Starting?.Invoke(this, EventArgs.Empty);

				this.traceSource.TraceEvent(TraceEventType.Information, 0, $"Reporting configuration loaded, using connection string: { Configuration.ConnectionString }");

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

			this.Stopped?.Invoke(this, EventArgs.Empty);

			return true;
		}
	}
}