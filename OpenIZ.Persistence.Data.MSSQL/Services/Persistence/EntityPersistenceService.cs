using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Constants;
using System.Security.Principal;
using OpenIZ.Persistence.Data.MSSQL.Data;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Entity persistence service
    /// </summary>
    public class EntityPersistenceService : VersionedDataPersistenceService<Core.Model.Entities.Entity, Data.EntityVersion, Data.Entity>
    {

        /// <summary>
        /// To model instance
        /// </summary>
        public virtual TEntityType ToModelInstance<TEntityType>(Data.EntityVersion dbInstance, Data.ModelDataContext context, IPrincipal principal) where TEntityType : Core.Model.Entities.Entity, new()
        {
            var retVal = m_mapper.MapDomainInstance<Data.EntityVersion, TEntityType>(dbInstance);
            retVal.ClassConceptKey = dbInstance.Entity.ClassConceptId;
            retVal.DeterminerConceptKey = dbInstance.Entity.DeterminerConceptId;
            return retVal;
        }
        
        /// <summary>
        /// Insert the specified entity into the data context
        /// </summary>
        public override Core.Model.Entities.Entity Insert(ModelDataContext context, Core.Model.Entities.Entity data, IPrincipal principal)
        {

            // Ensure FK exists
            data.ClassConcept?.EnsureExists(context,principal);
            data.DeterminerConcept?.EnsureExists(context, principal);
            data.StatusConcept?.EnsureExists(context, principal);
            data.TypeConcept?.EnsureExists(context, principal);
            data.StatusConceptKey = data.StatusConceptKey == Guid.Empty ? StatusKeys.New : data.StatusConceptKey;

            var retVal = base.Insert(context, data, principal);

            // Identifiers
            if (data.Identifiers != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityIdentifier, Data.EntityIdentifier>(
                    data.Identifiers,
                    retVal,
                    context,
                    principal);

            // Relationships
            if (data.Relationships != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityRelationship, Data.EntityAssociation>(
                    data.Relationships,
                    retVal,
                    context,
                    principal);

            // Telecoms
            if (data.Telecoms != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityTelecomAddress, Data.EntityTelecomAddress>(
                    data.Telecoms,
                    retVal,
                    context,
                    principal);

            // Extensions
            if (data.Extensions != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityExtension, Data.EntityExtension>(
                    data.Extensions,
                    retVal,
                    context,
                    principal);

            // Names
            if (data.Names != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityName, Data.EntityName>(
                    data.Names,
                    retVal,
                    context,
                    principal);

            // Addresses
            if (data.Addresses != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityAddress, Data.EntityAddress>(
                    data.Addresses,
                    retVal,
                    context,
                    principal);

            // Notes
            if (data.Notes != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityNote, Data.EntityNote>(
                    data.Notes,
                    retVal,
                    context,
                    principal);

            // Tags
            if (data.Tags != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.EntityTag, Data.EntityTag>(
                    data.Tags,
                    retVal,
                    context,
                    principal);

            return retVal;
        }

        /// <summary>
        /// Update the specified entity
        /// </summary>
        public override Core.Model.Entities.Entity Update(ModelDataContext context, Core.Model.Entities.Entity data, IPrincipal principal)
        {
            // Esnure exists
            data.ClassConcept?.EnsureExists(context, principal);
            data.DeterminerConcept?.EnsureExists(context, principal);
            data.StatusConcept?.EnsureExists(context, principal);
            data.TypeConcept?.EnsureExists(context, principal);

            var retVal = base.Update(context, data, principal);


            // Identifiers
            if (data.Identifiers != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityIdentifier, Data.EntityIdentifier>(
                    data.Identifiers,
                    retVal,
                    context,
                    principal);

            // Relationships
            if (data.Relationships != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityRelationship, Data.EntityAssociation>(
                    data.Relationships,
                    retVal,
                    context,
                    principal);

            // Telecoms
            if (data.Telecoms != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityTelecomAddress, Data.EntityTelecomAddress>(
                    data.Telecoms,
                    retVal,
                    context,
                    principal);

            // Extensions
            if (data.Extensions != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityExtension, Data.EntityExtension>(
                    data.Extensions,
                    retVal,
                    context,
                    principal);

            // Names
            if (data.Names != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityName, Data.EntityName>(
                    data.Names,
                    retVal,
                    context,
                    principal);

            // Addresses
            if (data.Addresses != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityAddress, Data.EntityAddress>(
                    data.Addresses,
                    retVal,
                    context,
                    principal);

            // Notes
            if (data.Notes != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityNote, Data.EntityNote>(
                    data.Notes,
                    retVal,
                    context,
                    principal);

            // Tags
            if (data.Tags != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.EntityTag, Data.EntityTag>(
                    data.Tags,
                    retVal,
                    context,
                    principal);

            return retVal;
        }

        /// <summary>
        /// Obsoleted status key
        /// </summary>
        public override Core.Model.Entities.Entity Obsolete(ModelDataContext context, Core.Model.Entities.Entity data, IPrincipal principal)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.Obsolete(context, data, principal);
        }
    }
}
