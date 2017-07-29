/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2017-7-4
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Http;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.GS1.Configuration;
using OpenIZ.Messaging.GS1.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.GS1.Transport.AS2
{
    /// <summary>
    /// GS1 Stock Integration Service
    /// </summary>
    [Description("GS1 AS2(ish) Integration Service")]
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
                    Object dq = null;
                    try
                    {
                        dq = ApplicationContext.Current.GetService<IPersistentQueueService>().Dequeue(this.m_configuration.Gs1QueueName);
                        if (dq == null) break;
                        this.SendQueueMessage(dq);
                    }
                    catch (Exception ex)
                    {
                        this.m_tracer.TraceError(">>>> !!ALERT!! >>>> Error sending message to GS1 broker. Message will be placed in dead-letter queue");
                        this.m_tracer.TraceError(ex.ToString());
                        ApplicationContext.Current.GetService<IPersistentQueueService>().Enqueue("dead", dq);
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
