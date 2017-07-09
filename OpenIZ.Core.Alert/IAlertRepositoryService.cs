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
 * Date: 2016-9-3
 */
using OpenIZ.Core.Alert.Alerting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents an alerting service.
	/// </summary>
	public interface IAlertRepositoryService
	{
		/// <summary>
		/// Fired when an alert is received.
		/// </summary>
		event EventHandler<AlertEventArgs> Committed;

		/// <summary>
		/// Fired when an alert was raised and is being processed.
		/// </summary>
		event EventHandler<AlertEventArgs> Received;

		/// <summary>
		/// Broadcasts an alert.
		/// </summary>
		/// <param name="message">The message.</param>
		void BroadcastAlert(AlertMessage message);

		/// <summary>
		/// Searches for alerts.
		/// </summary>
		/// <param name="predicate">The predicate to use to search for alerts.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The count of the search results.</param>
		/// <param name="totalCount">The total count of the alerts.</param>
		/// <returns>Returns a list of alerts.</returns>
		IEnumerable<AlertMessage> Find(Expression<Func<AlertMessage, bool>> predicate, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets an alert.
		/// </summary>
		/// <param name="id">The id of the alert to be retrieved.</param>
		/// <returns>Returns an alert.</returns>
		AlertMessage Get(Guid id);

		/// <summary>
		/// Inserts an alert message.
		/// </summary>
		/// <param name="message">The alert message to be inserted.</param>
		/// <returns>Returns the inserted alert.</returns>
		AlertMessage Insert(AlertMessage message);

		/// <summary>
		/// Saves an alert.
		/// </summary>
		/// <param name="message">The alert message to be saved.</param>
		/// <returns>Returns the saved alert.</returns>
		AlertMessage Save(AlertMessage message);
	}
}