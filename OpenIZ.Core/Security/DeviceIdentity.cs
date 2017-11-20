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
 * Date: 2017-3-13
 */
using System;
using System.Security.Claims;

namespace OpenIZ.Core.Security
{
	/// <summary>
	/// Represents a device identity.
	/// </summary>
	/// <seealso cref="System.Security.Claims.ClaimsIdentity" />
	public class DeviceIdentity : ClaimsIdentity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DeviceIdentity"/> class.
		/// </summary>
		/// <param name="sid">The sid.</param>
		/// <param name="name">The name.</param>
		/// <param name="isAuthenticated">if set to <c>true</c> [is authenticated].</param>
		public DeviceIdentity(Guid sid, string name, bool isAuthenticated)
		{
			this.IsAuthenticated = isAuthenticated;
			this.Name = name;
			this.AddClaim(new Claim(ClaimTypes.Sid, sid.ToString()));
		}

		/// <summary>
		/// Gets a value that indicates whether the identity has been authenticated.
		/// </summary>
		/// <returns>
		/// true if the identity has been authenticated; otherwise, false.
		/// </returns>
		public override bool IsAuthenticated { get; }

		/// <summary>
		/// Gets the name of this claims identity.
		/// </summary>
		/// <returns>
		/// The name or null.
		/// </returns>
		public override string Name { get; }
	}
}
