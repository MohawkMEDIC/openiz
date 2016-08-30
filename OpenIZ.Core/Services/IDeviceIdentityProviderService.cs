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
 * Date: 2016-7-7
 */

using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents an identity service which authenticates devices.
	/// </summary>
	public interface IDeviceIdentityProviderService
	{
		/// <summary>
		/// Authenticate the device based on certificate provided
		/// </summary>
		/// <param name="deviceCertificate">The certificate of the device used to authenticate the device.</param>
		/// <returns>Returns the principal of the device.</returns>
		IPrincipal Authenticate(X509Certificate2 deviceCertificate);
	}
}