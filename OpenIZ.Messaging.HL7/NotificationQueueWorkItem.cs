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
 * Date: 2016-11-12
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Event;
using OpenIZ.Core.Model;

namespace OpenIZ.Messaging.HL7
{
	/// <summary>
	/// Represents a notification work item for the wait thread pool.
	/// </summary>
	/// <typeparam name="T">The type of data of the notification.</typeparam>
	public class NotificationQueueWorkItem<T> where T : IdentifiedData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationQueueWorkItem{T}"/> class
		/// with a specific identified data and notification type.
		/// </summary>
		/// <param name="data">The data of the notification work item.</param>
		/// <param name="notificationType">The notification type of the work item.</param>
		public NotificationQueueWorkItem(T data, NotificationType notificationType)
		{
			this.Event = data;
			this.NotificationType = notificationType;
		}

		/// <summary>
		/// Gets the event that triggered the action.
		/// </summary>
		public T Event { get; }

		/// <summary>
		/// Gets the action to perform on the event.
		/// </summary>
		public NotificationType NotificationType { get; }
	}
}
