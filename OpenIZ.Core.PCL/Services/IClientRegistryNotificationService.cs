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
 * Date: 2016-11-30
 */
using OpenIZ.Core.Event;
using OpenIZ.Core.Model.Roles;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a client registry notification service.
	/// </summary>
	public interface IClientRegistryNotificationService
	{
		/// <summary>
		/// Notify that duplicates have been resolved.
		/// </summary>
		/// <param name="eventArgs">The notification event arguments.</param>
		void NotifyDuplicatesResolved(NotificationEventArgs<Patient> eventArgs);

		/// <summary>
		/// Notify that a registration occurred.
		/// </summary>
		/// <param name="eventArgs">The notification event arguments.</param>
		void NotifyRegister(NotificationEventArgs<Patient> eventArgs);

		/// <summary>
		/// Notify that an update occurred.
		/// </summary>
		/// <param name="eventArgs">The notification event arguments.</param>
		void NotifyUpdate(NotificationEventArgs<Patient> eventArgs);
	}
}