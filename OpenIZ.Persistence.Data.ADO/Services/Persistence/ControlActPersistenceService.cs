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
 * Date: 2017-1-21
 */
using OpenIZ.Core.Model.Acts;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Acts;
using OpenIZ.OrmLite;

using System.Linq;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Control act persistence service
    /// </summary>
    public class ControlActPersistenceService : ActDerivedPersistenceService<Core.Model.Acts.ControlAct, DbControlAct, CompositeResult<DbControlAct, DbActVersion, DbAct>>
    {
        /// <summary>
        /// Convert to model instance
        /// </summary>
        public Core.Model.Acts.ControlAct ToModelInstance(DbControlAct controlAct, DbActVersion actVersionInstance, DbAct actInstance, DataContext context, IPrincipal principal)
        {
            // TODO: Any other cact fields
            return m_actPersister.ToModelInstance<Core.Model.Acts.ControlAct>(actVersionInstance, actInstance, context, principal);
        }
    }
}