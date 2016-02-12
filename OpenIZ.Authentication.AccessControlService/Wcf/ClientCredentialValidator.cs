using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Authentication.OAuth2.Wcf
{
    /// <summary>
    /// Username & Password validator which will validate the client BASIC auth headers
    /// </summary>
    public class ClientCredentialValidator : UserNamePasswordValidator
    {

        // Trace source
        private TraceSource m_traceSource = new TraceSource(OAuthConstants.TraceSourceName);

        /// <summary>
        /// Validate the specified username and password
        /// </summary>
        public override void Validate(string userName, string password)
        {
            try
            {
                this.m_traceSource.TraceInformation("Entering OAuth2.Wcf.ClientCredentialValidator");
                IApplicationIdentityProviderService clientIdentityService = ApplicationContext.Current.GetService<IApplicationIdentityProviderService>();
                // attempt to validate
                var auth = clientIdentityService.Authenticate(userName, password);
                if (auth == null)
                    throw new FaultException("Non-valid client");

            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }
    }
}
