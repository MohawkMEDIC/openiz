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
 * Date: 2017-4-4
 */
using System;
using OpenIZ.Reporting.Core.Auth;

namespace OpenIZ.Reporting.Core.Event
{
	/// <summary>
	/// Represents authenticated event arguments.
	/// </summary>
	/// <seealso cref="System.EventArgs" />
	public class AuthenticatedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AuthenticatedEventArgs"/> class.
		/// </summary>
		public AuthenticatedEventArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthenticatedEventArgs"/> class.
		/// </summary>
		/// <param name="authenticationResult">The authentication result.</param>
		public AuthenticatedEventArgs(AuthenticationResult authenticationResult)
		{
			this.AuthenticationResult = authenticationResult;
		}

		/// <summary>
		/// Gets the authentication result.
		/// </summary>
		/// <value>The authentication result.</value>
		public AuthenticationResult AuthenticationResult { get; set; }
	}
}