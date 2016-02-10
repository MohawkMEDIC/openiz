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
 * Date: 2016-1-19
 */
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Linq.Expressions;
using System.Security.Principal;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Concept reference type persistence service
    /// </summary>
    public class ConceptRelationshipTypePersistenceService : BaseDataPersistenceService<Core.Model.DataTypes.ConceptRelationshipType>
    {
        /// <summary>
        /// Convert instance from model 
        /// </summary>
        internal override object ConvertFromModel(Core.Model.DataTypes.ConceptRelationshipType model)
        {
            return s_mapper.MapModelInstance<Core.Model.DataTypes.ConceptRelationshipType, Data.ConceptRelationshipType>(model);
        }

        /// <summary>
        /// Conver to a model
        /// </summary>
        internal override Core.Model.DataTypes.ConceptRelationshipType ConvertToModel(object data)
        {
            return this.ConvertToModel(data as Data.ConceptRelationshipType);
        }

        /// <summary>
        /// Convert to model
        /// </summary>
        internal Core.Model.DataTypes.ConceptRelationshipType ConvertToModel(Data.ConceptRelationshipType data)
        {
            return this.ConvertItem(data);
        }

        /// <summary>
        /// Get the specified container
        /// </summary>
        internal override Core.Model.DataTypes.ConceptRelationshipType Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            return this.ConvertToModel(dataContext.ConceptRelationshipTypes.SingleOrDefault(r => r.ConceptRelationshipTypeId == containerId.Id));
        }

        /// <summary>
        /// Insert a concept relationship type
        /// </summary>
        internal override Core.Model.DataTypes.ConceptRelationshipType Insert(Core.Model.DataTypes.ConceptRelationshipType storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData.Key != null)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.IdentityInsert);

            var domainRelationshipType = this.ConvertFromModel(storageData) as Data.ConceptRelationshipType;
            domainRelationshipType.CreatedByEntity = principal.GetUser(dataContext);
            dataContext.ConceptRelationshipTypes.InsertOnSubmit(domainRelationshipType);
            dataContext.SubmitChanges();
            storageData.Key = domainRelationshipType.ConceptRelationshipTypeId;
            return storageData;
        }

        /// <summary>
        /// Obsolete the specified record (in the case of meta-data, a delete)
        /// </summary>
        internal override Core.Model.DataTypes.ConceptRelationshipType Obsolete(Core.Model.DataTypes.ConceptRelationshipType storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            // Validate
            if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);

            var domainRelationshipType = dataContext.ConceptRelationshipTypes.SingleOrDefault(o => o.ConceptRelationshipTypeId == storageData.Key);
            if (domainRelationshipType == null)
                throw new KeyNotFoundException();

            domainRelationshipType.ObsoletedByEntity = principal.GetUser(dataContext);
            domainRelationshipType.ObsoletionTime = DateTime.Now;

            //dataContext.ConceptRelationshipTypes.DeleteOnSubmit(domainRelationshipType);
            dataContext.SubmitChanges();

            storageData.Key = Guid.Empty;

            return storageData;
        }

        /// <summary>
        /// Query the database for the specified data
        /// </summary>
        internal override IQueryable<Core.Model.DataTypes.ConceptRelationshipType> Query(Expression<Func<Core.Model.DataTypes.ConceptRelationshipType, bool>> query, IPrincipal principal, ModelDataContext dataContext)
        {
            var domainQuery = s_mapper.MapModelExpression<Core.Model.DataTypes.ConceptRelationshipType, Data.ConceptRelationshipType>(query);
            return dataContext.ConceptRelationshipTypes.Where(domainQuery).Select(o => this.ConvertToModel(o));
        }

        /// <summary>
        /// Update the specified storage data
        /// </summary>
        internal override Core.Model.DataTypes.ConceptRelationshipType Update(Core.Model.DataTypes.ConceptRelationshipType storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);

            var domainRelationshipType = dataContext.ConceptRelationshipTypes.SingleOrDefault(o => o.ConceptRelationshipTypeId == storageData.Key);
            if (domainRelationshipType == null)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);

            // Update data
            domainRelationshipType.UpdatedByEntity = principal.GetUser(dataContext);
            domainRelationshipType.UpdatedTime = DateTimeOffset.Now;

            storageData.Mnemonic = domainRelationshipType.Mnemonic = storageData.Mnemonic ?? domainRelationshipType.Mnemonic;
            storageData.Name = domainRelationshipType.Name = storageData.Mnemonic ?? domainRelationshipType.Name;
            dataContext.SubmitChanges();

            return storageData;
        }
    }
}
