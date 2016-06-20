using OpenIZ.Core.Model.Entities;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System;
using System.Collections.Generic;
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

            var retVal = base.Update(context, data, principal);

            var sourceKey = data.Key.ToByteArray();

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<Core.Model.Entities.EntityAddressComponent, Data.EntityAddressComponent>(
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
                retVal.EntityAddressComponentValue = existing;
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
    }
}
