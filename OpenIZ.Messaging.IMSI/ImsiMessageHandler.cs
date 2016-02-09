/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-24
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Messaging.IMSI.Configuration;
using OpenIZ.Messaging.IMSI.Wcf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.IMSI
{
    /// <summary>
    /// The IMSI Message Handler Daemon class
    /// </summary>
    public class ImsiMessageHandler : IMessageHandlerService
    {

        // IMSI Trace host
        private TraceSource m_traceSource = new TraceSource("OpenIZ.Messaging.IMSI");

        // configuration
        private ImsiConfiguration m_configuration= ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.messaging.imsi") as ImsiConfiguration;

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

                this.m_webHost = new WebServiceHost(typeof(ImsiServiceBehavior));

                foreach(var endpoint in this.m_webHost.Description.Endpoints)
                {
                    this.m_traceSource.TraceInformation("Starting IMSI on {0}...", endpoint.Address);
                    (endpoint.Binding as WebHttpBinding).ContentTypeMapper = new ImsiContentTypeHandler();
                    endpoint.EndpointBehaviors.Add(new ImsiRestEndpointBehavior());
                }

                // Start the webhost
                this.m_webHost.Open();

                this.Started?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch(Exception e)
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

            if(this.m_webHost != null)
            {
                this.m_webHost.Close();
                this.m_webHost = null;
            }

            this.Stopped?.Invoke(this, EventArgs.Empty);

            return true;
        }
    }
}
