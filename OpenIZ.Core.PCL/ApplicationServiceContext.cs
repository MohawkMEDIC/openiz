using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core
{
    /// <summary>
    /// Application context
    /// </summary>
    public static class ApplicationServiceContext 
    {

        /// <summary>
        /// Gets or sets the current application service context
        /// </summary>
        public static IServiceProvider Current { get; set; }

    }
}
