using OpenIZ.Core.Http.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Http
{
    /// <summary>
    /// Represents a rest client description
    /// </summary>
    public class ServiceClientDescription : IRestClientDescription
    {
        /// <summary>
        /// Gets or sets the binding description
        /// </summary>
        public IRestClientBindingDescription Binding { get; set; }

        /// <summary>
        /// Gets or sets the endpoints
        /// </summary>
        public List<IRestClientEndpointDescription> Endpoint { get; set; }
    }

    /// <summary>
    /// Represents a service client description
    /// </summary>
    public class ServiceClientEndpointDescription : IRestClientEndpointDescription
    {

        /// <summary>
        /// Service client endpoint description
        /// </summary>
        public ServiceClientEndpointDescription(String address)
        {
            this.Address = address;
            this.Timeout = 10000;
        }

        /// <summary>
        /// Gets or sets the address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the timeout
        /// </summary>
        public int Timeout { get; set; }
    }

    /// <summary>
    /// REST client binding description
    /// </summary>
    public class ServiceClientBindingDescription : IRestClientBindingDescription
    {
        /// <summary>
        /// Gets the content type mapper
        /// </summary>
        public IContentTypeMapper ContentTypeMapper { get { return new DefaultContentTypeMapper(); } }

        /// <summary>
        /// Gets or sets the optimization flag
        /// </summary>
        public bool Optimize { get; set; }

        /// <summary>
        /// Gets or sets the security description
        /// </summary>
        public IRestClientSecurityDescription Security { get; set; }
    }
}
