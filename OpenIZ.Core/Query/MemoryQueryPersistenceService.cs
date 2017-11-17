using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Data;
using System.Diagnostics;
using OpenIZ.Core.Diagnostics;
using MARC.HI.EHRS.SVC.Core.Timer;
using System.Timers;
using MARC.HI.EHRS.SVC.Core;

namespace OpenIZ.Core.Query
{


    /// <summary>
    /// Represents a simple query persistence service that uses local memory for query continuation
    /// </summary>
    public class MemoryQueryPersistenceService : IQueryPersistenceService, ITimerJob, IDaemonService
    {

        /// <summary>
        /// Memory based query information
        /// </summary>
        public class MemoryQueryInfo
        {
            /// <summary>
            /// Query info ctor
            /// </summary>
            public MemoryQueryInfo()
            {
                this.CreationTime = DateTime.Now;
            }

            /// <summary>
            /// Total results
            /// </summary>
            public int TotalResults { get; set; }

            /// <summary>
            /// Results in the result set
            /// </summary>
            public List<Object> Results { get; set; }

            /// <summary>
            /// The query tag
            /// </summary>
            public object QueryTag { get; set; }

            /// <summary>
            /// Get or sets the creation time
            /// </summary>
            public DateTime CreationTime { get; private set; }
        }

        // Tracer
        private TraceSource m_tracer = new TraceSource("OpenIZ.Core.Query.MemoryQueryPersistence");

        // Memory cache of queries
        private Dictionary<String, MemoryQueryInfo> m_queryCache = new Dictionary<String, MemoryQueryInfo>(10);

        // Sync object
        private Object m_syncObject = new object();

        public event EventHandler Starting;
        public event EventHandler Stopping;
        public event EventHandler Started;
        public event EventHandler Stopped;

        public bool IsRunning
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Add results to id set
        /// </summary>
        public bool AddResults<TIdentifier>(string queryId, Identifier<TIdentifier>[] results)
        {
            MemoryQueryInfo retVal = null;
            if (this.m_queryCache.TryGetValue(queryId, out retVal))
            {
                this.m_tracer.TraceVerbose("Updating query {0} ({1} results)", queryId, results.Count());
                lock (retVal.Results)
                    retVal.Results.AddRange(results.Where(o => !retVal.Results.Contains(o.Id)).Select(o => o.Id).OfType<Object>());

                retVal.TotalResults = retVal.Results.Count();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Get query results
        /// </summary>
        public Identifier<TIdentifier>[] GetQueryResults<TIdentifier>(string queryId, int startRecord, int nRecords)
        {
            MemoryQueryInfo retVal = null;
            if (this.m_queryCache.TryGetValue(queryId, out retVal))
                return retVal.Results.ToArray().Distinct().Skip(startRecord).Take(nRecords).Select(o => new Identifier<Guid>((Guid)o)).OfType<Identifier<TIdentifier>>().ToArray();
            return null;
        }

        /// <summary>
        /// Get query tag
        /// </summary>
        public object GetQueryTag(string queryId)
        {
            MemoryQueryInfo retVal = null;
            if (this.m_queryCache.TryGetValue(queryId, out retVal))
                return retVal.QueryTag;
            return null;
        }

        /// <summary>
        /// True if registered
        /// </summary>
        public bool IsRegistered(string queryId)
        {
            return this.m_queryCache.ContainsKey(queryId);
        }

        /// <summary>
        /// Get total results
        /// </summary>
        public long QueryResultTotalQuantity(string queryId)
        {
            MemoryQueryInfo retVal = null;
            if (this.m_queryCache.TryGetValue(queryId, out retVal))
                return retVal.TotalResults;
            return 0;
        }

        /// <summary>
        /// Register a query
        /// </summary>
        public bool RegisterQuerySet<TIdentifier>(string queryId, int count, Identifier<TIdentifier>[] results, object tag)
        {
            lock (this.m_syncObject)
            {
                MemoryQueryInfo retVal = null;
                if (this.m_queryCache.TryGetValue(queryId, out retVal))
                {
                    this.m_tracer.TraceVerbose("Updating query {0} ({1} results)", queryId, results.Count());
                    retVal.Results = results.Select(o => o.Id).OfType<Object>().ToList();
                    retVal.QueryTag = tag;
                }
                else
                {
                    this.m_tracer.TraceVerbose("Registering query {0} ({1} results)", queryId, results.Count());

                    this.m_queryCache.Add(queryId, new MemoryQueryInfo()
                    {
                        QueryTag = tag,
                        Results = results.Select(o => o.Id).OfType<Object>().ToList(),
                        TotalResults = results.ToArray().Count()
                    });
                }
            }
            return true;
        }

        /// <summary>
        /// Start the service
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            ApplicationContext.Current.Started += (o, e) =>
            {
                var timerService = ApplicationContext.Current.GetService<ITimerService>();
                if (!timerService.IsJobRegistered(typeof(MemoryQueryPersistenceService)))
                    timerService.AddJob(this, new TimeSpan(4, 0, 0));
            };

            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Stop the service
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);
            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Timer has elapsed
        /// </summary>
        public void Elapsed(object sender, ElapsedEventArgs e)
        {
#if DEBUG
            this.m_tracer.TraceInformation("Cleaning stale queries from memory...");
#endif

            try
            {
                lock (this.m_syncObject)
                {
                    DateTime now = DateTime.Now;
                    var garbageBin = this.m_queryCache.Where(o => now.Subtract(o.Value.CreationTime).TotalMinutes == 30).Select(o => o.Key);
                    foreach (var itm in garbageBin)
                        this.m_queryCache.Remove(itm);// todo configuration
                }
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError(ex.ToString());
            }
        }
    }
}
