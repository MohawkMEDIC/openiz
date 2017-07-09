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
using OpenIZ.Messaging.HL7.Notifier;

namespace OpenIZ.Messaging.HL7.Configuration
{
	/// <summary>
	/// Represents an action configuration.
	/// </summary>
	public class ActionConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ActionConfiguration"/> class.
		/// </summary>
		public ActionConfiguration()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionConfiguration"/> class
		/// with a specific <see cref="Notifier.ActionType"/> instance.
		/// </summary>
		/// <param name="actionType"></param>
		public ActionConfiguration(ActionType actionType)
		{
			this.ActionType = actionType;
		}

		/// <summary>
		/// Gets or sets the action type of the action configuration.
		/// </summary>
		public ActionType ActionType { get; set; }
	}
}