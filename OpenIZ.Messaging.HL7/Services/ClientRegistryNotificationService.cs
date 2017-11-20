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
 * Date: 2016-11-30
 */
using MARC.Everest.Threading;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Event;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.HL7.Configuration;
using OpenIZ.Messaging.HL7.Notifier;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenIZ.Core.Security;

namespace OpenIZ.Messaging.HL7.Services
{
	/// <summary>
	/// Represents a client registry notification service.
	/// </summary>
	[Service(ServiceInstantiationType.Instance)]
	public class ClientRegistryNotificationService : IClientRegistryNotificationService, IDisposable
	{
		/// <summary>
		/// The internal reference to the <see cref="NotificationConfiguration"/> instance.
		/// </summary>
		private readonly NotificationConfiguration configuration;

		/// <summary>
		/// The internal reference to the sync lock instance.
		/// </summary>
		private readonly object syncLock = new object();

		/// <summary>
		/// The internal reference to the <see cref="WaitThreadPool"/> instance.
		/// </summary>
		private readonly WaitThreadPool threadPool;

		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private readonly TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// Initializes a new instance of the <see cref="ClientRegistryNotificationService"/> class.
		/// </summary>
		public ClientRegistryNotificationService()
		{
			var configurationManager = ApplicationContext.Current.GetService<IConfigurationManager>();

			this.configuration = configurationManager.GetSection("openiz.messaging.hl7.notification.pixpdq") as NotificationConfiguration;

			this.threadPool = new WaitThreadPool(this.configuration.ConcurrencyLevel);
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.threadPool.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ClientRegistryNotificationService() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support

		/// <summary>
		/// Notify that duplicates have been resolved.
		/// </summary>
		/// <param name="eventArgs">The notification event arguments.</param>
		public void NotifyDuplicatesResolved(NotificationEventArgs<Patient> eventArgs)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Notifies a remote system.
		/// </summary>
		/// <param name="state">The notification data.</param>
		private void NotifyInternal(object state)
		{
			var workItem = state as NotificationQueueWorkItem<Patient>;

			if (workItem == null)
			{
				throw new ArgumentException($"Notification event data must be of type {typeof(NotificationQueueWorkItem<Patient>)}");
			}

			try
			{
				List<TargetConfiguration> targetConfigurations = null;

				lock (syncLock)
				{
					targetConfigurations = this.configuration.TargetConfigurations.FindAll(t => t.NotificationDomainConfigurations.Exists(delegate (NotificationDomainConfiguration domainConfiguration)
					{
						var action = domainConfiguration.ActionConfigurations.Exists(a => (a.ActionType & workItem.ActionType) == workItem.ActionType);
						var domain = workItem.Event.Identifiers.Exists(i => i.Authority.Oid == domainConfiguration.Domain);

						return action && domain;
					}));
				}

				foreach (var target in targetConfigurations)
				{
					target.Notifier.Notify(new NotificationQueueWorkItem<IdentifiedData>(workItem.Event, workItem.ActionType));
				}
			}
			catch (Exception e)
			{
#if DEBUG
				this.tracer.TraceEvent(TraceEventType.Error, 0, e.StackTrace);
#endif
				this.tracer.TraceEvent(TraceEventType.Error, 0, e.Message);
			}
		}

		/// <summary>
		/// Notify that a registration occurred.
		/// </summary>
		/// <param name="eventArgs">The notification event arguments.</param>
		public void NotifyRegister(NotificationEventArgs<Patient> eventArgs)
		{
			this.threadPool.QueueUserWorkItem(NotifyInternal, new NotificationQueueWorkItem<Patient>(eventArgs.Data, ActionType.Create));
		}

		/// <summary>
		/// Notify that an update occurred.
		/// </summary>
		/// <param name="eventArgs">The notification event arguments.</param>
		public void NotifyUpdate(NotificationEventArgs<Patient> eventArgs)
		{
			this.threadPool.QueueUserWorkItem(NotifyInternal, new NotificationQueueWorkItem<Patient>(eventArgs.Data, ActionType.Update));
		}
	}
}