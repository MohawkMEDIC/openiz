/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-6-19
 */
using OpenIZ.Core.Model.Entities;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Represents a persistence service for entity addresses
    /// </summary>
    public class EntityAddressPersistenceService : IdentifiedPersistenceService<Core.Model.Entities.EntityAddress, Data.EntityAddress>
    {

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override Core.Model.Entities.EntityAddress Insert(Data.ModelDataContext context, Core.Model.Entities.EntityAddress data, IPrincipal principal)
        {

            // Ensure exists
            data.AddressUse?.EnsureExists(context, principal);
            data.AddressUseKey = data.AddressUse?.Key ?? data.AddressUseKey;

            var retVal = base.Insert(context, data, principal);

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<Core.Model.Entities.EntityAddressComponent, Data.EntityAddressComponent>(
                    data.Component,
                    data,
                    context,
                    principal);

            return retVal;
        }

        /// <summary>
        /// Update the entity name
        /// </summary>
        public override Core.Model.Entities.EntityAddress Update(ModelDataContext context, Core.Model.Entities.EntityAddress data, IPrincipal principal)
        {

            // Ensure exists
            data.AddressUse?.EnsureExists(context, principal);
            data.AddressUseKey = data.AddressUse?.Key ?? data.AddressUseKey;

            var retVal = base.Update(context, data, principal);

            var sourceKey = data.Key.Value.ToByteArray();

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<Core.Model.Entities.EntityAddressComponent, Data.EntityAddressComponent>(
                    data.Component,
                    data,
                    context,
                    principal);

            return retVal;
        }

        /// <summary>
        /// Data load options
        /// </summary>
        /// <returns></returns>
        internal override DataLoadOptions GetDataLoadOptions()
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<Data.EntityAddress>(c => c.AddressUseConcept);
            dlo.LoadWith<Data.EntityAddress>(c => c.EntityAddressComponents);
            dlo.LoadWith<Data.EntityAddressComponent>(c => c.ComponentTypeConcept);
            dlo.LoadWith<Data.EntityAddressComponent>(c => c.EntityAddressComponentValue);

            return dlo;
        }
    }

    /// <summary>
    /// Entity address component persistence service
    /// </summary>
    public class EntityAddressComponentPersistenceService : IdentifiedPersistenceService<Core.Model.Entities.EntityAddressComponent, Data.EntityAddressComponent>
    {
        /// <summary>
        /// From the model instance
        /// </summary>
        public override object FromModelInstance(Core.Model.Entities.EntityAddressComponent modelInstance, ModelDataContext context, IPrincipal princpal)
        {
            var retVal = base.FromModelInstance(modelInstance, context, princpal) as Data.EntityAddressComponent;

            // Address component already exists?
            var existing = context.EntityAddressComponentValues.FirstOrDefault(o => o.Value == modelInstance.Value);
            if (existing != null)
                retVal.ValueId = existing.ValueId;
            else
            {
                retVal.EntityAddressComponentValue = new EntityAddressComponentValue()
                {
                    ValueId = Guid.NewGuid(),
                    Value = modelInstance.Value
                };
            }

            return retVal;
        }

        /// <summary>
        /// Entity address component
        /// </summary>
        public override Core.Model.Entities.EntityAddressComponent Insert(ModelDataContext context, Core.Model.Entities.EntityAddressComponent data, IPrincipal principal)
        {
            data.ComponentType?.EnsureExists(context, principal);
            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update 
        /// </summary>
        public override Core.Model.Entities.EntityAddressComponent Update(ModelDataContext context, Core.Model.Entities.EntityAddressComponent data, IPrincipal principal)
        {
            data.ComponentType?.EnsureExists(context, principal);
            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.Update(context, data, principal);
        }
    }
}
