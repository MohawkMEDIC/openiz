using OpenIZ.Core.Model;
using OpenIZ.Persistence.Data.MSSQL.Data;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.DataTypes;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using System.ComponentModel;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Services;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Versioned domain data
    /// </summary>
    public abstract class VersionedDataPersistenceService<TModel, TDomain, TDomainKey> : BaseDataPersistenceService<TModel, TDomain> 
        where TDomain : class, IDbVersionedData<TDomainKey>, new() 
        where TModel : VersionedEntityData<TModel>, new()
        where TDomainKey : class, IDbIdentified, new()
    {
        
        /// <summary>
        /// Insert the data
        /// </summary>
        public override TModel Insert(ModelDataContext context, TModel data, IPrincipal principal)
        {

            // first we map the TDataKey entity
            var nonVersionedPortion = m_mapper.MapModelInstance<TModel, TDomainKey>(data);
            
            // Domain object
            var domainObject = this.FromModelInstance(data, context, principal) as TDomain;
            domainObject.NonVersionedObject = nonVersionedPortion;
            if (nonVersionedPortion.Id == Guid.Empty &&
                domainObject.Id != Guid.Empty)
                nonVersionedPortion.Id = domainObject.Id;

            if (nonVersionedPortion.Id == null ||
                nonVersionedPortion.Id == Guid.Empty)
            {
                data.Key = Guid.NewGuid();
                nonVersionedPortion.Id = data.Key.Value;
            }
            if (domainObject.VersionId == null ||
                domainObject.VersionId == Guid.Empty)
            {
                data.VersionKey = Guid.NewGuid();
                domainObject.VersionId = data.VersionKey.Value;
            }

            // Ensure created by exists
            data.CreatedBy?.EnsureExists(context, principal);
            data.CreatedByKey = domainObject.CreatedBy = domainObject.CreatedBy == Guid.Empty ? principal.GetUser(context).UserId : domainObject.CreatedBy;
            context.GetTable<TDomain>().InsertOnSubmit(domainObject);

            context.SubmitChanges();

            data.VersionSequence = domainObject.VersionSequenceId;
            data.VersionKey = domainObject.VersionId;
            data.Key = domainObject.Id;
            data.CreationTime = (DateTimeOffset)domainObject.CreationTime;

            return data;

        }

        /// <summary>
        /// Update the data with new version information
        /// </summary>
        public override TModel Update(ModelDataContext context, TModel data, IPrincipal principal)
        {

            if (data.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);

            // This is technically an insert and not an update
            var existingObject = context.GetTable<TDomain>().FirstOrDefault(ExpressionRewriter.Rewrite<TDomain>(o => o.Id == data.Key && !o.ObsoletionTime.HasValue)); // Get the last version (current)
            if (existingObject == null)
                throw new KeyNotFoundException(data.Key.ToString());
            else if (existingObject.IsReadonly)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.UpdatedReadonlyObject);

            // Map existing
            var storageInstance = this.FromModelInstance(data, context, principal);

            // Create a new version
            var user = principal.GetUser(context);
            var newEntityVersion = new TDomain();
            newEntityVersion.CopyObjectData(storageInstance);
            data.VersionSequence = newEntityVersion.VersionSequenceId = default(Decimal);
            data.VersionKey = newEntityVersion.VersionId = Guid.NewGuid();
            newEntityVersion.Id = data.Key.Value;
            data.PreviousVersionKey = newEntityVersion.ReplacesVersionId = existingObject.VersionId;
            data.CreatedByKey = newEntityVersion.CreatedBy = user.UserId;
            // Obsolete the old version 
            existingObject.ObsoletedBy = user.UserId;
            existingObject.ObsoletionTime = DateTime.Now;

            context.GetTable<TDomain>().InsertOnSubmit(newEntityVersion);
            context.SubmitChanges();

            // Pull database generated fields
            data.VersionSequence = newEntityVersion.VersionSequenceId;
            data.CreationTime = newEntityVersion.CreationTime;

            return data;
            //return base.Update(context, data, principal);
        }

        /// <summary>
        /// Gets the specified object
        /// </summary>
        public override TModel Get<TIdentifier>(MARC.HI.EHRS.SVC.Core.Data.Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
        {
            var tr = 0;
            var uuid = containerId as Identifier<Guid>;

            if (uuid.Id != Guid.Empty)
            {
                var cacheItem = ApplicationContext.Current.GetService<IDataCachingService>()?.GetCacheItem<TModel>(uuid.Id) as TModel;
                if (cacheItem != null && (cacheItem.VersionKey.HasValue && uuid.VersionId == cacheItem.VersionKey.Value || uuid.VersionId == Guid.Empty))
                    return cacheItem;
            }

            // Get most recent version
            if (uuid.VersionId == Guid.Empty)
                return base.Query(o => o.Key == uuid.Id && o.ObsoletionTime == null, 0, 1, principal, out tr).FirstOrDefault();
            else
                return base.Query(o => o.Key == uuid.Id && o.VersionKey == uuid.VersionId, 0, 1, principal, out tr).FirstOrDefault();
        }

        /// <summary>
        /// Update versioned association items
        /// </summary>
        internal virtual void UpdateVersionedAssociatedItems<TAssociation, TDomainAssociation>(List<TAssociation> storage, TModel source, ModelDataContext context, IPrincipal principal)
            where TAssociation : VersionedAssociation<TModel>, new()
            where TDomainAssociation : class, IDbVersionedAssociation, new()
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAssociation>>() as SqlServerBasePersistenceService<TAssociation>;
            if (persistenceService == null)
            {
                this.m_tracer.TraceEvent(System.Diagnostics.TraceEventType.Information, 0, "Missing persister for type {0}", typeof(TAssociation).Name);
                return;
            }
            // Ensure the source key is set
            foreach (var itm in storage)
                if (itm.SourceEntityKey == Guid.Empty ||
                    itm.SourceEntityKey == null)
                    itm.SourceEntityKey = source.Key;

            // Get existing
            var existing = context.GetTable<TDomainAssociation>().Where(ExpressionRewriter.Rewrite<TDomainAssociation>(o => o.AssociatedItemKey == source.Key && source.VersionSequence >= o.EffectiveVersionSequenceId && (source.VersionSequence < o.ObsoleteVersionSequenceId || !o.ObsoleteVersionSequenceId.HasValue))).ToList().Select(o => m_mapper.MapDomainInstance<TDomainAssociation, TAssociation>(o) as TAssociation);
            
            // Remove old
            var obsoleteRecords = existing.Where(o => !storage.Exists(ecn => ecn.Key == o.Key));
            foreach (var del in obsoleteRecords)
            {
                del.ObsoleteVersionSequenceId = source.VersionSequence;
                persistenceService.Update(context, del, principal);
            }

            // Update those that need it
            var updateRecords = storage.Where(o => existing.Any(ecn => ecn.Key == o.Key && o.Key != Guid.Empty && o != ecn));
            foreach (var upd in updateRecords)
                persistenceService.Update(context, upd, principal);

            // Insert those that do not exist
            var insertRecords = storage.Where(o => !existing.Any(ecn => ecn.Key == o.Key));
            foreach (var ins in insertRecords)
            {
                ins.EffectiveVersionSequenceId = source.VersionSequence;
                persistenceService.Insert(context, ins, principal);
            }
        }
    }
}
