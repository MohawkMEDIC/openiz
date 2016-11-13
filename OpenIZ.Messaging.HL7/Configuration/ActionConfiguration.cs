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
 * Date: 2016-11-11
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Event;

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
		/// with a specific <see cref="NotificationType"/> instance.
		/// </summary>
		/// <param name="notificationType"></param>
		public ActionConfiguration(NotificationType notificationType)
		{
			this.NotificationType = notificationType;
		}

		public NotificationType NotificationType { get; set; }
	}
}
