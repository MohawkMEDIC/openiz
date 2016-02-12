using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Diagnostics;
using OpenIZ.Persistence.Data.MSSQL.Configuration;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Event;
using System.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Services;

namespace OpenIZ.Persistence.Data.MSSQL.Services
{
    /// <summary>
    /// Sql Application IdP
    /// </summary>
    public class SqlApplicationIdentityProvider : IApplicationIdentityProviderService
    {
        // Trace source
        private TraceSource m_traceSource = new TraceSource(SqlServerConstants.IdentityTraceSourceName);

        // Configuration
        private SqlConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(SqlServerConstants.ConfigurationSectionName) as SqlConfiguration;

        /// <summary>
        /// Fired prior to an authentication request being made
        /// </summary>
        public event EventHandler<AuthenticatingEventArgs> Authenticating;

        /// <summary>
        /// Fired after an authentication request has been made
        /// </summary>
        public event EventHandler<AuthenticatedEventArgs> Authenticated;
        /// <summary>
        /// Authenticate the application identity to an application principal
        /// </summary>
        public IPrincipal Authenticate(string applicationId, string applicationSecret)
        {
            // Data context
            using (ModelDataContext dataContext = new ModelDataContext(this.m_configuration.ReadonlyConnectionString))
            {
                Guid appId = Guid.Parse(applicationId);
                var client = dataContext.SecurityApplications.SingleOrDefault(o => o.ApplicationId == appId && o.ApplicationSecret == applicationSecret);
                if (client == null)
                    throw new SecurityException("Invalid application credentials");

                IPrincipal applicationPrincipal = new ApplicationPrincipal(new OpenIZ.Core.Security.ApplicationIdentity(client.ApplicationId, true));
                new PolicyPermission(System.Security.Permissions.PermissionState.None, PermissionPolicyIdentifiers.Login, applicationPrincipal).Demand();
                return applicationPrincipal;
            }

        }
    }
}
