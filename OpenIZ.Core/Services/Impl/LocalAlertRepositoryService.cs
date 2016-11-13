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
 * Date: 2016-9-2
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Alert.Alerting;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents a local alert service.
	/// </summary>
	public class LocalAlertRepositoryService : IAlertRepositoryService
	{
		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private TraceSource traceSource = new TraceSource("OpenIZ.Core");

		/// <summary>
		/// Fired when an alert was raised and is being processed.
		/// </summary>
		public event EventHandler<AlertEventArgs> Committed;

		/// <summary>
		/// Fired when an alert is received.
		/// </summary>
		public event EventHandler<AlertEventArgs> Received;

		/// <summary>
		/// Broadcasts an alert.
		/// </summary>
		/// <param name="message">The alert message to be broadcast.</param>
		public void BroadcastAlert(AlertMessage message)
		{
			this.Committed?.Invoke(this, new AlertEventArgs(message));
		}

		/// <summary>
		/// Searches for alerts.
		/// </summary>
		/// <param name="predicate">The predicate to use to search for alerts.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The count of the search results.</param>
		/// <param name="totalCount">The total count of the alerts.</param>
		/// <returns>Returns a list of alerts.</returns>
		public IEnumerable<AlertMessage> Find(Expression<Func<AlertMessage, bool>> predicate, int offset, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AlertMessage>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<AlertMessage>)));
			}

			return persistenceService.Query(predicate, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Gets an alert.
		/// </summary>
		/// <param name="id">The id of the alert to be retrieved.</param>
		/// <returns>Returns an alert.</returns>
		public AlertMessage Get(Guid id)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AlertMessage>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<AlertMessage>)));
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(id), AuthenticationContext.Current.Principal, false);
		}

		/// <summary>
		/// Inserts an alert message.
		/// </summary>
		/// <param name="message">The alert message to be inserted.</param>
		/// <returns>Returns the inserted alert.</returns>
		public AlertMessage Insert(AlertMessage message)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AlertMessage>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<AlertMessage>)));
			}

			AlertMessage alert;

			try
			{
				alert = persistenceService.Insert(message, AuthenticationContext.Current.Principal, TransactionMode.Commit);
				this.Received?.Invoke(this, new AlertEventArgs(alert));
			}
			catch (Exception e)
			{
#if DEBUG
				this.traceSource.TraceEvent(TraceEventType.Error, 0, e.StackTrace);
#endif
				this.traceSource.TraceEvent(TraceEventType.Error, 0, e.Message);

				throw;
			}

			return alert;
		}

		/// <summary>
		/// Saves an alert.
		/// </summary>
		/// <param name="message">The alert message to be saved.</param>
		public AlertMessage Save(AlertMessage message)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AlertMessage>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<AlertMessage>)));
			}

			AlertMessage alert;

			try
			{
				alert = persistenceService.Update(message, AuthenticationContext.Current.Principal, TransactionMode.Commit);
				this.Received?.Invoke(this, new AlertEventArgs(alert));
			}
			catch (KeyNotFoundException)
			{
				alert = persistenceService.Insert(message, AuthenticationContext.Current.Principal, TransactionMode.Commit);
				this.Received?.Invoke(this, new AlertEventArgs(alert));
			}

			return alert;
		}
	}
}