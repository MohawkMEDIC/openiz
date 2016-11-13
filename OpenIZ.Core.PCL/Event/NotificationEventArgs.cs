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
using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Event
{
	/// <summary>
	/// Represents notification event arguments.
	/// </summary>
	public class NotificationEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationEventArgs"/> class.
		/// </summary>
		public NotificationEventArgs()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationEventArgs"/> class
		/// with identified data.
		/// </summary>
		/// <param name="data">The raw request data.</param>
		public NotificationEventArgs(object data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(string.Format("{0} cannot be null", nameof(data)));
			}

			this.Data = data;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationEventArgs"/> class
		/// with a specific notification type.
		/// </summary>
		/// <param name="notificationType">The type of notification.</param>
		public NotificationEventArgs(NotificationType notificationType)
		{
			this.NotificationType = NotificationType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationEventArgs"/> class
		/// with identified data and a specific notification type.
		/// </summary>
		/// <param name="data">The raw request data.</param>
		/// <param name="notificationType">The type of notification.</param>
		public NotificationEventArgs(object data, NotificationType notificationType)
		{
			if (data == null)
			{
				throw new ArgumentNullException(string.Format("{0} cannot be null", nameof(data)));
			}

			this.Data = data;
			this.NotificationType = NotificationType;
		}

		/// <summary>
		/// Gets or sets the data of the notification.
		/// </summary>
		public object Data { get; set; }

		/// <summary>
		/// Gets or sets the type of notification.
		/// </summary>
		public NotificationType NotificationType { get; set; }
	}
}
