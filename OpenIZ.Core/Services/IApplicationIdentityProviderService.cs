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
 * Date: 2016-7-7
 */
using MARC.HI.EHRS.SVC.Core.Event;
using System;
using System.Security.Principal;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a service which retrieves IPrincipal objects for applications.
	/// </summary>
	public interface IApplicationIdentityProviderService
	{
		/// <summary>
		/// Fired after an authentication request has been made.
		/// </summary>
		event EventHandler<AuthenticatedEventArgs> Authenticated;

		/// <summary>
		/// Fired prior to an authentication request being made.
		/// </summary>
		event EventHandler<AuthenticatingEventArgs> Authenticating;

		/// <summary>
		/// Authenticate the application identity.
		/// </summary>
		/// <param name="applicationId">The application id to authenticate.</param>
		/// <param name="applicationSecret">The application secret to authenticate.</param>
		/// <returns>Returns the principal of the application.</returns>
		IPrincipal Authenticate(String applicationId, String applicationSecret);

		/// <summary>
		/// Gets the specified identity for an application.
		/// </summary>
		/// <param name="name">The name of the application for which to retrieve the identity.</param>
		/// <returns>Returns the identity of the application.</returns>
		IIdentity GetIdentity(string name);
	}
}