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
 * Date: 2016-1-19
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Local policy enforcement service
    /// </summary>
    public class LocalPolicyEnforcementService : IDaemonService
    {

        // Policy tracer
        private TraceSource m_tracer = new TraceSource("OpenIZ.Core.Security.Policy");
        
        // Attached and receiving events?
        private bool m_attached = false;

        /// <summary>
        /// Gets whether the daemon is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_attached;
            }
        }

        /// <summary>
        /// Fired when the daemon has started
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Fired when the daemon is starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Fired when the daemon has stopped
        /// </summary>
        public event EventHandler Stopped;
        /// <summary>
        /// Fired when the daemon is stopping
        /// </summary>
        public event EventHandler Stopping;

        /// <summary>
        /// Start the PEP service
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            this.m_attached = true;

            this.Started?.Invoke(this, EventArgs.Empty);

            return true;
        }

        /// <summary>
        /// Stop the PEP service
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);

            this.m_attached = false;

            this.Stopped?.Invoke(this, EventArgs.Empty);

            return true;
        }
    }
}
