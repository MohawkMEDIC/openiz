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
 * Date: 2017-1-6
 */

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace OpenIZ.Messaging.RISI.Wcf.Behavior
{
	/// <summary>
	/// Represents the endpoint behavior for the RISI endpoint.
	/// </summary>
	public class RisiRestEndpointBehavior : IEndpointBehavior
	{
		/// <summary>
		/// Implement to pass data at runtime to bindings to support custom behavior.
		/// </summary>
		/// <param name="endpoint">The endpoint to modify.</param>
		/// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		/// <summary>
		/// Implements a modification or extension of the client across an endpoint.
		/// </summary>
		/// <param name="endpoint">The endpoint that is to be customized.</param>
		/// <param name="clientRuntime">The client runtime to be customized.</param>
		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
		}

		/// <summary>
		/// Applies dispatch behavior.
		/// </summary>
		/// <param name="endpoint">The endpoint for which to apply the behavior.</param>
		/// <param name="endpointDispatcher">The endpoint dispatcher of the endpoint.</param>
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			// Apply to each operation the RISI formatter
			foreach (var op in endpoint.Contract.Operations)
			{
				op.OperationBehaviors.Add(new RisiSerializerOperationBehavior());
			}
		}

		/// <summary>
		/// Validates a service endpoint.
		/// </summary>
		/// <param name="endpoint">The service endpoint to validate.</param>
		/// <exception cref="System.InvalidOperationException">RISI Must be bound to type webHttpBinding</exception>
		public void Validate(ServiceEndpoint endpoint)
		{
			var bindingElements = endpoint.Binding.CreateBindingElements();
			var webEncoder = bindingElements.Find<WebMessageEncodingBindingElement>();

			if (webEncoder == null)
			{
				throw new InvalidOperationException("RISI Must be bound to type webHttpBinding");
			}
		}
	}
}