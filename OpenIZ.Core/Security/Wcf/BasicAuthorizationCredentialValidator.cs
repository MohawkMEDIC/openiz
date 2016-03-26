using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Exceptions;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security.Wcf
{
    /// <summary>
    /// Basic authorization credential validator for OpenIZ basic auth
    /// </summary>
    public class BasicAuthorizationCredentialValidator : UserNamePasswordValidator
    {

        // Security trace source
        private TraceSource m_traceSource = new TraceSource(OpenIzConstants.SecurityTraceSourceName);

        /// <summary>
        /// Validate the username and password
        /// </summary>
        public override void Validate(string userName, string password)
        {
            try
            {
                // Validation
                var authService = ApplicationContext.Current.GetService<IIdentityProviderService>();
                var principal = authService.Authenticate(userName, password);
                if (principal == null)
                    throw new UnauthorizedRequestException("Invalid username/password", "Basic", "openiz.org", null);
            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }
    }
}
