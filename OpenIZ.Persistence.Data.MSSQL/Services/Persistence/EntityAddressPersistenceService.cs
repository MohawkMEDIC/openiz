using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Data.Linq;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Entitu address persistence service
    /// </summary>
    public class EntityAddressPersistenceService : VersionedAssociationPersistenceService<Core.Model.Entities.EntityAddress, Core.Model.Entities.Entity, Data.EntityAddress>
    {
        /// <summary>
        /// Get the data table
        /// </summary>
        protected override Table<Data.EntityAddress> GetDataTable(ModelDataContext context)
        {
            return context.EntityAddresses;
        }

        /// <summary>
        /// Insert an entity address creating a new version if required
        /// </summary>
        internal override Core.Model.Entities.EntityAddress Insert(Core.Model.Entities.EntityAddress storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            var domainAddress = this.ConvertFromModel(storageData) as Data.EntityAddress;

            // Ensure that the use code exists
            if (storageData.AddressUse != null)
                domainAddress.AddressUseConceptId = storageData.AddressUse.EnsureExists(principal, dataContext).Key;

            // Get the current version & create a new version if needed
            var currentEntityVersion = dataContext.EntityVersions.Single(o => o.EntityId == storageData.SourceEntityKey && o.ObsoletionTime == null);
            EntityVersion newEntityVersion = newVersion ? currentEntityVersion.NewVersion(principal, dataContext) : currentEntityVersion;
            domainAddress.EffectiveVersionSequenceId = newEntityVersion.VersionSequenceId;
            domainAddress.Entity = newEntityVersion.Entity;

            // Convert address components 
            foreach (var itm in storageData.Component)
            {
                var addressValue = dataContext.EntityAddressComponentValues.SingleOrDefault(o => o.Value == itm.Value);
                if (addressValue == null)
                    addressValue = new EntityAddressComponentValue() { Value = itm.Value };

                domainAddress.EntityAddressComponents.Add(new Data.EntityAddressComponent()
                {
                    ComponentTypeConceptId = itm.ComponentTypeKey == Guid.Empty ? (Guid?)null : itm.ComponentTypeKey,
                    EntityAddress = domainAddress,
                    EntityAddressComponentValue = addressValue
                });
            }
            dataContext.EntityAddresses.InsertOnSubmit(domainAddress);
            dataContext.SubmitChanges(); // Write and reload data from database

            // Copy properties
            storageData.Key = domainAddress.EntityAddressId;
            storageData.EffectiveVersionSequenceId = domainAddress.EffectiveVersionSequenceId;
            storageData.SourceEntityKey = domainAddress.EntityId;
            return storageData;

        }

        internal override Core.Model.Entities.EntityAddress Obsolete(Core.Model.Entities.EntityAddress storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            throw new NotImplementedException();
        }

        internal override Core.Model.Entities.EntityAddress Update(Core.Model.Entities.EntityAddress storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            throw new NotImplementedException();
        }
    }
}
