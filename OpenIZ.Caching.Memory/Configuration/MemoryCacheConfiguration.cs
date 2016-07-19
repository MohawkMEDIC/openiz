using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Caching.Memory.Configuration
{
    /// <summary>
    /// Create memory cache configuration
    /// </summary>
    public class MemoryCacheConfiguration
    {

        /// <summary>
        /// Memory type configuration
        /// </summary>
        public MemoryCacheConfiguration()
        {
            this.Types = new List<TypeCacheConfigurationInfo>();
        }

        /// <summary>
        /// Autosubscribe types
        /// </summary>
        public bool AutoSubscribeTypes { get; set; }

        /// <summary>
        /// Type cache configuration information 
        /// </summary>
        public List<TypeCacheConfigurationInfo> Types { get; set; }

    }
}
