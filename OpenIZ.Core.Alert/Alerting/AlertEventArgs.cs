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
 * Date: 2016-8-29
 */
using System;

namespace OpenIZ.Core.Alert.Alerting
{
	/// <summary>
	/// Represents alert event arguments.
	/// </summary>
	public class AlertEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AlertEventArgs"/> class
		/// with a specified alert message.
		/// </summary>
		/// <param name="message">The alert message.</param>
		public AlertEventArgs(AlertMessage message)
		{
			this.Message = message;
		}

		/// <summary>
		/// Allows the handler to instruct the alert engine to ignore (not to persist) the
		/// alert.
		/// </summary>
		public bool Ignore { get; set; }

		/// <summary>
		/// Gets the alert message.
		/// </summary>
		public AlertMessage Message { get; internal set; }
	}
}