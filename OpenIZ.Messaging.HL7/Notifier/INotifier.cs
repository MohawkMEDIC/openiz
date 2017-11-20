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
using OpenIZ.Core.Model;
using OpenIZ.Messaging.HL7.Configuration;

namespace OpenIZ.Messaging.HL7.Notifier
{
	/// <summary>
	/// Represents a notifier.
	/// </summary>
	/// <typeparam name="T">The type of data of the notification.</typeparam>
	public interface INotifier
	{
		/// <summary>
		/// Gets or sets the target configuration of the notifier.
		/// </summary>
		TargetConfiguration TargetConfiguration { get; set; }

		/// <summary>
		/// Notifies a remote system.
		/// </summary>
		/// <param name="workItem">The work item of the notification.</param>
		void Notify<T>(NotificationQueueWorkItem<T> workItem) where T : IdentifiedData;
	}
}