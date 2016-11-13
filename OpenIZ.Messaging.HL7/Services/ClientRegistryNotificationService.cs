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
 * Date: 2016-11-3
 */
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Roles;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.Everest.Threading;
using OpenIZ.Core.Event;
using NHapi.Model.V25.Message;
using OpenIZ.Messaging.HL7.Queue;
using System.Diagnostics;

namespace OpenIZ.Messaging.HL7.Services
{
	/// <summary>
	/// Represents a client registry notification service.
	/// </summary>
	[Service(ServiceInstantiationType.Instance)]
	public class ClientRegistryNotificationService : IClientRegistryNotificationService, IDisposable
	{
		/// <summary>
		/// The internal reference to the sync lock instance.
		/// </summary>
		private object syncLock = new object();

		/// <summary>
		/// The internal reference to the <see cref="WaitThreadPool"/> instance.
		/// </summary>
		private WaitThreadPool threadPool;

		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// Initializes a new instance of the <see cref="ClientRegistryNotificationService"/> class.
		/// </summary>
		public ClientRegistryNotificationService()
		{
			this.threadPool = new WaitThreadPool();
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

		#endregion

		/// <summary>
		/// Notify that duplicates have been resolved.
		/// </summary>
		/// <param name="eventArgs">The notification event arguments.</param>
		public void NotifyDuplicatesResolved(NotificationEventArgs eventArgs)
		{

		}

		/// <summary>
		/// Notify that a registration occurred.
		/// </summary>
		/// <param name="eventArgs">The notification event arguments.</param>
		public void NotifyRegister(NotificationEventArgs eventArgs)
		{
			if (eventArgs.Data == null)
			{
				throw new ArgumentNullException(string.Format("{0} cannot be null", nameof(eventArgs.Data)));
			}

			if (eventArgs.Data is ADT_A01)
			{
				var message = eventArgs.Data as ADT_A01;

				message.MSH.MessageType.TriggerEvent.Value = "A04";

				MessageQueueWorkItem workItem = new MessageQueueWorkItem(message);

				if (!workItem.TrySend())
				{
					this.tracer.TraceEvent(TraceEventType.Warning, 0, "Unable to registration notification");
					Hl7MessageQueue.Current.Enqueue(workItem);
				}
			}
			else
			{
				throw new InvalidOperationException(string.Format("{0} must be of type {1}", nameof(eventArgs.Data), nameof(ADT_A01)));
			}
		}

		/// <summary>
		/// Notify that an update occurred.
		/// </summary>
		/// <param name="eventArgs">The notification event arguments.</param>
		public void NotifyUpdate(NotificationEventArgs eventArgs)
		{

		}
	}
}
