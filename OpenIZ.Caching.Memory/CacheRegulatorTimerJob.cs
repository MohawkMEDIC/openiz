using System;
using System.Timers;
using MARC.HI.EHRS.SVC.Core.Timer;

namespace OpenIZ.Caching.Memory
{
    /// <summary>
    /// Timer job that reduces pressure on the cache
    /// </summary>
    internal class CacheRegulatorTimerJob : ITimerJob
    {
        /// <summary>
        /// Timer has elapsed
        /// </summary>
        public void Elapsed(object sender, ElapsedEventArgs e)
        {
            MemoryCache.Current.ReducePressure();
        }
    }
}