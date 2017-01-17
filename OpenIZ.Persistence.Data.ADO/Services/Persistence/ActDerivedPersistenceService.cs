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
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Services;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Represents a persistence service which is derived from an act
    /// </summary>
    public abstract class ActDerivedPersistenceService<TModel, TData> : ActDerivedPersistenceService<TModel, TData, CompositeResult<TData, DbActVersion, DbAct>>
        where TModel : Core.Model.Acts.Act, new()
        where TData : DbActSubTable, new()
    { }

    /// <summary>
    /// Represents a persistence service which is derived from an act
    /// </summary>
    public abstract class ActDerivedPersistenceService<TModel, TData, TQueryReturn> : SimpleVersionedEntityPersistenceService<TModel, TData, TQueryReturn, DbActVersion>
        where TModel : Core.Model.Acts.Act, new()
        where TData : DbActSubTable, new()
        where TQueryReturn : CompositeResult
    {
        // act persister
        protected ActPersistenceService m_actPersister = new ActPersistenceService();


        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(TModel modelInstance, DataContext context, IPrincipal princpal)
        {
            var retVal = base.FromModelInstance(modelInstance, context, princpal);
            (retVal as DbActSubTable).ParentKey = modelInstance.VersionKey.Value;
            return retVal;
        }

        /// <summary>
        /// Entity model instance
        /// </summary>
        public override sealed TModel ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
        {
            return (TModel)this.m_actPersister.ToModelInstance(dataInstance, context, principal);
        }

        /// <summary>
        /// Insert the specified TModel into the database
        /// </summary>
        public override TModel Insert(DataContext context, TModel data, IPrincipal principal)
        {
            var inserted = this.m_actPersister.Insert(context, data, principal);
            data.Key = inserted.Key;
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update the specified TModel
        /// </summary>
        public override TModel Update(DataContext context, TModel data, IPrincipal principal)
        {
            this.m_actPersister.Update(context, data, principal);
            return base.Update(context, data, principal);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override TModel Obsolete(DataContext context, TModel data, IPrincipal principal)
        {
            var retVal = this.m_actPersister.Obsolete(context, data, principal);
            return data;
        }

    }
}
