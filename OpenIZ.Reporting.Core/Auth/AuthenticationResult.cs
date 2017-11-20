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
namespace OpenIZ.Reporting.Core.Auth
{
	/// <summary>
	/// Represents an authentication result.
	/// </summary>
	public class AuthenticationResult
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AuthenticationResult"/> class.
		/// </summary>
		public AuthenticationResult()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthenticationResult"/> class
		/// with a specific token.
		/// </summary>
		/// <param name="token">The token value.</param>
		public AuthenticationResult(string token)
		{
			this.Token = token;
		}

		/// <summary>
		/// Gets or sets the token of the authentication result.
		/// </summary>
		public string Token { get; }

		/// <summary>
		/// Returns a security token from the authentication result.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString()
		{
			return this.Token;
		}
	}
}