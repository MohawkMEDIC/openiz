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
 * User: justi
 * Date: 2016-8-2
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.MSSQL.Data;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Assigning authority persistence service
    /// </summary>
    public class AssigningAuthorityPersistenceService : BaseDataPersistenceService<Core.Model.DataTypes.AssigningAuthority, Data.AssigningAuthority>
    {
        /// <summary>
        /// Convert assigning authority to model
        /// </summary>
        public override Core.Model.DataTypes.AssigningAuthority ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var dataAA = dataInstance as Data.AssigningAuthority;
            var retVal = base.ToModelInstance(dataInstance, context, principal);
            retVal.AuthorityScopeXml = dataAA.AssigningAuthorityScopes.Select(o => o.ScopeConceptId).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified data
        /// </summary>
        public override Core.Model.DataTypes.AssigningAuthority Insert(ModelDataContext context, Core.Model.DataTypes.AssigningAuthority data, IPrincipal principal)
        {
            var retVal = base.Insert(context, data, principal);

            // Scopes?
            if(data.AuthorityScopeXml != null)
                context.AssigningAuthorityScopes.InsertAllOnSubmit(data.AuthorityScopeXml.Select(o => new AssigningAuthorityScope() { ScopeConceptId = o, AssigningAuthorityId = retVal.Key.Value }));
            return retVal;
        }

        /// <summary>
        /// Update the specified data
        /// </summary>
        public override Core.Model.DataTypes.AssigningAuthority Update(ModelDataContext context, Core.Model.DataTypes.AssigningAuthority data, IPrincipal principal)
        {
            var retVal = base.Update(context, data, principal);

            // Scopes?
            if (data.AuthorityScopeXml != null)
            {
                context.AssigningAuthorityScopes.DeleteAllOnSubmit(context.AssigningAuthorityScopes.Where(o => o.AssigningAuthorityId == retVal.Key.Value));
                context.AssigningAuthorityScopes.InsertAllOnSubmit(data.AuthorityScopeXml.Select(o => new AssigningAuthorityScope() { ScopeConceptId = o, AssigningAuthorityId = retVal.Key.Value }));
            }
            return retVal;
        }

    }
}
