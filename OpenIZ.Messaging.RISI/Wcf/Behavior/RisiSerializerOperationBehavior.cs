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

using OpenIZ.Core.Wcf.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace OpenIZ.Messaging.RISI.Wcf.Behavior
{
	/// <summary>
	/// Represents serialization operation behavior for the RISI interface.
	/// </summary>
	internal class RisiSerializerOperationBehavior : IOperationBehavior
	{
		/// <summary>
		/// Implement to pass data at runtime to bindings to support custom behavior.
		/// </summary>
		/// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
		/// <param name="bindingParameters">The collection of objects that binding elements require to support the behavior.</param>
		public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
		{
		}

		/// <summary>
		/// Implements a modification or extension of the client across an operation.
		/// </summary>
		/// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
		/// <param name="clientOperation">The run-time object that exposes customization properties for the operation described by <paramref name="operationDescription" />.</param>
		public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
		{
		}

		/// <summary>
		/// Apply the dispatch behavior.
		/// </summary>
		/// <param name="operationDescription">The operation description.</param>
		/// <param name="dispatchOperation">The dispatch description.</param>
		public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
		{
			dispatchOperation.Formatter = new WcfMessageDispatchFormatter<IRisiContract>(operationDescription);
		}

		/// <summary>
		/// Implement to confirm that the operation meets some intended criteria.
		/// </summary>
		/// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
		public void Validate(OperationDescription operationDescription)
		{
		}
	}
}