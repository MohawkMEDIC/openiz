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
using OpenIZ.Core.Http;
using OpenIZ.Core.Http.Description;

namespace OpenIZ.Core.Interop.Clients
{
	/// <summary>
	/// Represents a basic service client
	/// </summary>
	public abstract class ServiceClientBase
	{
		// The configuration
		private IRestClientDescription m_configuration;

		// The rest client
		private IRestClient m_restClient;

		/// <summary>
		/// Gets the client.
		/// </summary>
		/// <value>The client.</value>
		public IRestClient Client
		{
			get
			{
				return this.m_restClient;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Interop.Clients.ServiceClientBase"/> class.
		/// </summary>
		/// <param name="clientName">Client name.</param>
		public ServiceClientBase(IRestClient restClient)
		{
			this.m_restClient = restClient;
			this.m_configuration = this.m_restClient.Description;
		}
	}
}