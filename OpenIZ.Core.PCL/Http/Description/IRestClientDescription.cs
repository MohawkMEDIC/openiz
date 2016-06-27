using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.PCL.Http.Description
{
    /// <summary>
    /// Represents a description of a service
    /// </summary>
    public interface IRestClientDescription
    {
        /// <summary>
        /// Gets or sets the endpoints for the client
        /// </summary>
        List<IRestClientEndpointDescription> Endpoint { get; }

        /// <summary>
        /// Gets or sets the binding for the service client.
        /// </summary>
        IRestClientBindingDescription Binding { get; }
    }

}
