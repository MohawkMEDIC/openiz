using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.BusinessRules.JavaScript.JNI;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenIZ.BusinessRules.JavaScript
{
    /// <summary>
    /// Represents a pool of JavaScript engine threads which allows for
    /// multi-threading of javascript execution
    /// </summary>
    public class JavascriptEnginePool  : IDisposable
    {

        // True if the pools 
        private Jint.Engine[] m_engines = null;

        /// <summary>
        /// Worker data structure
        /// </summary>
        private struct WorkItem
        {
            /// <summary>
            /// The callback to execute on the worker
            /// </summary>
            public Action<Object> Callback { get; set; }
            /// <summary>
            /// The state or parameter to the worker
            /// </summary>
            public object State { get; set; }
        }

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(JavascriptEnginePool));
        /// <summary>
        /// Queue of work items to be finished
        /// </summary>
        private Queue<WorkItem> m_workItems = new Queue<WorkItem>();
        // Number of remaining work items
        private int m_remainingWorkItems = 1;
        // Thread is done reset event
        private ManualResetEvent m_threadDoneResetEvent = new ManualResetEvent(false);
        // Hint of the number of threads waiting to be executed
        private int m_threadWait = 0;

        /// <summary>
        /// Start the process
        /// </summary>
        public JavascriptEnginePool(BusinessRulesBridge bridge)
        {
            this.m_engines = new Jint.Engine[Environment.ProcessorCount];
            for (int i = 0; i < this.m_engines.Length; i++)
                this.m_engines[i] = new Jint.Engine(cfg => cfg.AllowClr(
                   typeof(OpenIZ.Core.Model.BaseEntityData).GetTypeInfo().Assembly,
                   typeof(IBusinessRulesService<>).GetTypeInfo().Assembly
               )
               .Strict(false)
#if DEBUG
                .DebugMode(true)
#endif

                ).SetValue("OpenIZBre", bridge)
                .SetValue("console", new JsConsoleProvider());

        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }
    }
}
