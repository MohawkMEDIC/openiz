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
 * Date: 2017-7-5
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Event;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;

namespace OpenIZ.Messaging.HL7.Subscriber
{
	/// <summary>
	/// Represents a bundle persistence subscriber.
	/// </summary>
	public class BundlePersistenceSubscriber : IDaemonService
	{
		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private readonly TraceSource traceSource = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// Gets a value indicating whether this instance is running.
		/// </summary>
		/// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
		public bool IsRunning => true;

		/// <summary>
		/// Occurs when the service is starting.
		/// </summary>
		public event EventHandler Starting;

		/// <summary>
		/// Occurs when the service is stopping.
		/// </summary>
		public event EventHandler Stopping;

		/// <summary>
		/// Occurs when the service is started.
		/// </summary>
		public event EventHandler Started;

		/// <summary>
		/// Occurs when the service is stopped.
		/// </summary>
		public event EventHandler Stopped;

		/// <summary>
		/// Starts this instance.
		/// </summary>
		/// <returns><c>true</c> if the service started successfully, <c>false</c> otherwise.</returns>
		public bool Start()
		{
			try
			{
				// HACK: need to revisit bundle BRE
				ApplicationContext.Current.Started += (o, e) =>
				{
					ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>().Inserted += (sender, instance) =>
					{
						foreach (var patient in instance.Data.Item.OfType<Patient>())
						{
							ApplicationContext.Current.GetService<IClientRegistryNotificationService>()?.NotifyRegister(new NotificationEventArgs<Patient>(patient));
						}
					};

					ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>().Updated += (sender, instance) =>
					{
						foreach (var patient in instance.Data.Item.OfType<Patient>())
						{
							ApplicationContext.Current.GetService<IClientRegistryNotificationService>()?.NotifyUpdate(new NotificationEventArgs<Patient>(patient));
						}
					};
				};

				this.traceSource.TraceEvent(TraceEventType.Information, 0, "Bundle persistence Client Registry Notifications started successfully");

			}
			catch (Exception e)
			{
				traceSource.TraceEvent(TraceEventType.Error, 1900, $"Unable to subscribe to bundle persistence events on the HL7 handler instance: {e}");
			}

			return true;
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		/// <returns><c>true</c> if the service stopped successfully, <c>false</c> otherwise.</returns>
		public bool Stop()
		{
			return true;
		}
	}
}

