using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Caching.Redis.Configuration
{
    /// <summary>
    /// REDIS Configuration Section
    /// </summary>
    public class RedisConfigurationHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create the configuration section
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {
            // REDIS configuration
            return new RedisConfiguration()
            {
                Servers = section.SelectNodes("./server/add").OfType<XmlElement>().Select(o=>String.Format("{0}:{1}", o.Attributes["host"]?.Value ?? "localhost", o.Attributes["port"]?.Value ?? "6379")).ToList(),
                UserName = section.SelectSingleNode("./authentication@userName")?.Value,
                Password = section.SelectSingleNode("./authentication@password")?.Value
            };
        }
    }
}
