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
 * Date: 2016-7-22
 */
using OpenIZ.Core.Interop.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenIZ.Core.Http;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.AMI.Auth;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Messaging.AMI.Client
{
    /// <summary>
    /// Administrative management service client
    /// </summary>
    public class AmiServiceClient : ServiceClientBase, IDisposable
    {
        /// <summary>
        /// Creates the AMI service client with the specified rest client
        /// </summary>
        /// <param name="restClient"></param>
        public AmiServiceClient(IRestClient restClient) : base(restClient)
        {
        }

        /// <summary>
        /// Retrieves the specified role from the AMI
        /// </summary>
        public AmiCollection<SecurityRoleInfo> FindRole(Expression<Func<SecurityRole, bool>> query)
        {
            var queryParms = QueryExpressionBuilder.BuildQuery(query).ToArray();
            return this.Client.Get<AmiCollection<SecurityRoleInfo>>("role", queryParms);
        }

        /// <summary>
        /// Retrieves the specified role from the AMI
        /// </summary>
        public AmiCollection<SecurityPolicyInfo> FindPolicy(Expression<Func<SecurityPolicy, bool>> query)
        {
            var queryParms = QueryExpressionBuilder.BuildQuery(query).ToArray();
            return this.Client.Get<AmiCollection<SecurityPolicyInfo>>("policy", queryParms);
        }

        /// <summary>
        /// Dispose the client
        /// </summary>
        public void Dispose()
        {
            this.Client.Dispose();
        }
    }
}
