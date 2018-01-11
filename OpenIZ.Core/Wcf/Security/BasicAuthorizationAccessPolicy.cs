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
 * User: justi
 * Date: 2016-8-2
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using OpenIZ.Core.Configuration;
using OpenIZ.Core.Security.Claims;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core;
using System.ServiceModel;
using OpenIZ.Core.Services;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Wcf;

namespace OpenIZ.Core.Wcf.Security
{
    /// <summary>
    /// Basic authorization policy
    /// </summary>
    [AuthenticationSchemeDescription(AuthenticationScheme.Basic)]
    public class BasicAuthorizationAccessPolicy : IAuthorizationPolicy
    {

        // Configuration from main OpenIZ
        private OpenIzConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(OpenIzConstants.OpenIZConfigurationName) as OpenIzConfiguration;

        // Trace source
        private TraceSource m_traceSource = new TraceSource(OpenIzConstants.SecurityTraceSourceName);
        
        // Guid
        private Guid m_id = Guid.NewGuid();

        /// <summary>
        /// Gets the id of the authoziation policy
        /// </summary>
        public string Id
        {
            get
            {
                return this.m_id.ToString();
            }
        }

        /// <summary>
        /// Issuer
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
                this.m_traceSource.TraceInformation("Entering BasicAuthorizationAccessPolicy");

                object obj;
                if (!evaluationContext.Properties.TryGetValue("Identities", out obj))
                    throw new Exception("No Identity found");
                IList<IIdentity> identities = obj as IList<IIdentity>;
                if (identities == null || identities.Count <= 0)
                    throw new Exception("No Identity found");

                // Role service
                var roleService = ApplicationContext.Current.GetService<IRoleProviderService>();
                var pipService = ApplicationContext.Current.GetService<IPolicyInformationService>();

                // Claims
                var roles = roleService.GetAllRoles(identities[0].Name);
                List<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>(
                    roles.Select(o => new System.Security.Claims.Claim(ClaimsIdentity.DefaultRoleClaimType, o))
                    );

                // Add claims made by the client
                HttpRequestMessageProperty httpRequest = (HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name];
                if (httpRequest != null)
                {
                    var clientClaims = OpenIzClaimTypes.ExtractClaims(httpRequest.Headers);
                    foreach (var claim in clientClaims)
                    {
                        if (this.m_configuration?.Security?.BasicAuth?.AllowedClientClaims?.Contains(claim.Type) == false)
                            throw new SecurityException(ApplicationContext.Current.GetLocaleString("SECE001"));
                        else
                        {
                            var handler = OpenIzClaimTypes.GetHandler(claim.Type);
                            if (handler == null ||
                                handler.Validate(new GenericPrincipal(identities[0], roles), claim.Value))
                                claims.Add(claim);
                            else
                                throw new SecurityException(ApplicationContext.Current.GetLocaleString("SECE002"));
                        }
                    }
                }

                // Claim headers built in
                if (pipService != null)
                    claims.AddRange(pipService.GetActivePolicies(identities[0]).Where(o => o.Rule == PolicyDecisionOutcomeType.Grant).Select(o => new System.Security.Claims.Claim(OpenIzClaimTypes.OpenIzGrantedPolicyClaim, o.Policy.Oid)));

                // Finally validate the client 
                if (this.m_configuration?.Security?.BasicAuth?.RequireClientAuth == true)
                {
                    var clientAuth = httpRequest.Headers[OpenIzConstants.BasicHttpClientCredentialHeaderName];
                    if (clientAuth == null ||
                        !clientAuth.StartsWith("basic", StringComparison.InvariantCultureIgnoreCase))
                        throw new SecurityException("Client credentials invalid");
                    else
                    {
                        String clientAuthString = clientAuth.Substring(clientAuth.IndexOf("basic", StringComparison.InvariantCultureIgnoreCase) + 5).Trim();
                        String[] authComps = Encoding.UTF8.GetString(Convert.FromBase64String(clientAuthString)).Split(':');
                        var applicationPrincipal = ApplicationContext.Current.GetApplicationProviderService().Authenticate(authComps[0], authComps[1]);
                        claims.Add(new System.Security.Claims.Claim(OpenIzClaimTypes.OpenIzApplicationIdentifierClaim, applicationPrincipal.Identity.Name));
                    }
                }
                var principal = new ClaimsPrincipal(new ClaimsIdentity(identities[0], claims));
                
                evaluationContext.Properties["Principal"] = principal;

                AuthenticationContext.Current = new AuthenticationContext(principal); // Set Authentication context

                return true;
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                return false;
            }
        }
    }
}
