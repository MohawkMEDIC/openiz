using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Caching.Redis.Configuration
{
    /// <summary>
    /// Represents a simple redis configuration
    /// </summary>
    public class RedisConfiguration
    {

        /// <summary>
        /// Gets the configuration connection string
        /// </summary>
        public List<String> Servers { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// Password to the server
        /// </summary>
        public String Password { get; set; }
    }
}
