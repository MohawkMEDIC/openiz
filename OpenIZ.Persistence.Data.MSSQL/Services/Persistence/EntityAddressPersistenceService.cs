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
 * Date: 2016-4-19
 */
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
        /// Convert to model
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal override Core.Model.Entities.EntityAddress ConvertToModel(object data)
        {
            if (data == null)
                return null;

            var address = data as Data.EntityAddress;

            var retVal = DataCache.Current.Get(address.EntityAddressId) as Core.Model.Entities.EntityAddress;
            if (retVal == null)
            {
                retVal = this.ConvertItem(address);

                ConceptPersistenceService cp = new ConceptPersistenceService();
                if (address.AddressUseConcept != null)
                    retVal.AddressUse = cp.ConvertItem(address.AddressUseConcept.CurrentVersion());
                if (address.EntityAddressComponents != null)
                {
                    retVal.Component = new List<Core.Model.Entities.EntityAddressComponent>();
                    retVal.Component.AddRange(
                        address.EntityAddressComponents.Select(o => new Core.Model.Entities.EntityAddressComponent()
                        {
                            ComponentTypeKey = o.ComponentTypeConceptId,
                            Key = o.EntityAddressComponentId,
                            Value = o.EntityAddressComponentValue.Value,
                            SourceEntityKey = retVal.Key
                        }));
                }
            }

            return retVal;
        }

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
