using System;
using OpenIZ.Core.Http.Description;

namespace OpenIZ.AdminConsole.Shell
{
    internal class AdminClientEndpointDescription : IRestClientEndpointDescription
    {

        /// <summary>
        /// Host of the endpoint
        /// </summary>
        public AdminClientEndpointDescription(String host)
        {
            this.Address = host;
            this.Timeout = 20000;
        }

        /// <summary>
        /// Gets or sets the address
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Gets or sets the timeout
        /// </summary>
        public int Timeout { get; set; }
    }
}