using System;
using System.Collections.Generic;

namespace OpenIZ.Caching.Memory.Configuration
{
    /// <summary>
    /// Represents type cache configuration
    /// </summary>
    public class TypeCacheConfigurationInfo
    {

        /// <summary>
        /// Type cache configuration
        /// </summary>
        public TypeCacheConfigurationInfo()
        {
            this.SeedQueries = new List<String>();
        }

        /// <summary>
        /// Gets or sets the type of cache entry
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the items in the cache
        /// </summary>
        public int MaxCacheSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum age of items in the cache
        /// </summary>
        public long MaxCacheAge { get; set; }

        /// <summary>
        /// Gets or sets the seed query data
        /// </summary>
        internal List<String> SeedQueries { get; set; }

        /// <summary>
        /// Gets or sets the value of type
        /// </summary>
        public string TypeXml {
            get { return this.Type?.AssemblyQualifiedName; }
            set
            {
                if (value == null)
                    this.Type = null;
                else
                    this.Type = Type.GetType(value);
            }
        }
    }
}