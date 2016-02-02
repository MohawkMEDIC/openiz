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
    /// Concept name persistence service
    /// </summary>
    public class ConceptNamePersistenceService : BaseDataPersistenceService<Core.Model.DataTypes.ConceptName>
    {
        /// <summary>
        /// Convert from model into domain class
        /// </summary>
        internal override object ConvertFromModel(Core.Model.DataTypes.ConceptName model)
        {
            return s_mapper.MapModelInstance<Core.Model.DataTypes.ConceptName, Data.ConceptName>(model);
        }

        /// <summary>
        /// Convert from domain to model
        /// </summary>
        internal override Core.Model.DataTypes.ConceptName ConvertToModel(object data)
        {
            return this.ConvertToModel(data as Data.ConceptName);
        }

        /// <summary>
        /// Convert to model
        /// </summary>
        internal Core.Model.DataTypes.ConceptName ConvertToModel(Data.ConceptName data)
        {
            return this.ConvertItem(data);

        }

        /// <summary>
        /// Get the specified concept name
        /// </summary>
        internal override Core.Model.DataTypes.ConceptName Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            var domainConceptName = dataContext.ConceptNames.SingleOrDefault(o => o.ConceptNameId == containerId.Id);
            if (domainConceptName == null)
                return null;
            else
                return this.ConvertToModel(domainConceptName);

        }

        /// <summary>
        /// Insert component name
        /// </summary>
        internal override Core.Model.DataTypes.ConceptName Insert(Core.Model.DataTypes.ConceptName storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            return this.Insert(storageData, principal, dataContext, true);
        }

        /// <summary>
        /// Insert a concept name
        /// </summary>
        internal Core.Model.DataTypes.ConceptName Insert(Core.Model.DataTypes.ConceptName storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            // Verify data
            if (storageData.Key != Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.IdentityInsert);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (storageData.SourceEntityKey == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.AssociatedEntityWithoutSourceKey);

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
        /// Obsolete specified component name
        /// </summary>
        internal override Core.Model.DataTypes.ConceptName Obsolete(Core.Model.DataTypes.ConceptName storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            return this.Obsolete(storageData, principal, dataContext, true);
        }

        /// <summary>
        /// Obsolete the specified concept name
        /// </summary>
        internal Core.Model.DataTypes.ConceptName Obsolete(Core.Model.DataTypes.ConceptName storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
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
        /// Query a concept name
        /// </summary>
        internal override IQueryable<Core.Model.DataTypes.ConceptName> Query(Expression<Func<Core.Model.DataTypes.ConceptName, bool>> query, IPrincipal principal, ModelDataContext dataContext)
        {
            var domainQuery = s_mapper.MapModelExpression<Core.Model.DataTypes.ConceptName, Data.ConceptName>(query);
            return dataContext.ConceptNames.Where(domainQuery).Select(o => this.ConvertToModel(o));
        }

        /// <summary>
        /// Update concept name 
        /// </summary>
        internal override Core.Model.DataTypes.ConceptName Update(Core.Model.DataTypes.ConceptName storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            return this.Update(storageData, principal, dataContext, true);
        }

        /// <summary>
        /// Update the concept name. An update occurs by creating (if necessary) a new version of the concept 
        /// to which the name is associated and obsoleting the old record
        /// </summary>
        internal Core.Model.DataTypes.ConceptName Update(Core.Model.DataTypes.ConceptName storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
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
                var currentConceptVersion = domainConceptName.Concept.ConceptVersions.Single(o => o.ObsoletionTime == null);
                ConceptVersion newConceptVersion = newVersion ? currentConceptVersion.NewVersion(principal, dataContext) : currentConceptVersion;

                // Obsolete the old data
                storageData.ObsoleteVersionSequenceId = domainConceptName.ObsoleteVersionSequenceId = newConceptVersion.VersionSequenceId;
                newDomainConceptName.ConceptId = domainConceptName.ConceptId;
                newDomainConceptName.EffectiveVersionSequenceId = newConceptVersion.VersionSequenceId;

                // Insert the new concept domain name
                dataContext.ConceptNames.InsertOnSubmit(newDomainConceptName);
                dataContext.SubmitChanges(); 

                return this.ConvertToModel(newDomainConceptName);
            }
            else
                return this.ConvertToModel(domainConceptName);

            
        }
    }
}