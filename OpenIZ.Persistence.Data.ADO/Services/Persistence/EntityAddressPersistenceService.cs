/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 *
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
 * User: justi
 * Date: 2016-6-18
 */
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using OpenIZ.OrmLite;


namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Represents a persistence service for entity addresses
    /// </summary>
    public class EntityAddressPersistenceService : IdentifiedPersistenceService<Core.Model.Entities.EntityAddress, DbEntityAddress>, IAdoAssociativePersistenceService
    {

        /// <summary>
        /// Get addresses from source
        /// </summary>
        public IEnumerable GetFromSource(DataContext context, Guid id, decimal? versionSequenceId, IPrincipal principal)
        {
            int tr = 0;
            return this.QueryInternal(context, base.BuildSourceQuery<EntityAddress>(id, versionSequenceId), 0, null, out tr, principal, false);
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override Core.Model.Entities.EntityAddress InsertInternal(DataContext context, EntityAddress data, IPrincipal principal)
        {

            // Ensure exists
            if (data.AddressUse != null) data.AddressUse = data.AddressUse?.EnsureExists(context, principal) as Concept;
            data.AddressUseKey = data.AddressUse?.Key ?? data.AddressUseKey;

            var retVal = base.InsertInternal(context, data, principal);

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<Core.Model.Entities.EntityAddressComponent, DbEntityAddressComponent>(
                   data.Component,
                    data,
                    context,
                    principal);

            return retVal;
        }

        /// <summary>
        /// Update the entity name
        /// </summary>
        public override Core.Model.Entities.EntityAddress UpdateInternal(DataContext context, Core.Model.Entities.EntityAddress data, IPrincipal principal)
        {

            // Ensure exists
            if (data.AddressUse != null) data.AddressUse = data.AddressUse?.EnsureExists(context, principal) as Concept;
            data.AddressUseKey = data.AddressUse?.Key ?? data.AddressUseKey;

            var retVal = base.UpdateInternal(context, data, principal);

            var sourceKey = data.Key.Value.ToByteArray();

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<Core.Model.Entities.EntityAddressComponent, DbEntityAddressComponent>(
                   data.Component,
                    data,
                    context,
                    principal);

            return retVal;
        }


    }

    /// <summary>
    /// Entity address component persistence service
    /// </summary>
    public class EntityAddressComponentPersistenceService : IdentifiedPersistenceService<Core.Model.Entities.EntityAddressComponent, DbEntityAddressComponent, CompositeResult<DbEntityAddressComponent, DbEntityAddressComponentValue>>, IAdoAssociativePersistenceService
    {

        /// <summary>
        /// To model instance
        /// </summary>
        public override EntityAddressComponent ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
        {
            if (dataInstance == null) return null;

            var addrComp = (dataInstance as CompositeResult)?.Values.OfType<DbEntityAddressComponent>().FirstOrDefault() ?? dataInstance as DbEntityAddressComponent;
            var addrValue = (dataInstance as CompositeResult)?.Values.OfType<DbEntityAddressComponentValue>().FirstOrDefault() ?? context.FirstOrDefault<DbEntityAddressComponentValue>(o => o.Key == addrComp.ValueKey);
            return new EntityAddressComponent()
            {
                ComponentTypeKey = addrComp.ComponentTypeKey,
                Value = addrValue.Value,
                Key = addrComp.Key,
                SourceEntityKey = addrComp.SourceKey
            };
        }

        /// <summary>
        /// From the model instance
        /// </summary>
        public override object FromModelInstance(Core.Model.Entities.EntityAddressComponent modelInstance, DataContext context, IPrincipal princpal)
        {
            var retVal = base.FromModelInstance(modelInstance, context, princpal) as DbEntityAddressComponent;

            // Address component already exists?
            var existing = context.FirstOrDefault<DbEntityAddressComponentValue>(o => o.Value == modelInstance.Value);
            if (existing != null && existing.Key != retVal.ValueKey)
                retVal.ValueKey = existing.Key;
            else
                retVal.ValueKey = context.Insert(new DbEntityAddressComponentValue()
                {
                    Value = modelInstance.Value
                }).Key;
            return retVal;
        }

        /// <summary>
        /// Entity address component
        /// </summary>
        public override Core.Model.Entities.EntityAddressComponent InsertInternal(DataContext context, Core.Model.Entities.EntityAddressComponent data, IPrincipal principal)
        {
            if (data.ComponentType != null) data.ComponentType = data.ComponentType?.EnsureExists(context, principal) as Concept;
            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.InsertInternal(context, data, principal);
        }

        /// <summary>
        /// Update 
        /// </summary>
        public override Core.Model.Entities.EntityAddressComponent UpdateInternal(DataContext context, Core.Model.Entities.EntityAddressComponent data, IPrincipal principal)
        {
            if (data.ComponentType != null) data.ComponentType = data.ComponentType?.EnsureExists(context, principal) as Concept;

            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.UpdateInternal(context, data, principal);
        }

        /// <summary>
        /// Get components from source
        /// </summary>
        public IEnumerable GetFromSource(DataContext context, Guid id, decimal? versionSequenceId, IPrincipal principal)
        {
            int tr = 0;
            return this.QueryInternal(context, base.BuildSourceQuery<EntityAddressComponent>(id), 0, null, out tr, principal, false);
        }

    }
}
