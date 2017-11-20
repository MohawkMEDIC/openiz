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
 * Date: 2017-2-4
 */
using System;
using System.Linq;
using OpenIZ.Core.Model;
using System.Collections.Generic;
using OpenIZ.Persistence.Data.ADO.Data;
using System.Security.Principal;
using OpenIZ.Core;
using System.Linq.Expressions;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Exceptions;
using OpenIZ.Core.Model.Security;
using OpenIZ.OrmLite;


namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Base persistence service
    /// </summary>
    public abstract class BaseDataPersistenceService<TModel, TDomain> : BaseDataPersistenceService<TModel, TDomain, TDomain>
        where TModel : BaseEntityData, new()
        where TDomain : class, IDbBaseData, new()
    { }

    /// <summary>
    /// Base data persistence service
    /// </summary>
    public abstract class BaseDataPersistenceService<TModel, TDomain, TQueryResult> : IdentifiedPersistenceService<TModel, TDomain, TQueryResult>
        where TModel : BaseEntityData, new()
        where TDomain : class, IDbBaseData, new()
    {

        /// <summary>
        /// Performthe actual insert.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel InsertInternal(DataContext context, TModel data, IPrincipal principal)
        {
            if (data.CreatedBy != null) data.CreatedBy = data.CreatedBy?.EnsureExists(context, principal) as SecurityUser;
            data.CreatedByKey = data.CreatedBy?.Key ?? data.CreatedByKey;

            // HACK: For now, modified on can only come from one property, some non-versioned data elements are bound on UpdatedTime
            var nvd = data as NonVersionedEntityData;
            if (nvd != null)
            {
                nvd.UpdatedByKey = nvd.UpdatedByKey ?? principal.GetUserKey(context);
                nvd.UpdatedTime = DateTimeOffset.Now;
            }

            if (data.CreationTime == DateTimeOffset.MinValue || data.CreationTime.Year < 100)
                data.CreationTime = DateTimeOffset.Now;

            var domainObject = this.FromModelInstance(data, context, principal) as TDomain;

            // Ensure created by exists
            data.CreatedByKey = domainObject.CreatedByKey = domainObject.CreatedByKey == Guid.Empty ? principal.GetUserKey(context).Value : domainObject.CreatedByKey;
            domainObject = context.Insert<TDomain>(domainObject);
            data.CreationTime = (DateTimeOffset)domainObject.CreationTime;
            data.Key = domainObject.Key;
            return data;

        }

        /// <summary>
        /// Perform the actual update.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel UpdateInternal(DataContext context, TModel data, IPrincipal principal)
        {
            var nvd = data as NonVersionedEntityData;
            if (nvd != null)
            {
                if (nvd.UpdatedBy != null) nvd.UpdatedBy = nvd.UpdatedBy?.EnsureExists(context, principal) as SecurityUser;
                nvd.UpdatedByKey = nvd.UpdatedBy?.Key ?? nvd.UpdatedByKey;
            }

            // Check for key
            if (data.Key == Guid.Empty)
                throw new AdoFormalConstraintException(AdoFormalConstraintType.NonIdentityUpdate);

            // Get current object
            var domainObject = this.FromModelInstance(data, context, principal) as TDomain;
            var currentObject = context.FirstOrDefault<TDomain>(o => o.Key == data.Key);
            // Not found
            if (currentObject == null)
                throw new KeyNotFoundException(data.Key.ToString());

            // VObject
            var vobject = domainObject as IDbNonVersionedBaseData;
            if (vobject != null)
            {
                nvd.UpdatedByKey = vobject.UpdatedByKey = nvd.UpdatedByKey ?? principal.GetUserKey(context);
                nvd.UpdatedTime = vobject.UpdatedTime = DateTimeOffset.Now;
            }

            if (currentObject.CreationTime == domainObject.CreationTime) // HACK: Someone keeps passing up the same data so we have to correct here
                domainObject.CreationTime = DateTimeOffset.Now;

            currentObject.CopyObjectData(domainObject);
            currentObject = context.Update<TDomain>(currentObject);

            return data;
        }

        /// <summary>
        /// Query the specified object ordering by creation time
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<TModel> QueryInternal(DataContext context, Expression<Func<TModel, bool>> query, Guid queryId, int offset, int? count, out int totalResults, IPrincipal principal, bool countResults = true)
        {
            var qresult = this.QueryInternal(context, query, queryId, offset, count, out totalResults, countResults);
            return qresult.Select(o => o is Guid ? this.Get(context, (Guid)o, principal) : this.CacheConvert(o, context, principal)).ToList();
        }

        /// <summary>
        /// Performs the actual obsoletion
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel ObsoleteInternal(DataContext context, TModel data, IPrincipal principal)
        {
            if (data.Key == Guid.Empty)
                throw new AdoFormalConstraintException(AdoFormalConstraintType.NonIdentityUpdate);

            if (data.ObsoletedBy != null) data.ObsoletedBy = data.ObsoletedBy?.EnsureExists(context, principal) as SecurityUser;
            data.ObsoletedByKey = data.ObsoletedBy?.Key ?? data.ObsoletedByKey;

            // Current object
            var currentObject = context.FirstOrDefault<TDomain>(o => o.Key == data.Key);
            if (currentObject == null)
                throw new KeyNotFoundException(data.Key.ToString());

            //data.ObsoletedBy?.EnsureExists(context, principal);
            data.ObsoletedByKey = currentObject.ObsoletedByKey = data.ObsoletedBy?.Key ?? principal.GetUserKey(context);
            data.ObsoletionTime = currentObject.ObsoletionTime = currentObject.ObsoletionTime ?? DateTimeOffset.Now;

            context.Update(currentObject);
            return data;
        }


    }
}

