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
    /// Concept name persistence service
    /// </summary>
    public class ConceptNamePersistenceService : VersionedAssociationPersistenceService<Core.Model.DataTypes.ConceptName, Core.Model.DataTypes.Concept, Data.ConceptName>
    {
        /// <summary>
        /// Get the data table
        /// </summary>
        protected override Table<Data.ConceptName> GetDataTable(ModelDataContext context)
        {
            return context.ConceptNames;
        }

        /// <summary>
        /// Insert a concept name
        /// </summary>
        internal override Core.Model.DataTypes.ConceptName Insert(Core.Model.DataTypes.ConceptName storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            var domainConceptName = this.ConvertFromModel(storageData) as Data.ConceptName;

            // Ensure traversable properties exist if they're objects
            if (storageData.PhoneticAlgorithm != null)
                domainConceptName.PhoneticAlgorithmId = storageData.PhoneticAlgorithm.EnsureExists(principal, dataContext).Key;
            if (storageData.SourceEntity != null)
                domainConceptName.ConceptId = storageData.SourceEntity.EnsureExists(principal, dataContext).Key;

            // Get the current version & create a new version if needed
            var currentConceptVersion = dataContext.ConceptVersions.Single(o => o.ConceptId == storageData.SourceEntityKey && o.ObsoletionTime == null);
            ConceptVersion newConceptVersion = newVersion ? currentConceptVersion.NewVersion(principal, dataContext) : currentConceptVersion;
            domainConceptName.EffectiveVersionSequenceId = newConceptVersion.VersionSequenceId;
            domainConceptName.Concept = newConceptVersion.Concept;

            dataContext.ConceptNames.InsertOnSubmit(domainConceptName);

            dataContext.SubmitChanges(); // Write and reload data from database

            // Copy properties
            storageData.Key = domainConceptName.ConceptNameId;
            storageData.EffectiveVersionSequenceId = domainConceptName.EffectiveVersionSequenceId;
            storageData.SourceEntityKey = domainConceptName.ConceptId;
            return storageData;
        }

        /// <summary>
        /// Obsolete the specified concept name
        /// </summary>
        internal override Core.Model.DataTypes.ConceptName Obsolete(Core.Model.DataTypes.ConceptName storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (storageData.SourceEntityKey == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.AssociatedEntityWithoutSourceKey);

            // obsolete (i.e. remove the name association)
            var domainConceptName = dataContext.ConceptNames.SingleOrDefault(o => o.ConceptNameId == storageData.Key);

            if (domainConceptName == null)
                throw new KeyNotFoundException();

            // Get the current version & create a new version if needed
            var currentConceptVersion = domainConceptName.Concept.ConceptVersions.Single(o => o.ObsoletionTime == null);
            ConceptVersion newConceptVersion = newVersion ? currentConceptVersion.NewVersion(principal, dataContext) : currentConceptVersion;

            // Set the obsoletion time to the current version of the target entity
            domainConceptName.ObsoleteVersionSequenceId = newConceptVersion.VersionSequenceId;
            dataContext.SubmitChanges(); // Write and reload values from db

            // Copy properties
            storageData.ObsoleteVersionSequenceId = domainConceptName.ObsoleteVersionSequenceId;
            return storageData;

        }

        /// <summary>
        /// Update the concept name. An update occurs by creating (if necessary) a new version of the concept 
        /// to which the name is associated and obsoleting the old record
        /// </summary>
        internal override Core.Model.DataTypes.ConceptName Update(Core.Model.DataTypes.ConceptName storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            // Existing version of the name
            var domainConceptName = dataContext.ConceptNames.SingleOrDefault(o => o.ConceptNameId == storageData.Key);
            if (domainConceptName == null)
                throw new KeyNotFoundException();

            var newDomainConceptName = this.ConvertFromModel(storageData) as Data.ConceptName;
            newDomainConceptName.ConceptNameId = Guid.Empty;

            
            // Is there a need to update?
            if (newDomainConceptName.LanguageCode != domainConceptName.LanguageCode ||
                newDomainConceptName.Name != domainConceptName.Name ||
                newDomainConceptName.PhoneticCode != domainConceptName.PhoneticCode ||
                newDomainConceptName.PhoneticAlgorithmId != domainConceptName.PhoneticAlgorithmId)
            {

                // Ensure traversable properties exist if they're objects
                if (storageData.PhoneticAlgorithm != null)
                    newDomainConceptName.PhoneticAlgorithmId = storageData.PhoneticAlgorithm.EnsureExists(principal, dataContext).Key;

                // New Concept Version
                var currentConceptVersion = domainConceptName.Concept.ConceptVersions.OrderByDescending(o=>o.VersionSequenceId).Single(o => o.ObsoletionTime == null);
                ConceptVersion newConceptVersion = newVersion ? currentConceptVersion.NewVersion(principal, dataContext) : currentConceptVersion;

                // Obsolete the old data
                storageData.ObsoleteVersionSequenceId = domainConceptName.ObsoleteVersionSequenceId = newConceptVersion.VersionSequenceId;
                newDomainConceptName.ConceptId = domainConceptName.ConceptId;
                
                newDomainConceptName.EffectiveVersionSequenceId = newConceptVersion.VersionSequenceId;

                // Insert the new concept domain name
                //dataContext.ConceptNames.InsertOnSubmit(newDomainConceptName);
                newConceptVersion.Concept.ConceptNames.Add(newDomainConceptName);
                dataContext.SubmitChanges(); 

                return this.ConvertToModel(newDomainConceptName);
            }
            else
                return this.ConvertToModel(domainConceptName);

            
        }
    }
}