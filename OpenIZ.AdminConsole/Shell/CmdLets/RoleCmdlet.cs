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
 * User: khannan
 * Date: 2017-7-31
 */
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.AdminConsole.Attributes;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Messaging.AMI.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.AdminConsole.Shell.CmdLets
{
    /// <summary>
    /// Commandlets for roles
    /// </summary>
    [AdminCommandlet]
    public static class RoleCmdlet
    {

        /// <summary>
        /// Base class for user operations
        /// </summary>
        internal class GenericRoleParms
        {

            /// <summary>
            /// Gets or sets the username
            /// </summary>
            [Description("The name of the role")]
            [Parameter("r")]
            public String RoleName { get; set; }

        }

        // Ami client
        private static AmiServiceClient m_client = new AmiServiceClient(ApplicationContext.Current.GetRestClient(Core.Interop.ServiceEndpointType.AdministrationIntegrationService));


        #region Add Role

        /// <summary>
        /// Add role parameters
        /// </summary>
        internal class AddRoleParms : GenericRoleParms
        {

            /// <summary>
            /// Gets or sets the policies
            /// </summary>
            [Description("The policies to set on the role")]
            [Parameter("p")]
            public StringCollection Policies { get; set; }

            
            /// <summary>
            /// Gets or sets the description
            /// </summary>
            [Description("Set the description of the role")]
            [Parameter("d")]
            public String Description { get; set; }

        }

        /// <summary>
        /// Add a role
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.CreateRoles)]
        [AdminCommand("roleadd", "Adds a role to the current OpenIZ instance")]
        internal static void AddRole(AddRoleParms parms)
        {
            var policies = new List<SecurityPolicyInfo>();

            if (parms.Policies?.Count > 0)
            {
                policies = parms.Policies.OfType<String>().Select(o => m_client.GetPolicies(r => r.Name == o).CollectionItem.FirstOrDefault()).ToList();
                policies.ForEach(o => o.Grant = Core.Model.Security.PolicyGrantType.Grant);
            }

            m_client.CreateRole(new Core.Model.AMI.Auth.SecurityRoleInfo()
            {
                Name = parms.RoleName,
                Policies = policies,
                Role = new Core.Model.Security.SecurityRole()
                {
                    Name = parms.RoleName,
                    Description = parms.Description
                }
            });

        }

        #endregion


    }
}
