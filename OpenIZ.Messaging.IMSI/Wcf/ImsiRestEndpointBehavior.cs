using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.IMSI.Wcf
{
    /// <summary>
    /// IMSI REST Endpoint Behavior
    /// </summary>
    public class ImsiRestEndpointBehavior : IEndpointBehavior
    {
        /// <summary>
        /// Add binding parameter
        /// </summary>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Add client behavior
        /// </summary>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        /// <summary>
        /// Apply a dispatcher
        /// </summary>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new ImsiMessageInspector());
        }

        /// <summary>
        /// Validate the endpoint
        /// </summary>
        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
