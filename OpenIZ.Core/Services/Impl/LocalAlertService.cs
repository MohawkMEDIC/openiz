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
 * User: khannan
 * Date: 2016-8-24
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Alerting;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents a local alert service.
	/// </summary>
	public class LocalAlertService : IAlertService
	{
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
		/// <param name="msg">The alert message to be broadcast.</param>
		public void BroadcastAlert(AlertMessage msg)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Searches for alerts.
		/// </summary>
		/// <param name="predicate">The predicate to use to search for alerts.</param>
		/// <param name="offset">The offset of the search.</param>
		/// <param name="count">The count of the search results.</param>
		/// <returns>Returns a list of alerts.</returns>
		public List<AlertMessage> FindAlerts(Expression<Func<AlertMessage, bool>> predicate, int offset, int? count)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets an alert.
		/// </summary>
		/// <param name="id">The id of the alert to be retrieved.</param>
		/// <returns>Returns an alert.</returns>
		public AlertMessage GetAlert(Guid id)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Saves an alert.
		/// </summary>
		/// <param name="msg">The alert message to be saved.</param>
		public void SaveAlert(AlertMessage msg)
		{
			throw new NotImplementedException();
		}
	}
}
