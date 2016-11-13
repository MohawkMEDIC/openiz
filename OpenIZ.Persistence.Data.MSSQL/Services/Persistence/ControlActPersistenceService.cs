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
 * Date: 2016-8-3
 */
using OpenIZ.Core.Model.Acts;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Linq;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Control act persistence service
    /// </summary>
    public class ControlActPersistenceService : ActDerivedPersistenceService<Core.Model.Acts.ControlAct, Data.ControlAct>
    {
        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override Core.Model.Acts.ControlAct ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var iddat = dataInstance as IDbIdentified;
            var controlAct = dataInstance as Data.ControlAct ?? context.GetTable<Data.ControlAct>().Where(o => o.ActVersionId == iddat.Id).First();
            var dba = dataInstance as Data.ActVersion ?? context.GetTable<Data.ActVersion>().Where(a => a.ActVersionId == controlAct.ActVersionId).First();
            // TODO: Any other cact fields
            return m_actPersister.ToModelInstance<Core.Model.Acts.ControlAct>(dba, context, principal);
        }
    }
}