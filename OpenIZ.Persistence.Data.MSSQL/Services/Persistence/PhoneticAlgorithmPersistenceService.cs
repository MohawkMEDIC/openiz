/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-20
 */
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Collections.Generic;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Phonetic algorithm 
    /// </summary>
    internal class PhoneticAlgorithmPersistenceService : BaseDataPersistenceService<Core.Model.DataTypes.PhoneticAlgorithm>
    {
        /// <summary>
        /// Convert from model to domain classes
        /// </summary>
        internal override object ConvertFromModel(Core.Model.DataTypes.PhoneticAlgorithm model)
        {
            return s_mapper.MapModelInstance<Core.Model.DataTypes.PhoneticAlgorithm, Data.PhoneticAlgorithm>(model);
        }

        /// <summary>
        /// Convert from data model to busines model
        /// </summary>
        internal override Core.Model.DataTypes.PhoneticAlgorithm ConvertToModel(object data)
        {
            return this.ConvertToModel(data as Data.PhoneticAlgorithm);
        }

        /// <summary>
        /// Convert to model
        /// </summary>
        internal Core.Model.DataTypes.PhoneticAlgorithm ConvertToModel(Data.PhoneticAlgorithm data)
        {
            return this.ConvertItem(data);
        }

        /// <summary>
        /// Get the specified phonetic algorithm from the database storage layer
        /// </summary>
        internal override Core.Model.DataTypes.PhoneticAlgorithm Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            var domainPhoneticAlgorithm = dataContext.PhoneticAlgorithms.SingleOrDefault(o => o.PhoneticAlgorithmId == containerId.Id);
            if (domainPhoneticAlgorithm == null)
                return null;
            else
                return this.ConvertToModel(domainPhoneticAlgorithm);
        }

        /// <summary>
        /// Insert a phonetic algorithm
        /// </summary>
        internal override Core.Model.DataTypes.PhoneticAlgorithm Insert(Core.Model.DataTypes.PhoneticAlgorithm storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));


            var domainPhoneticAlgorithm = this.ConvertFromModel(storageData) as Data.PhoneticAlgorithm;
            dataContext.PhoneticAlgorithms.InsertOnSubmit(domainPhoneticAlgorithm);
            domainPhoneticAlgorithm.CreatedByEntity = principal.GetUser(dataContext);
            dataContext.SubmitChanges();
            return this.ConvertToModel(domainPhoneticAlgorithm);

        }

        /// <summary>
        /// Phonetic algorithms can only be deleted
        /// </summary>
        internal override Core.Model.DataTypes.PhoneticAlgorithm Obsolete(Core.Model.DataTypes.PhoneticAlgorithm storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var existingDomainAlgorithm = dataContext.PhoneticAlgorithms.SingleOrDefault(o => o.PhoneticAlgorithmId == storageData.Key);
            if (existingDomainAlgorithm == null)
                throw new KeyNotFoundException();

            //            dataContext.PhoneticAlgorithms.DeleteOnSubmit(existingDomainAlgorithm);
            existingDomainAlgorithm.ObsoletedByEntity = principal.GetUser(dataContext);
            existingDomainAlgorithm.ObsoletionTime = DateTimeOffset.Now;
            dataContext.SubmitChanges();

            existingDomainAlgorithm.PhoneticAlgorithmId = Guid.Empty;
            return this.ConvertToModel(existingDomainAlgorithm);
        }

        /// <summary>
        /// Query the phonetic algorithm types
        /// </summary>
        internal override IQueryable<Core.Model.DataTypes.PhoneticAlgorithm> Query(Expression<Func<Core.Model.DataTypes.PhoneticAlgorithm, bool>> query, IPrincipal principal, ModelDataContext dataContext)
        {
            var domainQuery = s_mapper.MapModelExpression<Core.Model.DataTypes.PhoneticAlgorithm, Data.PhoneticAlgorithm>(query);
            return dataContext.PhoneticAlgorithms.Where(domainQuery).Select(o => this.ConvertToModel(o));
        }

        /// <summary>
        /// Update the phonetic algorithm
        /// </summary>
        internal override Core.Model.DataTypes.PhoneticAlgorithm Update(Core.Model.DataTypes.PhoneticAlgorithm storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var domainPhoneticAlgorithm = dataContext.PhoneticAlgorithms.SingleOrDefault(o => o.PhoneticAlgorithmId == storageData.Key);
            domainPhoneticAlgorithm.CopyObjectData(this.ConvertFromModel(storageData));
            domainPhoneticAlgorithm.UpdatedByEntity = principal.GetUser(dataContext);
            domainPhoneticAlgorithm.UpdatedTime = DateTime.Now;

            // Update
            dataContext.SubmitChanges();
            return this.ConvertToModel(domainPhoneticAlgorithm);
        }
    }
}