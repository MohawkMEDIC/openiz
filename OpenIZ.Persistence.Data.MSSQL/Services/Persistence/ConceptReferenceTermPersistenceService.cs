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
using OpenIZ.Persistence.Data.MSSQL.Exceptions;
using System.Collections.Generic;
using System.Data.Linq;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Concept reference terms persistence service
    /// </summary> 
    public class ConceptReferenceTermPersistenceService : VersionedAssociationPersistenceService<Core.Model.DataTypes.ConceptReferenceTerm, Core.Model.DataTypes.Concept, Data.ConceptReferenceTerm>
    {
        /// <summary>
        /// Gets the datatables
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override Table<Data.ConceptReferenceTerm> GetDataTable(ModelDataContext context)
        {
            return context.ConceptReferenceTerms;
        }

        /// <summary>
        /// Insert a concept reference term
        /// </summary>
        internal override Core.Model.DataTypes.ConceptReferenceTerm Insert(Core.Model.DataTypes.ConceptReferenceTerm storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            if (storageData.Key != Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.IdentityInsert);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (storageData.SourceEntityKey == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.AssociatedEntityWithoutSourceKey);

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
        /// Obsolete the specified reference term optionally creating a new version
        /// </summary>
        internal override Core.Model.DataTypes.ConceptReferenceTerm Obsolete(Core.Model.DataTypes.ConceptReferenceTerm storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (storageData.SourceEntityKey == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.AssociatedEntityWithoutSourceKey);

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
        /// Update the specified concept reference term
        /// </summary>
        internal override Core.Model.DataTypes.ConceptReferenceTerm Update(Core.Model.DataTypes.ConceptReferenceTerm storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
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
                newConceptVersion.Concept.ConceptReferenceTerms.Add(newDomainConceptReferenceTerm);
                dataContext.SubmitChanges();

                return this.ConvertToModel(newDomainConceptReferenceTerm);
            }
            else
                return this.ConvertToModel(domainReferenceTerm);
        }
    }
}