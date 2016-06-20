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
    /// Entity name persistence service
    /// </summary>
    public class EntityNamePersistenceService : IdentifiedPersistenceService<Core.Model.Entities.EntityName, Data.EntityName>
    {

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override Core.Model.Entities.EntityName Insert(ModelDataContext context, Core.Model.Entities.EntityName data, IPrincipal principal)
        {

            // Ensure exists
            data.NameUse?.EnsureExists(context, principal);

            var retVal = base.Insert(context, data, principal);

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<Core.Model.Entities.EntityNameComponent, Data.EntityNameComponent>(
                    data.Component, 
                    data, 
                    context,
                    principal);

            return retVal;
        }

        /// <summary>
        /// Update the entity name
        /// </summary>
        public override Core.Model.Entities.EntityName Update(ModelDataContext context, Core.Model.Entities.EntityName data, IPrincipal principal)
        {
            // Ensure exists
            data.NameUse?.EnsureExists(context, principal);

            var retVal = base.Update(context, data, principal);

            var sourceKey = data.Key.ToByteArray();

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<Core.Model.Entities.EntityNameComponent, Data.EntityNameComponent>(
                    data.Component,
                    data,
                    context,
                    principal);

            return retVal;
        }

    }

}
