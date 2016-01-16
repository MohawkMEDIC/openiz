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
            return s_mapper.MapDomainInstance<Data.ConceptName, Core.Model.DataTypes.ConceptName>(data as Data.ConceptName);
        }

        /// <summary>
        /// Get the specified concept name
        /// </summary>
        internal override Core.Model.DataTypes.ConceptName Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            var domainConceptName = dataContext.ConceptNames.FirstOrDefault(o => o.ConceptNameId == containerId.Id);
            if (domainConceptName == null)
                return null;
            else
                return this.ConvertToModel(domainConceptName);

        }

        /// <summary>
        /// Insert a concept name
        /// </summary>
        internal override Core.Model.DataTypes.ConceptName Insert(Core.Model.DataTypes.ConceptName storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            // Verify data
            if (storageData.Key != Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.IdentityInsert);
            else if (storageData.TargetEntityKey == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.AssociatedEntityWithoutTargetKey);
            else if(storageData.EffectiveVersionSequenceId == default(Decimal))
                throw new SqlFormalConstraintException(SqlFormalConstraintType.AssociatedEntityWithoutEffectiveVersion);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var domainConceptName = this.ConvertFromModel(storageData) as Data.ConceptName;

            // Ensure traversable properties exist if they're objects
            if (storageData.PhoneticAlgorithm != null)
                domainConceptName.PhoneticAlgorithmId = storageData.PhoneticAlgorithm.EnsureExists(principal, dataContext).Key;
            if (storageData.TargetEntity != null)
                domainConceptName.ConceptId = storageData.TargetEntity.EnsureExists(principal, dataContext).Key;

            dataContext.ConceptNames.InsertOnSubmit(domainConceptName);
            dataContext.SubmitChanges();

            return this.ConvertToModel(domainConceptName);
        }
        
        /// <summary>
        /// Obsolete the specified concept name
        /// </summary>
        internal override Core.Model.DataTypes.ConceptName Obsolete(Core.Model.DataTypes.ConceptName storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            // obsolete (i.e. remove the name association)
            var domainConceptName = dataContext.ConceptNames.FirstOrDefault(o => o.ConceptNameId == storageData.Key);
            if (domainConceptName == null)
                throw new KeyNotFoundException();

            // Set the obsoletion time to the current version of the target entity
            domainConceptName.ObsoleteVersionSequenceId = storageData.ObsoleteVersion?.VersionSequence ?? storageData.TargetEntity?.VersionSequence ??
                dataContext.ConceptVersions.FirstOrDefault(o => o.VersionSequenceId == storageData.ObsoleteVersionSequenceId)?.VersionSequenceId ?? dataContext.ConceptVersions.FirstOrDefault(o => o.ConceptId == storageData.TargetEntityKey && o.ObsoletionTime == null)?.VersionSequenceId;

            dataContext.SubmitChanges();

            return this.ConvertToModel(domainConceptName);
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
        /// Update the concept name. An update occurs by creating (if necessary) a new version of the concept 
        /// to which the name is associated and obsoleting the old record
        /// </summary>
        internal override Core.Model.DataTypes.ConceptName Update(Core.Model.DataTypes.ConceptName storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            // Existing version of the name
            var domainConceptName = dataContext.ConceptNames.FirstOrDefault(o => o.ConceptNameId == storageData.Key);
            var newDomainConceptName = this.ConvertFromModel(storageData) as Data.ConceptName;
            newDomainConceptName.ConceptNameId = Guid.Empty;

            // Ensure traversable properties exist if they're objects
            if (storageData.PhoneticAlgorithm != null)
                newDomainConceptName.PhoneticAlgorithmId = storageData.PhoneticAlgorithm.EnsureExists(principal, dataContext).Key;

            // Is there a need to update?
            if (newDomainConceptName.LanguageCode != domainConceptName.LanguageCode ||
                newDomainConceptName.Name != domainConceptName.Name ||
                newDomainConceptName.PhoneticCode != domainConceptName.PhoneticCode ||
                newDomainConceptName.PhoneticAlgorithmId != domainConceptName.PhoneticAlgorithmId)
            {

                // Get the current version & mark as obsolete
                var currentConceptVersion = domainConceptName.Concept.ConceptVersions.First(o => o.ObsoletionTime == null);

                // Create a new version of the concept if needed
                ConceptVersion newConceptVersion = currentConceptVersion.NewVersion(principal, dataContext);

                // Obsolete the old data
                domainConceptName.ObsoleteVersionSequenceId = newConceptVersion.VersionSequenceId;
                newDomainConceptName.ConceptId = domainConceptName.ConceptId;
                newDomainConceptName.EffectiveVersionSequenceId = newConceptVersion.VersionSequenceId;

                // Insert the new concept domain name
                dataContext.ConceptNames.InsertOnSubmit(newDomainConceptName);

                return this.ConvertToModel(newDomainConceptName);
            }
            else
                return this.ConvertToModel(domainConceptName);
            
        }
    }
}