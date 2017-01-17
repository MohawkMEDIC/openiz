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

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Represents a persistence service for entity addresses
    /// </summary>
    public class EntityAddressPersistenceService : IdentifiedPersistenceService<Core.Model.Entities.EntityAddress, DbEntityAddress>
    {

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override Core.Model.Entities.EntityAddress Insert(Data.DataContext context, Core.Model.Entities.EntityAddress data, IPrincipal principal)
        {

            // Ensure exists
            data.AddressUse?.EnsureExists(context, principal);
            data.AddressUseKey = data.AddressUse?.Key ?? data.AddressUseKey;

            var retVal = base.Insert(context, data, principal);

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
        public override Core.Model.Entities.EntityAddress Update(DataContext context, Core.Model.Entities.EntityAddress data, IPrincipal principal)
        {

            // Ensure exists
            data.AddressUse?.EnsureExists(context, principal);
            data.AddressUseKey = data.AddressUse?.Key ?? data.AddressUseKey;

            var retVal = base.Update(context, data, principal);

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
    public class EntityAddressComponentPersistenceService : IdentifiedPersistenceService<Core.Model.Entities.EntityAddressComponent, DbEntityAddressComponent>
    {
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
        public override Core.Model.Entities.EntityAddressComponent Insert(DataContext context, Core.Model.Entities.EntityAddressComponent data, IPrincipal principal)
        {
            data.ComponentType?.EnsureExists(context, principal);
            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update 
        /// </summary>
        public override Core.Model.Entities.EntityAddressComponent Update(DataContext context, Core.Model.Entities.EntityAddressComponent data, IPrincipal principal)
        {
            data.ComponentType?.EnsureExists(context, principal);
            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.Update(context, data, principal);
        }
    }
}
