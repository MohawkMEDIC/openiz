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
    /// Reference term persistence service
    /// </summary>
    public class ReferenceTermPersistenceService : BaseDataPersistenceService<Core.Model.DataTypes.ReferenceTerm>
    {
        /// <summary>
        /// Convert from reference term to a domain type
        /// </summary>
        internal override object ConvertFromModel(Core.Model.DataTypes.ReferenceTerm model)
        {
            return s_mapper.MapModelInstance<Core.Model.DataTypes.ReferenceTerm, Data.ReferenceTerm>(model);
        }

        /// <summary>
        /// Convert from dat model into business model
        /// </summary>
        internal override Core.Model.DataTypes.ReferenceTerm ConvertToModel(object data)
        {
            return this.ConvertToModel(data as Data.ReferenceTerm);
        }

        /// <summary>
        /// Convert to model
        /// </summary>
        internal Core.Model.DataTypes.ReferenceTerm ConvertToModel(Data.ReferenceTerm data)
        {
            return this.ConvertItem(data);
        }

        /// <summary>
        /// Get the specified reference term
        /// </summary>
        internal override Core.Model.DataTypes.ReferenceTerm Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            return this.ConvertToModel(dataContext.ReferenceTerms.SingleOrDefault(o => o.ReferenceTermId == containerId.Id));
        }

        /// <summary>
        /// Insert the reference term into the database
        /// </summary>
        internal override Core.Model.DataTypes.ReferenceTerm Insert(Core.Model.DataTypes.ReferenceTerm storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData.Key != Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.IdentityInsert);

            var domainReferenceTerm = this.ConvertFromModel(storageData) as Data.ReferenceTerm;
            
            // Persist code system
            if (storageData.CodeSystem != null)
                storageData.CodeSystemKey = storageData.CodeSystem.EnsureExists(principal, dataContext).Key;

            // Display names
            if(storageData.DisplayNames != null)
                foreach(var itm in storageData.DisplayNames)
                {
                    var refDisplayName = s_mapper.MapModelInstance<Core.Model.DataTypes.ReferenceTermName, Data.ReferenceTermDisplayName>(itm);
                    refDisplayName.CreatedByEntity = principal.GetUser(dataContext);
                    refDisplayName.PhoneticAlgorithmId = itm.PhoneticAlgorithm.EnsureExists(principal, dataContext).Key;
                    domainReferenceTerm.ReferenceTermDisplayNames.Add(refDisplayName);
                }

            dataContext.ReferenceTerms.InsertOnSubmit(domainReferenceTerm);

            dataContext.SubmitChanges();
            storageData.Key = domainReferenceTerm.ReferenceTermId;

            return storageData;
        }

        /// <summary>
        /// Obsolete the reference term in the database (delete)
        /// </summary>
        internal override Core.Model.DataTypes.ReferenceTerm Obsolete(Core.Model.DataTypes.ReferenceTerm storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);

            var domainReferenceTerm = dataContext.ReferenceTerms.SingleOrDefault(o => o.ReferenceTermId == storageData.Key);
            if (domainReferenceTerm == null)
                throw new KeyNotFoundException();

            dataContext.ReferenceTerms.DeleteOnSubmit(domainReferenceTerm);
            storageData.Key = Guid.Empty;
            dataContext.SubmitChanges();

            return storageData;
        }

        /// <summary>
        /// Query the database for a reference term
        /// </summary>
        internal override IQueryable<Core.Model.DataTypes.ReferenceTerm> Query(Expression<Func<Core.Model.DataTypes.ReferenceTerm, bool>> query, IPrincipal principal, ModelDataContext dataContext)
        {
            var domainQuery = s_mapper.MapModelExpression<Core.Model.DataTypes.ReferenceTerm, Data.ReferenceTerm>(query);
            return dataContext.ReferenceTerms.Where(domainQuery).Select(o => this.ConvertToModel(o));
        }

        /// <summary>
        /// Update the reference term
        /// </summary>
        internal override Core.Model.DataTypes.ReferenceTerm Update(Core.Model.DataTypes.ReferenceTerm storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.UpdatedReadonlyObject);

            var domainReferenceTerm = dataContext.ReferenceTerms.SingleOrDefault(o => o.ReferenceTermId == storageData.Key);
            if (domainReferenceTerm == null)
                throw new KeyNotFoundException();
            domainReferenceTerm.CopyObjectData(this.ConvertFromModel(storageData));

            domainReferenceTerm.UpdatedByEntity = principal.GetUser(dataContext);
            domainReferenceTerm.UpdateTime = DateTime.Now;

            // Display names
            if (storageData.DisplayNames != null)
            {
                var existingDisplayNames = domainReferenceTerm.ReferenceTermDisplayNames.ToList();
                
                // Delete from those not in the storage data
                existingDisplayNames.RemoveAll(o => !storageData.DisplayNames.Exists(n => n.Key == o.ReferenceTermId));
                // Update existing
                foreach(var itm in existingDisplayNames.Where(o=>storageData.DisplayNames.Any(p=>p.Key == o.ReferenceTermDisplayNameId)))
                    itm.CopyObjectData(s_mapper.MapModelInstance<Core.Model.DataTypes.ReferenceTermName, Data.ReferenceTermDisplayName>(
                        storageData.DisplayNames.Single(o=>o.Key == itm.ReferenceTermDisplayNameId
                        )));

                // Insert new stuff
                foreach (var itm in storageData.DisplayNames.Where(w=>!existingDisplayNames.Any(e=>e.ReferenceTermId == w.Key)))
                {
                    var refDisplayName = s_mapper.MapModelInstance<Core.Model.DataTypes.ReferenceTermName, Data.ReferenceTermDisplayName>(itm);
                    refDisplayName.CreatedByEntity = principal.GetUser(dataContext);
                    domainReferenceTerm.ReferenceTermDisplayNames.Add(refDisplayName);
                }
            }
            
            dataContext.SubmitChanges();

            return storageData;
        }
    }
}
