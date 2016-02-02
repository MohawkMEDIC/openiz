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
 * Date: 2016-1-24
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
    /// A persistence implementation that can persist Concet Sets
    /// </summary>
    public class ConceptSetPersistenceService : BaseDataPersistenceService<Core.Model.DataTypes.ConceptSet>
    {
        /// <summary>
        /// Convert from a model
        /// </summary>
        internal override object ConvertFromModel(Core.Model.DataTypes.ConceptSet model)
        {
            return s_mapper.MapModelInstance<Core.Model.DataTypes.ConceptSet, Data.ConceptSet>(model);
        }

        /// <summary>
        /// Convert to a model instance
        /// </summary>
        internal override Core.Model.DataTypes.ConceptSet ConvertToModel(object data)
        {
            return this.ConvertToModel(data as Data.ConceptSet);
        }

        /// <summary>
        /// Convert to model
        /// </summary>
        internal Core.Model.DataTypes.ConceptSet ConvertToModel(Data.ConceptSet data)
        {
            return this.ConvertItem(data);
        }

        /// <summary>
        /// Get the specified concept set by identiifer
        /// </summary>
        internal override Core.Model.DataTypes.ConceptSet Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {

            Data.ConceptSet tRetVal = dataContext.ConceptSets.SingleOrDefault(o => o.ConceptSetId == containerId.Id);

            // Return value
            if (tRetVal == null)
                return null;
            else
                return this.ConvertToModel(tRetVal);

        }

        /// <summary>
        /// Insert the specified concept set into the datamodel
        /// </summary>
        internal override Core.Model.DataTypes.ConceptSet Insert(Core.Model.DataTypes.ConceptSet storageData, IPrincipal principal, ModelDataContext dataContext)
        {

            // Insert the concept model
            if (storageData.Key != Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.IdentityInsert);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            // Inser the record
            var domainConceptSet = this.ConvertFromModel(storageData) as Data.ConceptSet;
            dataContext.ConceptSets.InsertOnSubmit(domainConceptSet);
            domainConceptSet.CreatedByEntity = principal.GetUser(dataContext);
            // Now insert the relationships
            domainConceptSet.ConceptSetMembers.AddRange(storageData.Concepts.Select(o => new ConceptSetMember() { ConceptId = o.EnsureExists(principal, dataContext).Key, ConceptSet = domainConceptSet }));

            // Perform insert
            dataContext.SubmitChanges();
            storageData.Key = domainConceptSet.ConceptSetId;
            return storageData;
        }

        /// <summary>
        /// Obsolete the specified storage data container
        /// </summary>
        internal override Core.Model.DataTypes.ConceptSet Obsolete(Core.Model.DataTypes.ConceptSet storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            // Validate
            if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));


            // Retrieve
            var domainConceptSet = dataContext.ConceptSets.SingleOrDefault(o => o.ConceptSetId == storageData.Key);
            if (domainConceptSet == null)
                throw new KeyNotFoundException();
            // Obsolete
            domainConceptSet.ObsoletedByEntity = principal.GetUser(dataContext);
            storageData.ObsoletionTime =  domainConceptSet.ObsoletionTime = DateTime.Now;
            domainConceptSet.ObsoletionReason = storageData.ObsoletionReason;
            storageData.ObsoletedByKey = domainConceptSet.ObsoletedByEntity.UserId;

            // Submit changes
            dataContext.SubmitChanges();

            return storageData;
        }

        /// <summary>
        /// Query the specified concept set from the database
        /// </summary>
        internal override IQueryable<Core.Model.DataTypes.ConceptSet> Query(Expression<Func<Core.Model.DataTypes.ConceptSet, bool>> query, IPrincipal principal, ModelDataContext dataContext)
        {
            var domainQuery = s_mapper.MapModelExpression<Core.Model.DataTypes.ConceptSet, Data.ConceptSet>(query);
            return dataContext.ConceptSets.Where(domainQuery).Select(
                o => this.ConvertToModel(o)
            );
        }

        /// <summary>
        /// Update the specified concept set
        /// </summary>
        internal override Core.Model.DataTypes.ConceptSet Update(Core.Model.DataTypes.ConceptSet storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));


            // Get the existing version
            var existingSet = dataContext.ConceptSets.SingleOrDefault(o => o.ConceptSetId == storageData.Key);
            if (existingSet == null)
                throw new KeyNotFoundException();
            // Merge changes
            existingSet.CopyObjectData(this.ConvertFromModel(storageData));

            // Now verify which concepts should be removed/added/updated
            if(storageData.Concepts != null)
            {
                // Existing concepts which do not appear in the 
                var existingConcepts = existingSet.ConceptSetMembers.Where(o => o.ConceptSetId == storageData.Key); // active names

                // Remove old
                dataContext.ConceptSetMembers.DeleteAllOnSubmit(existingConcepts.Where(o => !storageData.Concepts.Exists(ecn => ecn.Key == o.ConceptId)));

                // Insert those that do not exist
                var insertRecords = storageData.Concepts.Where(o => !existingConcepts.Any(ecn => ecn.ConceptId == o.Key));
                foreach (var ins in insertRecords)
                    existingSet.ConceptSetMembers.Add(new ConceptSetMember() { ConceptSet = existingSet, ConceptId = ins.EnsureExists(principal, dataContext).Key });
            }

            dataContext.SubmitChanges();

            var retVal = this.ConvertToModel(existingSet);

            return retVal;
        }
    }
}
