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
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

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
		public static ReportingConfiguration Configuration
		{
			get
			{
				var configurationManager = ApplicationContext.Current.GetService<IConfigurationManager>();

				ReportingConfiguration config = null;

				if (configurationManager == null)
				{	
					config = ConfigurationManager.GetSection("openiz.persistence.reporting") as ReportingConfiguration;
				}
				else
				{
					config = configurationManager.GetSection("openiz.persistence.reporting") as ReportingConfiguration;
				}

				return config;
			}
		} 

		/// <summary>
		/// Gets the running state of the message handler.
		/// </summary>
		public bool IsRunning => true;

		/// <summary>
		/// Starts the service. Returns true if the service started successfully.
		/// </summary>
		/// <returns>Returns true if the service started successfully.</returns>
		public bool Start()
		{
			var status = false;

			try
			{
				this.traceSource.TraceEvent(TraceEventType.Information, 0, "Loading reporting persistence services");

				this.Starting?.Invoke(this, EventArgs.Empty);

				this.traceSource.TraceEvent(TraceEventType.Information, 0, $"Reporting configuration loaded, using connection string: { Configuration.ConnectionStringName }");

				// Iterate the persistence services
				foreach (var t in typeof(ReportingService).GetTypeInfo().Assembly.ExportedTypes.Where(o => o.Namespace == "OpenIZ.Persistence.Reporting.Services" && !o.GetTypeInfo().IsAbstract))
				{
					try
					{
						this.traceSource.TraceEvent(TraceEventType.Verbose, 0, "Loading {0}...", t.AssemblyQualifiedName);
						ApplicationContext.Current.AddServiceProvider(t);
					}
					catch (Exception e)
					{
						this.traceSource.TraceEvent(TraceEventType.Error, e.HResult, "Error adding service {0} : {1}", t.AssemblyQualifiedName, e);
					}
				}

				this.Started?.Invoke(this, EventArgs.Empty);
				status = true;
			}
			catch (Exception e)
			{
				this.traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
				status = false;
			}

			return status;
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