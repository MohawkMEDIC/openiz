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
 * User: khannan
 * Date: 2017-1-15
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Reporting.Core
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
		/// Initializes a new instance of the <see cref="AuthenticationResult"/> class
		/// with a specific success flag and token.
		/// </summary>
		/// <param name="success">The success flag of the authentication result.</param>
		/// <param name="token">The token value.</param>
		public AuthenticationResult(bool success, string token) : this(token)
		{
			this.IsSuccess = success;
		}

		/// <summary>
		/// Gets or sets whether the authentication result is a success.
		/// </summary>
		public bool IsSuccess { get; set; }

		/// <summary>
		/// Gets or sets the token of the authentication result.
		/// </summary>
		public string Token { get; set; }
	}
}
