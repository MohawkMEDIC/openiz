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
 * Date: 2016-11-30
 */
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Security;
using MARC.HI.EHRS.SVC.Core;
using System.Security;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Represents a default implementation of a TFA relay service which scans the entire application domain for 
    /// mechanisms and allows calling of them all
    /// </summary>
    public class DefaultTfaRelayService : ITfaRelayService
    {

        /// <summary>
        /// Construct the default relay service
        /// </summary>
        public DefaultTfaRelayService()
        {
            this.Mechanisms = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(o => typeof(ITfaMechanism).IsAssignableFrom(o) && o.IsClass).Select(o => Activator.CreateInstance(o) as ITfaMechanism));
        }

        /// <summary>
        /// Gets the configured mechanisms
        /// </summary>
        public IEnumerable<ITfaMechanism> Mechanisms
        {
            get; private set;
        }

        /// <summary>
        /// Sends the secret via the specified mechanism
        /// </summary>
        public void SendSecret(Guid mechanismId, SecurityUser user, string mechanismVerification, string tfaSecret)
        {
            // Get the mechanism
            var mechanism = this.Mechanisms.FirstOrDefault(o => o.Id == mechanismId);
            if (mechanism == null)
                throw new SecurityException($"TFA mechanism {mechanismId} not found");

            // send the secret
            mechanism.Send(user, mechanismVerification, tfaSecret);
        }
    }
}
