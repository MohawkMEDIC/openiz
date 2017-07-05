using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Http;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.GS1.Configuration;
using OpenIZ.Messaging.GS1.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.GS1.Transport.AS2
{
    /// <summary>
    /// GS1 Stock Integration Service
    /// </summary>
    public class As2IntegrationService : IDaemonService
    {

        // The event handler
        private EventHandler<PersistentQueueEventArgs> m_handler;

        // Tracer
        private TraceSource m_tracer = new TraceSource("OpenIZ.Messaging.GS1");

        // Configuration
        private Gs1ConfigurationSection m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.messaging.gs1") as Gs1ConfigurationSection;


        /// <summary>
        /// True when the service is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_handler != null;
            }
        }

        // Events for daemon service
        public event EventHandler Started;
        public event EventHandler Starting;
        public event EventHandler Stopped;
        public event EventHandler Stopping;

        /// <summary>
        /// Start the daemon service
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            // Create handler
            this.m_handler = (o, e) =>
            {
                do
                {
                    try
                    {
                        var dq = ApplicationContext.Current.GetService<IPersistentQueueService>().Dequeue(this.m_configuration.Gs1QueueName);
                        if (dq == null) break;
                        this.SendQueueMessage(dq);
                    }
                    catch (Exception ex)
                    {
                        this.m_tracer.TraceError(">>>> !!ALERT!! >>>> Error sending message to GS1 broker. All further communications with the broker will be suspended");
                        this.m_tracer.TraceError(ex.ToString());
                        this.Stop();
                        break;
                    }
                } while (true);
            };

            // Queue Handler
            ApplicationContext.Current.Started += (o, e) =>
            {
                ApplicationContext.Current.GetService<IPersistentQueueService>().Queued += this.m_handler;
                ApplicationContext.Current.GetService<IPersistentQueueService>().Open(this.m_configuration.Gs1QueueName);

            };

            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Get the message type and URL endpoint
        /// </summary>
        private void SendQueueMessage(object queueMessage)
        {
            try
            {
                this.m_tracer.TraceInfo("Dispatching message {0} to GS1 endpoint", queueMessage.GetType().Name);
                // First, we're going to create a rest client
                var restClient = new RestClient(this.m_configuration.Gs1BrokerAddress);
                var client = new Gs1ServiceClient(restClient);

                if (queueMessage is OrderMessageType)
                    client.IssueOrder(queueMessage as OrderMessageType);
                else if (queueMessage is DespatchAdviceMessageType)
                    client.IssueDespatchAdvice(queueMessage as DespatchAdviceMessageType);
                else if (queueMessage is ReceivingAdviceMessageType)
                    client.IssueReceivingAdvice(queueMessage as ReceivingAdviceMessageType);
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Could not dispatch message to GS1 endpoint: {0}", e);
                ApplicationContext.Current.GetService<IPersistentQueueService>().Enqueue(this.m_configuration.Gs1QueueName, queueMessage);
                throw;
            }
        }

        /// <summary>
        /// Stop the current service
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);

            ApplicationContext.Current.GetService<IPersistentQueueService>().Queued -= this.m_handler;
            this.m_handler = null;

            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
