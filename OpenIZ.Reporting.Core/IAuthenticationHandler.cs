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
 * Date: 2017-1-7
 */
using System;
using OpenIZ.Reporting.Core.Auth;
using OpenIZ.Reporting.Core.Event;

namespace OpenIZ.Reporting.Core
{
	/// <summary>
	/// Represents an authentication handler.
	/// </summary>
	public interface IAuthenticationHandler : IDisposable
	{
		/// <summary>
		/// Occurs when a service is authenticated.
		/// </summary>
		event EventHandler<AuthenticatedEventArgs> Authenticated;

		/// <summary>
		/// Occurs when a service is authenticating.
		/// </summary>
		event EventHandler<AuthenticatingEventArgs> Authenticating;

		/// <summary>
		/// Occurs when a service fails authentication.
		/// </summary>
		event EventHandler<AuthenticationErrorEventArgs> OnAuthenticationError;

		/// <summary>
		/// Gets or sets the authentication result of the authentication handler.
		/// </summary>
		AuthenticationResult AuthenticationResult { get; set; }
	}
}