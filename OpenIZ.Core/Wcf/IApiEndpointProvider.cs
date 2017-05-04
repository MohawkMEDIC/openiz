using OpenIZ.Core.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Wcf
{
    /// <summary>
    /// Represents an OpenIZ API endpoint
    /// </summary>
    public interface IApiEndpointProvider
    {
        /// <summary>
        /// Gets the service type
        /// </summary>
        ServiceEndpointType ApiType { get; }

        /// <summary>
        /// Service URL
        /// </summary>
        String[] Url { get; }
    }
}
