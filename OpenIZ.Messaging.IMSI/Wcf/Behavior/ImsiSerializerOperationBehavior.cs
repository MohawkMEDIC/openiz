/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using OpenIZ.Core.Wcf.Serialization;
using OpenIZ.Messaging.IMSI.Wcf.Serialization;
using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace OpenIZ.Messaging.IMSI.Wcf.Behavior
{
    /// <summary>
    /// IMSI Serializer operation behavior
    /// </summary>
    internal class ImsiSerializerOperationBehavior : IOperationBehavior
    {
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        /// <summary>
        /// Apply the dispatch behavior
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="dispatchOperation"></param>
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Formatter = new WcfMessageDispatchFormatter<IImsiServiceContract>(operationDescription);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}