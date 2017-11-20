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
using OpenIZ.Reporting.Core.Auth;

namespace OpenIZ.Reporting.Core
{
	/// <summary>
	/// Represents a service which supports bearer authentication.
	/// </summary>
	public interface ISupportBearerAuthentication : IAuthenticationHandler
	{
		/// <summary>
		/// Authenticates against a remote system using a bearer token.
		/// </summary>
		/// <param name="token">The bearer token used to authenticate against the remote system.</param>
		/// <returns>Returns an authentication result.</returns>
		AuthenticationResult Authenticate(string token);
	}
}