using MARC.HI.EHRS.SVC.Core.Timer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OpenIZ.Caching.Memory
{
    /// <summary>
    /// Timer job that cleans up cache
    /// </summary>
    internal class CacheCleanupTimerJob : ITimerJob
    {
        /// <summary>
        /// Elapsed
        /// </summary>
        public void Elapsed(object sender, ElapsedEventArgs e)
        {
            MemoryCache.Current.Clean();
        }
    }
}
