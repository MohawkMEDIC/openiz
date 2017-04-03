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
 * User: Nityan
 * Date: 2017-4-2
 */

using OpenIZ.Messaging.CSD.Model;
using System.ServiceModel;

namespace OpenIZ.Messaging.CSD.Wcf.Csd
{
	/// <summary>
	/// Represents a care services discovery contract.
	/// </summary>
	[ServiceContract(Name = "CSD", ConfigurationName = "CSD", Namespace = "http://openiz.org/csd")]
	public interface ICareServicesDiscoveryContract
	{
		/// <summary>
		/// Searches the specified request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>careServicesResponse.</returns>
		[OperationContract(Name = "careServicesRequest")]
		careServicesResponse Search(careServicesRequest request);
	}
}