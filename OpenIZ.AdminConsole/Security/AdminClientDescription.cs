using System;
using System.Collections.Generic;
using OpenIZ.Core.Http.Description;
using OpenIZ.Core.Http;

namespace OpenIZ.AdminConsole.Security
{
    /// <summary>
    /// Administrative client description
    /// </summary>
    public class AdminClientDescription : IRestClientDescription
    {

        // Endpoints
        private List<IRestClientEndpointDescription> m_endpoints = new List<IRestClientEndpointDescription>();

        /// <summary>
        /// Represents client description
        /// </summary>
        public AdminClientDescription()
        {
            
        }

        /// <summary>
        /// Bidning description
        /// </summary>
        public ServiceClientBindingDescription Binding
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the binding description
        /// </summary>
        IRestClientBindingDescription IRestClientDescription.Binding
        {
            get
            {
                return this.Binding;
            }
        }

        /// <summary>
        /// Gets the list of endpoints
        /// </summary>
        public List<IRestClientEndpointDescription> Endpoint
        {
            get
            {
                return this.m_endpoints;
            }
        }


        /// <summary>
        /// Trace?
        /// </summary>
        public bool Trace
        {
            get
            {
                return false;
            }
        }
    }
}