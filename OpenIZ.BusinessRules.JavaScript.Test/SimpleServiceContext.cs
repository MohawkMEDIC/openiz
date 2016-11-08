using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.BusinessRules.JavaScript.Test
{
    /// <summary>
    /// Represents a simple service provider
    /// </summary>
    public class SimpleServiceContext : IServiceProvider, IServiceManager
    {

        /// <summary>
        /// Represents a simple service provder
        /// </summary>
        private List<Object> m_services = new List<object>();

        /// <summary>
        /// CTOR adds itself to provider
        /// </summary>
        public SimpleServiceContext()
        {
            this.m_services.Add(this);
        }

        /// <summary>
        /// Add a service provider class
        /// </summary>
        public void AddServiceProvider(Type serviceType)
        {
            this.m_services.Add(Activator.CreateInstance(serviceType));
        }

        /// <summary>
        /// Service type
        /// </summary>
        public object GetService(Type serviceType)
        {
            return this.m_services.FirstOrDefault(o => serviceType.IsAssignableFrom(o.GetType()));
        }
    }
}
