using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents a service manager
    /// </summary>
    public interface IServiceManager
    {

        /// <summary>
        /// Add the specified service provider
        /// </summary>
        void AddServiceProvider(Type serviceType);
    }
}
