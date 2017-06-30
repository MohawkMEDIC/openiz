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
 * Date: 2016-8-2
 */
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace OpenIZ.Core.Http
{
	/// <summary>
	/// Represents a series of credentials which are used when accessing the mobile core
	/// </summary>
	public abstract class Credentials
	{
		// Principal
		private IPrincipal m_principal;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Authentication.Credentials"/> class.
		/// </summary>
		/// <param name="principal">Principal.</param>
		protected Credentials(IPrincipal principal)
		{
			this.m_principal = principal;
		}

		/// <summary>
		/// Gets the principal represented by this credential
		/// </summary>
		/// <value>The principal.</value>
		public virtual IPrincipal Principal { get { return this.m_principal; } }

		/// <summary>
		/// Get the http headers which are requried for the credential
		/// </summary>
		public abstract Dictionary<String, String> GetHttpHeaders();
	}
}