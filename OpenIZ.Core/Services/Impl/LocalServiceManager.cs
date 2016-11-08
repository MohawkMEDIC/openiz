using MARC.HI.EHRS.SVC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Local service manager
    /// </summary>
    public class LocalServiceManager : IServiceManager
    {
        /// <summary>
        /// Add service provider
        /// </summary>
        public void AddServiceProvider(Type serviceType)
        {
            ApplicationContext.Current.AddServiceProvider(serviceType);
        }
    }
}
