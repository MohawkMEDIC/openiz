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
 * Date: 2016-8-2
 */

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace OpenIZ.Messaging.AMI.Wcf.Behavior
{
	/// <summary>
	/// Represents the endpoint behavior for the AMI endpoint.
	/// </summary>
	public class AmiRestEndpointBehavior : IEndpointBehavior
	{
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

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
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new AmiMessageInspector());

			// Apply to each operation the AMI formatter
			foreach (var op in endpoint.Contract.Operations)
			{
				op.OperationBehaviors.Add(new AmiSerializerOperationBehavior());
			}
		}

		public void Validate(ServiceEndpoint endpoint)
		{
			var bindingElements = endpoint.Binding.CreateBindingElements();
			var webEncoder = bindingElements.Find<WebMessageEncodingBindingElement>();

			if (webEncoder == null)
			{
				throw new InvalidOperationException("AMI Must be bound to type webHttpBinding");
			}
		}
	}
}