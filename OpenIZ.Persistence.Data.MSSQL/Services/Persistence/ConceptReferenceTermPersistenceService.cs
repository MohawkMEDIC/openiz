/**
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
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.MSSQL.Data;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;
using System.Collections.Generic;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Concept reference terms persistence service
    /// </summary> 
    public class ConceptReferenceTermPersistenceService : BaseDataPersistenceService<Core.Model.DataTypes.ConceptReferenceTerm>
    {
        /// <summary>
        /// Convert the reference term from model to domain model
        /// </summary>
        internal override object ConvertFromModel(Core.Model.DataTypes.ConceptReferenceTerm model)
        {
            return s_mapper.MapModelInstance<Core.Model.DataTypes.ConceptReferenceTerm, Data.ConceptReferenceTerm>(model);
        }

        /// <summary>
        /// Convert to model
        /// </summary>
        internal override Core.Model.DataTypes.ConceptReferenceTerm ConvertToModel(object data)
        {
            return s_mapper.MapDomainInstance<Data.ConceptReferenceTerm, Core.Model.DataTypes.ConceptReferenceTerm>(data as Data.ConceptReferenceTerm);
        }

        /// <summary>
        /// Get the specified reference term association
        /// </summary>
        internal override Core.Model.DataTypes.ConceptReferenceTerm Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            return this.ConvertToModel(dataContext.ConceptReferenceTerms.SingleOrDefault(o => o.ConceptReferenceTermId == containerId.Id));
        }

        /// <summary>
        /// Inserts specified reference term creating version 
        /// </summary>
        internal override Core.Model.DataTypes.ConceptReferenceTerm Insert(Core.Model.DataTypes.ConceptReferenceTerm storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            return this.Insert(storageData, principal, dataContext, true);
        }

        /// <summary>
        /// Insert a concept reference term
        /// </summary>
        internal Core.Model.DataTypes.ConceptReferenceTerm Insert(Core.Model.DataTypes.ConceptReferenceTerm storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            if (storageData.Key != Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.IdentityInsert);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (storageData.SourceEntityKey == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.AssociatedEntityWithoutTargetKey);

            // Domain concept name
            var domainConceptReferenceTerm = this.ConvertFromModel(storageData) as Data.ConceptReferenceTerm;

            // Ensure traversable properties exist if they're objects
            if (storageData.ReferenceTerm != null)
                domainConceptReferenceTerm.ReferenceTermId = storageData.ReferenceTerm.EnsureExists(principal, dataContext).Key;
            if (storageData.SourceEntity != null)
                domainConceptReferenceTerm.ConceptId = storageData.SourceEntity.EnsureExists(principal, dataContext).Key;
            if(storageData.RelationshipType != null)
                domainConceptReferenceTerm.ConceptRelationshipTypeId = storageData.RelationshipType.EnsureExists(principal, dataContext).Key;
            
            // Get the current version & create a new version if needed
            var currentConceptVersion = dataContext.ConceptVersions.Single(o => o.ConceptId == storageData.SourceEntityKey && o.ObsoletionTime == null);
            ConceptVersion newConceptVersion = newVersion ? currentConceptVersion.NewVersion(principal, dataContext) : currentConceptVersion;
            domainConceptReferenceTerm.EffectiveVersionSequenceId = newConceptVersion.VersionSequenceId;
            domainConceptReferenceTerm.Concept = newConceptVersion.Concept;

            dataContext.ConceptReferenceTerms.InsertOnSubmit(domainConceptReferenceTerm);

            dataContext.SubmitChanges(); // Write and reload data from database

            // Copy properties
            storageData.Key = domainConceptReferenceTerm.ReferenceTermId;
            storageData.EffectiveVersionSequenceId = domainConceptReferenceTerm.EffectiveVersionSequenceId;
            storageData.SourceEntityKey = domainConceptReferenceTerm.ConceptId;
            return storageData;
        }

        /// <summary>
        /// Obsolete the specified reference term creating a new version of the target entity
        /// </summary>
        internal override Core.Model.DataTypes.ConceptReferenceTerm Obsolete(Core.Model.DataTypes.ConceptReferenceTerm storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            return this.Obsolete(storageData, principal, dataContext, true);
        }

        /// <summary>
        /// Obsolete the specified reference term optionally creating a new version
        /// </summary>
        internal Core.Model.DataTypes.ConceptReferenceTerm Obsolete(Core.Model.DataTypes.ConceptReferenceTerm storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (storageData.SourceEntityKey == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.AssociatedEntityWithoutTargetKey);

            // obsolete (i.e. remove the name association)
            var domainReferenceTerm = dataContext.ConceptReferenceTerms.SingleOrDefault(o => o.ConceptReferenceTermId == storageData.Key);

            if (domainReferenceTerm == null)
                throw new KeyNotFoundException();

            // Get the current version & create a new version if needed
            var currentConceptVersion = domainReferenceTerm.Concept.ConceptVersions.Single(o => o.ObsoletionTime == null);
            ConceptVersion newConceptVersion = newVersion ? currentConceptVersion.NewVersion(principal, dataContext) : currentConceptVersion;

            // Set the obsoletion time to the current version of the target entity
            domainReferenceTerm.ObsoleteVersionSequenceId = newConceptVersion.VersionSequenceId;
            dataContext.SubmitChanges(); // Write and reload values from db

            // Copy properties
            storageData.ObsoleteVersionSequenceId = domainReferenceTerm.ObsoleteVersionSequenceId;
            return storageData;
        }

        /// <summary>
        /// Query for the specified reference term
        /// </summary>
        internal override IQueryable<Core.Model.DataTypes.ConceptReferenceTerm> Query(Expression<Func<Core.Model.DataTypes.ConceptReferenceTerm, bool>> query, IPrincipal principal, ModelDataContext dataContext)
        {
            var domainQuery = s_mapper.MapModelExpression<Core.Model.DataTypes.ConceptReferenceTerm, Data.ConceptReferenceTerm>(query);
            return dataContext.ConceptReferenceTerms.Where(domainQuery).Select(o => this.ConvertToModel(o));
        }

        /// <summary>
        /// Update the reference term creating a new version of the target entity
        /// </summary>
        internal override Core.Model.DataTypes.ConceptReferenceTerm Update(Core.Model.DataTypes.ConceptReferenceTerm storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            return this.Update(storageData, principal, dataContext, true);
        }

        /// <summary>
        /// Update the specified concept reference term
        /// </summary>
        internal Core.Model.DataTypes.ConceptReferenceTerm Update(Core.Model.DataTypes.ConceptReferenceTerm storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);

            // Existing version of the name
            var domainReferenceTerm = dataContext.ConceptReferenceTerms.SingleOrDefault(o => o.ConceptReferenceTermId == storageData.Key);
            if (domainReferenceTerm == null)
                throw new KeyNotFoundException();

            // Prepare new reference term
            var newDomainConceptReferenceTerm = this.ConvertFromModel(storageData) as Data.ConceptReferenceTerm;
            newDomainConceptReferenceTerm.ConceptReferenceTermId = Guid.Empty;


            // Is there a need to update?
            if (newDomainConceptReferenceTerm.ConceptReferenceTermId != domainReferenceTerm.ConceptReferenceTermId ||
                newDomainConceptReferenceTerm.ConceptRelationshipTypeId != domainReferenceTerm.ConceptRelationshipTypeId)
            {

                // Ensure traversable properties exist if they're objects
                if (storageData.ReferenceTerm != null)
                    newDomainConceptReferenceTerm.ReferenceTermId = storageData.ReferenceTerm.EnsureExists(principal, dataContext).Key;
                if (storageData.RelationshipType != null)
                    newDomainConceptReferenceTerm.ConceptRelationshipTypeId = storageData.RelationshipType.EnsureExists(principal, dataContext).Key;

                // New Concept Version
                var currentConceptVersion = domainReferenceTerm.Concept.ConceptVersions.Single(o => o.ObsoletionTime == null);
                ConceptVersion newConceptVersion = newVersion ? currentConceptVersion.NewVersion(principal, dataContext) : currentConceptVersion;

                // Obsolete the old data
                storageData.ObsoleteVersionSequenceId = domainReferenceTerm.ObsoleteVersionSequenceId = newConceptVersion.VersionSequenceId;
                newDomainConceptReferenceTerm.ConceptId = domainReferenceTerm.ConceptId;
                newDomainConceptReferenceTerm.EffectiveVersionSequenceId = newConceptVersion.VersionSequenceId;

                // Insert the new concept domain name
                dataContext.ConceptReferenceTerms.InsertOnSubmit(newDomainConceptReferenceTerm);
                dataContext.SubmitChanges();

                return this.ConvertToModel(newDomainConceptReferenceTerm);
            }
            else
                return this.ConvertToModel(domainReferenceTerm);
        }
    }
}