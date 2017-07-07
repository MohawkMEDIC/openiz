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
 * User: justi
 * Date: 2016-11-30
 */
using OpenIZ.Core.Http.Description;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Http
{
    /// <summary>
    /// Represents a rest client description
    /// </summary>
    public class ServiceClientDescription : ConfigurationElement, IRestClientDescription
    {

        /// <summary>
        /// Gets or sets the service binding
        /// </summary>
        [ConfigurationProperty("binding")]
        public ServiceClientBindingDescription Binding {
            get { return (ServiceClientBindingDescription)this["binding"]; }
            set { this["binding"] = value; }
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
        /// Gets whether a tracing is enabled.
        /// </summary>
        [ConfigurationProperty("enableTracing")]
        public bool Trace {
            get { return (bool)this["enableTracing"]; }
            set { this["enableTracing"] = value; }
        }

        /// <summary>
        /// Gets or sets the endpoints
        /// </summary>
        public List<IRestClientEndpointDescription> Endpoint
        {
            get
            {
                return this.EndpointCollection.OfType<IRestClientEndpointDescription>().ToList();
            }
        }

        /// <summary>
        /// Endpoint collection for configuration
        /// </summary>
        [ConfigurationProperty("endpoint")]
        [ConfigurationCollection(typeof(ServiceClientEndpointDescription),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ServiceClientEndpointCollection EndpointCollection {
            get { return (ServiceClientEndpointCollection)this["endpoint"]; }
            set { this["endpoint"] = value; }

        }
    }


    /// <summary>
    /// Represents a collection service client endpoints
    /// </summary>
    public class ServiceClientEndpointCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Create new element
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceClientEndpointDescription();
        }

        /// <summary>
        /// Get element key
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceClientEndpointDescription)element).Address;
        }
    }

    /// <summary>
    /// Represents a service client description
    /// </summary>
    public class ServiceClientEndpointDescription : ConfigurationElement, IRestClientEndpointDescription
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public ServiceClientEndpointDescription()
        {

        }

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
        [ConfigurationProperty("address")]
        public string Address {
            get { return (string)this["address"]; }
            set { this["address"] = value; }
        }

        /// <summary>
        /// Gets or sets the timeout
        /// </summary>
        [ConfigurationProperty("timeout")]
        public int Timeout
        {
            get { return (int)this["timeout"]; }
            set { this["timeout"] = value; }
        }
    }

    /// <summary>
    /// REST client binding description
    /// </summary>
    public class ServiceClientBindingDescription : ConfigurationElement, IRestClientBindingDescription
    {
        /// <summary>
        /// Gets the content type mapper
        /// </summary>
        public IContentTypeMapper ContentTypeMapper { get { return new DefaultContentTypeMapper(); } }

        /// <summary>
        /// Gets or sets the optimization flag
        /// </summary>
        [ConfigurationProperty("optimize")]
        public bool Optimize {
            get { return (bool)this["optimize"]; }
            set { this["optimize"] = value; }
        }

        /// <summary>
        /// Gets or sets the security description
        /// </summary>
        public IRestClientSecurityDescription Security { get; set; }
    }
}
