/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Authentication.OAuth2.Wcf
{
    /// <summary>
    /// Authorization policy
    /// </summary>
    public class ClientAuthorizationPolicy : IAuthorizationPolicy
    {
        // ID
        private readonly Guid m_id = Guid.NewGuid();

        // Trace source
        private TraceSource m_traceSource = new TraceSource(OAuthConstants.TraceSourceName);

        /// <summary>
        /// Return ID
        /// </summary>
        public string Id
        {
            get
            {
                return this.m_id.ToString();
            }
        }
        
        /// <summary>
        /// Issuer of the claim
        /// </summary>
        public ClaimSet Issuer
        {
            get
            {
                return ClaimSet.System;
            }
        }

        /// <summary>
        /// Evaluate the context
        /// </summary>
        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {

            try
            {
                this.m_traceSource.TraceInformation("Entering OAuth2.Wcf.ClientAuthorizationPolicy");

                object obj;
                if (!evaluationContext.Properties.TryGetValue("Identities", out obj))
                    throw new Exception("No Identity found");
                IList<IIdentity> identities = obj as IList<IIdentity>;
                if (identities == null || identities.Count <= 0)
                    throw new Exception("No Identity found");

                // Add SID claim
                var principal = new GenericPrincipal(identities[0], null);
                var applicationProvider = ApplicationContext.Current.GetService<IApplicationIdentityProviderService>();
                var applicationPrincipal = applicationProvider.GetIdentity(principal.Identity.Name);
                principal.AddIdentity(applicationPrincipal as Core.Security.ApplicationIdentity);
                evaluationContext.Properties["Principal"] = principal;
                return true;
            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }
    }
}
