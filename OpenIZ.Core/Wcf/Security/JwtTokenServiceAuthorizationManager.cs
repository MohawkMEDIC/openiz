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
 * Date: 2016-4-19
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Exceptions;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Configuration;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IdentityModel.Configuration;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Authentication;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Wcf.Security
{
    /// <summary>
    /// JwtToken SAM
    /// </summary>
    public class JwtTokenServiceAuthorizationManager : ServiceAuthorizationManager
    {

        // Configuration from main OpenIZ
        private OpenIzConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(OpenIzConstants.OpenIZConfigurationName) as OpenIzConfiguration;

        // Trace source
        private TraceSource m_traceSource = new TraceSource(OpenIzConstants.SecurityTraceSourceName);

        /// <summary>
        /// Check access
        /// </summary>
        public override bool CheckAccess(OperationContext operationContext)
        {
            try
            {
                // Http message inbound
                HttpRequestMessageProperty httpMessage = (HttpRequestMessageProperty)operationContext.IncomingMessageProperties[HttpRequestMessageProperty.Name];

                // Get the authorize header
                String authorization = httpMessage.Headers[System.Net.HttpRequestHeader.Authorization];
                if (authorization == null)
                    throw new UnauthorizedRequestException("Missing Authorization header", "Bearer", this.m_configuration.Security.ClaimsAuth.Realm, this.m_configuration.Security.ClaimsAuth.Audiences.FirstOrDefault());
                else if (!authorization.Trim().StartsWith("bearer", StringComparison.InvariantCultureIgnoreCase))
                    throw new UnauthorizedRequestException("Invalid authentication scheme", "Bearer", this.m_configuration.Security.ClaimsAuth.Realm, this.m_configuration.Security.ClaimsAuth.Audiences.FirstOrDefault());

                String authorizationToken = authorization.Substring(6).Trim();
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

                var identityModelConfig = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("system.identityModel") as SystemIdentityModelSection;

                if (!handler.CanReadToken(authorizationToken))
                    throw new SecurityTokenException("Token is not in a vlaid format");

                SecurityToken token = null;
                var identities = handler.ValidateToken(authorizationToken, this.m_configuration?.Security?.ClaimsAuth?.ToConfigurationObject(), out token);

                // Validate token expiry
                if (token.ValidTo < DateTime.Now.ToUniversalTime())
                    throw new SecurityTokenException("Token expired");
                else if (token.ValidFrom > DateTime.Now.ToUniversalTime())
                    throw new SecurityTokenException("Token not yet valid");

                operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Identities"] = identities.Identities;
                operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] = identities;
                Core.Security.AuthenticationContext.Current = new Core.Security.AuthenticationContext(identities);
                return base.CheckAccess(operationContext);
            }
            catch(UnauthorizedAccessException) { throw; }
            catch(UnauthorizedRequestException) { throw; }
            catch(Exception e)
            {
                throw new SecurityTokenException(e.Message);
            }
        }
    }
}
