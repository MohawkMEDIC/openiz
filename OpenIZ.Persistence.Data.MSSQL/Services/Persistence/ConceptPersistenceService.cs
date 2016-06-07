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
using OpenIZ.Core;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Concept persistence service
    /// </summary>
    public class ConceptPersistenceService : BaseDataPersistenceService<Core.Model.DataTypes.Concept>
    {
        /// <summary>
        /// Convert from model concept into a domain model concept
        /// </summary>
        internal override object ConvertFromModel(Core.Model.DataTypes.Concept model)
        {
            return s_mapper.MapModelInstance<Core.Model.DataTypes.Concept, Data.ConceptVersion>(model);
        }

        /// <summary>
        /// Convert a data context into a model object
        /// </summary>
        internal override Core.Model.DataTypes.Concept ConvertToModel(object data)
        {
            if (data == null)
                return null;

            var concept = data as ConceptVersion;

            var retVal = DataCache.Current.Get(concept.ConceptVersionId) as Core.Model.DataTypes.Concept;
            if (retVal == null)
            {
                retVal = this.ConvertItem(concept);

                // Load fast?
                if (retVal != null && concept.Concept != null)
                {
                    retVal.IsSystemConcept = concept.Concept.IsSystemConcept;
                    retVal.StatusConcept = base.ConvertItem(concept.StatusConcept.ConceptVersions.SingleOrDefault(o => o.ObsoletionTime == null));
                    retVal.Class = s_mapper.MapDomainInstance<Data.ConceptClass, Core.Model.DataTypes.ConceptClass>(concept.ConceptClass);
                    // Concept delay load
                    retVal.SetDelayLoadProperties(
                        concept.Concept.ConceptNames.Where(o => concept.VersionSequenceId >= o.EffectiveVersionSequenceId && (o.ObsoleteVersionSequenceId == null || concept.VersionSequenceId < o.ObsoleteVersionSequenceId)).Select(o => s_mapper.MapDomainInstance<Data.ConceptName, Core.Model.DataTypes.ConceptName>(o)).AsParallel().ToList(),
                        concept.Concept.ConceptReferenceTerms.Where(o => concept.VersionSequenceId >= o.EffectiveVersionSequenceId && (o.ObsoleteVersionSequenceId == null || concept.VersionSequenceId < o.ObsoleteVersionSequenceId)).Select(o => s_mapper.MapDomainInstance<Data.ConceptReferenceTerm, Core.Model.DataTypes.ConceptReferenceTerm>(o)).AsParallel().ToList()
                        );
                }
            }

            return retVal;
        }

        /// <summary>
        /// Convert to model
        /// </summary>
        internal Core.Model.DataTypes.Concept ConvertToModel(Data.ConceptVersion data, Data.Concept concept, Data.ConceptClass clazz, Data.ConceptVersion status)
        {
            var retVal = this.ConvertToModel(data);
            if(retVal != null)
                retVal.IsSystemConcept = concept.IsSystemConcept;
            retVal.StatusConcept = this.ConvertToModel(status);
            retVal.Class = s_mapper.MapDomainInstance<Data.ConceptClass, Core.Model.DataTypes.ConceptClass>(clazz);
            
            return retVal;
        }     

        /// <summary>
        /// Get the specified concept with version
        /// </summary>
        internal override Core.Model.DataTypes.Concept Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            if (containerId == null)
                throw new ArgumentNullException(nameof(containerId));

            ConceptVersion tRetVal = null;
            if (containerId.VersionId != default(Guid))
                tRetVal = dataContext.ConceptVersions.SingleOrDefault(o => o.ConceptVersionId == containerId.VersionId);
            else if (containerId.Id != default(Guid))
                tRetVal = dataContext.ConceptVersions.SingleOrDefault(o => o.ConceptId == containerId.Id && o.ObsoletionTime == null);

            // Return value
            if (tRetVal == null)
                return null;
            else
                return this.ConvertToModel(tRetVal);

        }

        /// <summary>
        /// Insert the concept and dependent properties (as list)
        /// </summary>
        internal override Core.Model.DataTypes.Concept Insert(Core.Model.DataTypes.Concept storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            
            // Store the data
            var dataConceptVersion = this.ConvertFromModel(storageData) as Data.ConceptVersion;
            dataConceptVersion.Concept = new Data.Concept() { IsSystemConcept = storageData.IsSystemConcept };
            dataConceptVersion.CreatedByEntity = principal.GetUser(dataContext);
            dataConceptVersion.StatusConceptId = dataConceptVersion.StatusConceptId == Guid.Empty ? StatusKeys.Active : dataConceptVersion.StatusConceptId;
            if(storageData.Class != null)
                dataConceptVersion.ConceptClassId = storageData.Class.EnsureExists(principal, dataContext)?.Key;

            // Store the root concept
            dataContext.ConceptVersions.InsertOnSubmit(dataConceptVersion);
            dataContext.SubmitChanges();

            storageData.Key = dataConceptVersion.ConceptId;
            storageData.VersionKey = dataConceptVersion.ConceptVersionId;

            // Concept names
            if (storageData.ConceptNames != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.ConceptName, Core.Model.DataTypes.Concept, Data.ConceptName>(
                    new List<Core.Model.DataTypes.ConceptName>(),
                    storageData.ConceptNames,
                    dataConceptVersion.ConceptId,
                    principal,
                    dataContext
                );

            // Reference terms
            if (storageData.ReferenceTerms != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.ConceptReferenceTerm, Core.Model.DataTypes.Concept, Data.ConceptReferenceTerm>(
                    new List<Core.Model.DataTypes.ConceptReferenceTerm>(),
                    storageData.ReferenceTerms,
                    dataConceptVersion.ConceptId,
                    principal,
                    dataContext
                );

            // Storage data 
            //if (storageData.Relationship != null)
            //{
            //    ConceptRelationshipPersistenceService relationshipPersister = new ConceptRelationshipPersistenceService();
            //    foreach (var rel in storageData.Relationship)
            //    {
            //        rel.EffectiveVersionSequenceId = storageData.VersionSequence;
            //        rel.SourceEntityKey = storageData.Key; // The Antelope's sloop wa a sickening sight
            //        relationshipPersister.Insert(rel, principal, dataContext, false); // How I wish I was in sherbrooke now!!!
            //    }
            //}

            return this.ConvertToModel(dataConceptVersion);
        }

        /// <summary>
        /// Obsolete the concept 
        /// </summary>
        internal override Core.Model.DataTypes.Concept Obsolete(Core.Model.DataTypes.Concept storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData == null)
                throw new ArgumentNullException(nameof(storageData));
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (storageData.Key == default(Guid))
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);
            else if (storageData.IsSystemConcept)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.UpdatedReadonlyObject);


            var dataConceptVersion = dataContext.ConceptVersions.SingleOrDefault(c => c.ConceptId == storageData.Key && c.ObsoletionTime == null);
            if (dataConceptVersion == null)
                throw new KeyNotFoundException();
            else if (dataConceptVersion.Concept.IsSystemConcept)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.UpdatedReadonlyObject);

            // Update old version as obsolete, insert a new version with obsolete status
            var newDataConceptVersion = this.ConvertFromModel(storageData) as ConceptVersion;
            dataConceptVersion.ObsoletionTime = DateTimeOffset.Now;
            newDataConceptVersion.CreatedByEntity = principal.GetUser(dataContext);
            dataConceptVersion.ObsoletedByEntity = principal.GetUser(dataContext);
            newDataConceptVersion.StatusConceptId = StatusKeys.Obsolete;
            dataContext.ConceptVersions.InsertOnSubmit(newDataConceptVersion);

            dataContext.SubmitChanges();
            
            var retVal = this.ConvertToModel(newDataConceptVersion);
            return retVal;
        }

        /// <summary>
        /// Query the concept versions
        /// </summary>
        internal override IQueryable<Core.Model.DataTypes.Concept> Query(Expression<Func<Core.Model.DataTypes.Concept, bool>> query, IPrincipal principal, ModelDataContext dataContext)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var domainQuery = s_mapper.MapModelExpression<Core.Model.DataTypes.Concept, Data.ConceptVersion>(query);
            return dataContext.ConceptVersions.Where(domainQuery)
                .OrderByDescending(o => o.VersionSequenceId)
                .Join(dataContext.Concepts, c => c.ConceptId, r => r.ConceptId, (a,b)=>new { ver = a, con = b })
                .Join(dataContext.ConceptClasses, c=>c.ver.ConceptClassId, r=>r.ConceptClassId, (a,b) => new { ver = a.ver, con = a.con, clazz = b })
                .Join(dataContext.ConceptVersions, c=>c.ver.StatusConceptId, r=>r.ConceptId, (a,b) => new { ver = a.ver, con = a.con, clazz = a.clazz, status = b })
                .Select(o=>this.ConvertToModel(o.ver, o.con, o.clazz, o.status));

        }

        /// <summary>
        /// Update the container
        /// </summary>
        internal override Core.Model.DataTypes.Concept Update(Core.Model.DataTypes.Concept storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (storageData.IsSystemConcept)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.UpdatedReadonlyObject);



            // Get the existing version 
            var domainConceptVersion = dataContext.ConceptVersions.OrderByDescending(o=>o.VersionSequenceId).SingleOrDefault(o => o.ConceptId == storageData.Key && o.ObsoletionTime == null);
            Decimal oldVersionSequenceId = domainConceptVersion.VersionSequenceId;

            if (domainConceptVersion == null)
                throw new KeyNotFoundException();

            // Create the new version
            domainConceptVersion = domainConceptVersion.NewVersion(principal, dataContext);
            // Copy and update new version tuple
            domainConceptVersion.CopyObjectData(this.ConvertFromModel(storageData));
            domainConceptVersion.CreatedByEntity = principal.GetUser(dataContext);
            domainConceptVersion.ConceptVersionId = Guid.Empty;
            domainConceptVersion.ReplacesVersionId = storageData.VersionKey;
            domainConceptVersion.Concept.IsSystemConcept = storageData.IsSystemConcept;

            dataContext.SubmitChanges(); // Submit changes to db

            // Update the dependent objects
            ConceptNamePersistenceService cnPersistenceService = new ConceptNamePersistenceService();
            ConceptReferenceTermPersistenceService rtPersistenceService = new ConceptReferenceTermPersistenceService();

            // First thing, we want to remove any names that no longer appear in the storageData and/or update those 
            if (storageData.ConceptNames != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.ConceptName, Core.Model.DataTypes.Concept, Data.ConceptName>(
                    domainConceptVersion.Concept.ConceptNames.Where(o => oldVersionSequenceId >= o.EffectiveVersionSequenceId && o.ObsoleteVersionSequenceId == null).Select(o => cnPersistenceService.ConvertToModel(o)).ToList(),
                    storageData.ConceptNames,
                    domainConceptVersion.ConceptId,
                    principal,
                    dataContext
                    );

            // Next thing, we want to remove any reference terms that no longer appear
            if (storageData.ReferenceTerms != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.ConceptReferenceTerm, Core.Model.DataTypes.Concept, Data.ConceptReferenceTerm>(
                   domainConceptVersion.Concept.ConceptReferenceTerms.Where(o => oldVersionSequenceId >= o.EffectiveVersionSequenceId && o.ObsoleteVersionSequenceId == null).Select(o => rtPersistenceService.ConvertToModel(o)).ToList(),
                   storageData.ReferenceTerms,
                   domainConceptVersion.ConceptId,
                   principal,
                   dataContext
                   );
           

            var retVal = this.ConvertToModel(domainConceptVersion);
            return retVal;
        }
    }
}
