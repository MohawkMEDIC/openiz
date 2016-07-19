using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenIZ.Caching.Memory
{
    /// <summary>
    /// Represents a cache entry
    /// </summary>
    public struct CacheEntry
    {

        // Last read time
        private long m_lastReadTime;

        /// <summary>
        /// Creates a new cache entry
        /// </summary>
        public CacheEntry(DateTime loadTime, Object data)
        {
            this.m_lastReadTime = this.LastUpdateTime = loadTime.Ticks;
            this.Data = data;
        }

        /// <summary>
        /// The time that the item was loaded
        /// </summary>
        public long LastUpdateTime { get; set; }

        /// <summary>
        /// Last read time
        /// </summary>
        public long LastReadTime { get { return this.m_lastReadTime; } set { this.m_lastReadTime = value; } }

        /// <summary>
        /// The data that was loaded
        /// </summary>
        public Object Data { get; set; }

        /// <summary>
        /// Touches the cache entry
        /// </summary>
        internal void Touch()
        {
            Interlocked.Exchange(ref m_lastReadTime, DateTime.Now.Ticks);
        }
    }
}
