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
 * Date: 2017-4-16
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Map;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Reporting.PSQL.Configuration;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace OpenIZ.Persistence.Reporting.PSQL
{
	/// <summary>
	/// Represents a persistence service for reporting services.
	/// </summary>
	public class ReportingPersistenceService : IDaemonService
	{
		/// <summary>
		/// The internal reference to the trace source.
		/// </summary>
		private readonly TraceSource traceSource = new TraceSource(ReportingPersistenceConstants.TraceName);

		static ReportingPersistenceService()
		{
			var tracer = new TraceSource(ReportingPersistenceConstants.TraceName);

			Configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(ReportingPersistenceConstants.ConfigurationSectionName) as ReportingConfiguration;

			try
			{
				ModelMapper = new ModelMapper(typeof(ReportingPersistenceService).GetTypeInfo().Assembly.GetManifestResourceStream(ReportingPersistenceConstants.MapResourceName));
				QueryBuilder = new QueryBuilder(ModelMapper, Configuration.Provider);
			}
			catch (ModelMapValidationException e)
			{
				tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error validating model map: {0}", e);
				throw;
			}
			catch (Exception e)
			{
				tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error validating model map: {0}", e);
				throw;
			}
		}

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
		public static ReportingConfiguration Configuration { get; }

		/// <summary>
		/// Gets the model mapper.
		/// </summary>
		public static ModelMapper ModelMapper { get; }

		/// <summary>
		/// Gets the query builder.
		/// </summary>
		/// <value>The query builder.</value>
		public static QueryBuilder QueryBuilder { get; }

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
			try
			{
				using (var dataContext = Configuration.Provider.GetReadonlyConnection())
				{
					dataContext.Open();

					var databaseVersion = new Version(dataContext.FirstOrDefault<string>("get_sch_vrsn"));
					var openizVersion = typeof(ReportingPersistenceService).Assembly.GetName().Version;

					if (openizVersion < databaseVersion)
					{
						throw new InvalidOperationException($"Invalid schema version. OpenIZ version {openizVersion} is older than the database schema version: {databaseVersion}");
					}

					traceSource.TraceEvent(TraceEventType.Information, 0, $"OpenIZ Reporting schema version: {databaseVersion}");
				}

				this.traceSource.TraceEvent(TraceEventType.Information, 0, "Loading reporting persistence services");

				this.Starting?.Invoke(this, EventArgs.Empty);

				this.traceSource.TraceEvent(TraceEventType.Information, 0, $"Reporting configuration loaded, using connection string: { Configuration.ReadWriteConnectionString }");

				// Iterate the persistence services
				foreach (var t in typeof(ReportingPersistenceService).GetTypeInfo().Assembly.DefinedTypes.Where(o => o.Namespace == "OpenIZ.Persistence.Reporting.PSQL.Services" && !o.GetTypeInfo().IsAbstract && !o.IsGenericTypeDefinition))
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
			}
			catch (Exception e)
			{
				this.traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
				throw;
			}

			return true;
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