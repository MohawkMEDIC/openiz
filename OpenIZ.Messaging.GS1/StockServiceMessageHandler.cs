using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Messaging.GS1.Wcf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.GS1
{
    /// <summary>
    /// Stock service message handler
    /// </summary>
    public class StockServiceMessageHandler : IMessageHandlerService
    {


        // IMSI Trace host
        private TraceSource m_traceSource = new TraceSource("OpenIZ.Messaging.StockService");

        // web host
        private WebServiceHost m_webHost;

        /// <summary>
        /// True if running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_webHost?.State == System.ServiceModel.CommunicationState.Opened;
            }
        }

        /// <summary>
        /// Fired when the object is starting up
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Fired when the object is starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Fired when the service has stopped
        /// </summary>
        public event EventHandler Stopped;
        /// <summary>
        /// Fired when the service is stopping
        /// </summary>
        public event EventHandler Stopping;

        /// <summary>
        /// Start the service
        /// </summary>
        public bool Start()
        {
            try
            {
                this.Starting?.Invoke(this, EventArgs.Empty);

                this.m_webHost = new WebServiceHost(typeof(StockServiceBehavior));
                foreach (ServiceEndpoint endpoint in this.m_webHost.Description.Endpoints)
                {
                    this.m_traceSource.TraceInformation("Starting GS1 on {0}...", endpoint.Address);
                }
                // Start the webhost
                this.m_webHost.Open();

                this.Started?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Stop the IMSI service
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);

            if (this.m_webHost != null)
            {
                this.m_webHost.Close();
                this.m_webHost = null;
            }

            this.Stopped?.Invoke(this, EventArgs.Empty);

            return true;
        }

    }
}
