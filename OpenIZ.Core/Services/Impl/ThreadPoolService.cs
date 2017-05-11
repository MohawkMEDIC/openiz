using MARC.Everest.Threading;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Configuration;
using OpenIZ.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Represents a thread pool service
    /// </summary>
    public class ThreadPoolService : IDaemonService, IDisposable, IThreadPoolService
    {

        // Constructs a thread pool
        private WaitThreadPool m_threadPool = new WaitThreadPool();

        private TraceSource m_traceSource = new TraceSource(OpenIzConstants.ServiceTraceSourceName);

        /// <summary>
        /// True if the service is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_threadPool != null;
            }
        }

        // Event handlers
        public event EventHandler Started;
        public event EventHandler Starting;
        public event EventHandler Stopped;
        public event EventHandler Stopping;

        /// <summary>
        /// Dispose this thread pool
        /// </summary>
        public void Dispose()
        {
            this.m_threadPool?.Dispose();
        }

        /// <summary>
        /// Queues a non-pooled work item
        /// </summary>
        public void QueueNonPooledWorkItem(Action<object> action, object parm)
        {
            Thread thd = new Thread(new ParameterizedThreadStart(action));
            thd.IsBackground = true;
            thd.Name = $"OpenIZBackground-{action}";
            thd.Start(parm);
        }

        /// <summary>
        /// Enqueues a user work item on the master thread pool
        /// </summary>
        /// <param name="action"></param>
        public void QueueUserWorkItem(Action<object> action)
        {
            this.m_threadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    action(o);
                }
                catch (Exception e)
                {
                    this.m_traceSource.TraceError("THREAD DEATH: {0}", e);
                }
            });
        }

        /// <summary>
        /// Queue user work item
        /// </summary>
        public void QueueUserWorkItem(Action<object> action, object parm)
        {
            this.m_threadPool.QueueUserWorkItem((o) => {
                try
                {
                    action(o);
                }
                catch (Exception e)
                {
                    this.m_traceSource.TraceError("THREAD DEATH: {0}", e);

                }
            }, parm);
        }

        /// <summary>
        /// Queue user work item
        /// </summary>
        public void QueueUserWorkItem(TimeSpan timeout, Action<object> action, object parm)
        {
            // Use timer service if it is available
            new Timer((o) => {
                try
                {
                    action(o);
                }
                catch (Exception e)
                {
                    this.m_traceSource.TraceError("THREAD DEATH: {0}", e);

                }
            }, parm, (int)timeout.TotalMilliseconds, Timeout.Infinite);
        }

        /// <summary>
        /// Start
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            int concurrency = (ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.core") as OpenIzConfiguration)?.ThreadPoolSize ?? Environment.ProcessorCount;
            if (this.m_threadPool != null)
                this.m_threadPool.Dispose();
            this.m_threadPool = new WaitThreadPool(concurrency);

            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Stop the service
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);
            this.m_threadPool.Dispose();
            this.m_threadPool = null;
            this.Stopped?.Invoke(this, EventArgs.Empty);

            return true;
        }
    }
}
