/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-6-14
 */
using OpenIZ.Core.Services;
using OpenIZ.Persistence.Data.ADO.Data.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using OpenIZ.Persistence.Data.ADO.Data;
using System.Diagnostics;
using OpenIZ.Persistence.Data.ADO.Configuration;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Event;
using System.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using OpenIZ.Persistence.Data.ADO.Data.Model.Security;
using OpenIZ.Persistence.Data.ADO.Util;

namespace OpenIZ.Persistence.Data.ADO.Services
{
    /// <summary>
    /// Sql Application IdP
    /// </summary>
    public class AdoApplicationIdentityProvider : IApplicationIdentityProviderService
    {
        // Trace source
        private TraceSource m_traceSource = new TraceSource(AdoDataConstants.IdentityTraceSourceName);

        // Configuration
        private AdoConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(AdoDataConstants.ConfigurationSectionName) as AdoConfiguration;

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
            using (DataContext dataContext = this.m_configuration.Provider.GetReadonlyConnection())
            {
                IPasswordHashingService hashService = ApplicationContext.Current.GetService<IPasswordHashingService>();

                var client = dataContext.ExecStoredProcedure<DbSecurityApplication>("auth_app", applicationId, applicationSecret);
                if (client == null)
                    throw new SecurityException("Invalid application credentials");

                IPrincipal applicationPrincipal = new ApplicationPrincipal(new OpenIZ.Core.Security.ApplicationIdentity(client.Key, client.PublicId, true));
                new PolicyPermission(System.Security.Permissions.PermissionState.None, PermissionPolicyIdentifiers.Login, applicationPrincipal).Demand();
                return applicationPrincipal;
            }

        }

        /// <summary>
        /// Gets the specified identity
        /// </summary>
        public IIdentity GetIdentity(string name)
        {
            // Data context
            using (DataContext dataContext = this.m_configuration.Provider.GetReadonlyConnection())
            {
                var client = dataContext.Query<DbSecurityApplication>(o => o.PublicId == name);
                return new OpenIZ.Core.Security.ApplicationIdentity(client.ApplicationId, client.ApplicationPublicId, false);

            }
        }
    }
}
