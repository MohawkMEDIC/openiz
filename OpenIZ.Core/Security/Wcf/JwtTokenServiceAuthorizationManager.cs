using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Configuration;
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

namespace OpenIZ.Core.Security.Wcf
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
            // Http message inbound
            HttpRequestMessageProperty httpMessage = (HttpRequestMessageProperty)operationContext.IncomingMessageProperties[HttpRequestMessageProperty.Name];

            // Get the authorize header
            String authorization = httpMessage.Headers[System.Net.HttpRequestHeader.Authorization];
            if (authorization == null)
                throw new UnauthorizedRequestException("Missing Authorization header", "Bearer", "openiz.org", this.m_configuration.Security.ClaimsAuth.Audiences.FirstOrDefault());
            else if (!authorization.Trim().StartsWith("bearer", StringComparison.InvariantCultureIgnoreCase))
                throw new UnauthorizedRequestException("Invalid authentication scheme", "Bearer", "openiz.org", this.m_configuration.Security.ClaimsAuth.Audiences.FirstOrDefault());

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
            AuthenticationContext.Current = new AuthenticationContext(identities);
            return base.CheckAccess(operationContext);
        }
    }
}
