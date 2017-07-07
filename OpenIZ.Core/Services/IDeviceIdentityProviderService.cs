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
using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core.Event;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents an identity service which authenticates devices.
	/// </summary>
	public interface IDeviceIdentityProviderService
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
		/// Authenticates the specified device identifier.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="deviceSecret">The device secret.</param>
		/// <returns>Returns the authenticated device principal.</returns>
		IPrincipal Authenticate(string deviceId, string deviceSecret);

		/// <summary>
		/// Authenticate the device based on certificate provided
		/// </summary>
		/// <param name="deviceCertificate">The certificate of the device used to authenticate the device.</param>
		/// <returns>Returns the authenticated device principal.</returns>
		IPrincipal Authenticate(X509Certificate2 deviceCertificate);

		/// <summary>
		/// Gets the identity of the device using a given device name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>Returns the identity of the device.</returns>
		IIdentity GetIdentity(string name);
	}
}