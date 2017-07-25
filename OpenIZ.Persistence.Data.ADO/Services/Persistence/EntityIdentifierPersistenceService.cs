using OpenIZ.Core.Model.DataTypes;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Data.ADO.Data.Model.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Security.Principal;
using OpenIZ.Persistence.Data.ADO.Data;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Entity identifier persistence service
    /// </summary>
    public class EntityIdentifierPersistenceService : IdentifiedPersistenceService<EntityIdentifier, DbEntityIdentifier, CompositeResult<DbEntityIdentifier, DbAssigningAuthority>>, IAdoAssociativePersistenceService
    {

        /// <summary>
        /// Convert to model
        /// </summary>
        public override EntityIdentifier ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
        {
            var identifier = (dataInstance as CompositeResult).Values.OfType<DbEntityIdentifier>().FirstOrDefault() ?? dataInstance as DbEntityIdentifier;
            var authority = (dataInstance as CompositeResult)?.Values.OfType<DbAssigningAuthority>().FirstOrDefault();
            return new EntityIdentifier()
            {
                AuthorityKey = identifier.AuthorityKey,
                Authority = authority != null ? new AssigningAuthority(authority.DomainName, authority.Name, authority.Oid) { Key = authority.Key } : null,
                EffectiveVersionSequenceId = identifier.EffectiveVersionSequenceId,
                IdentifierTypeKey = identifier.TypeKey,
                LoadState = Core.Model.LoadState.PartialLoad,
                Key = identifier.Key,
                SourceEntityKey = identifier.SourceKey,
                ObsoleteVersionSequenceId = identifier.ObsoleteVersionSequenceId,
                Value = identifier.Value
            };
        }

        /// <summary>
        /// Get from source
        /// </summary>
        public IEnumerable GetFromSource(DataContext context, Guid id, decimal? versionSequenceId, IPrincipal principal)
        {
            int tr = 0;
            return this.QueryInternal(context, base.BuildSourceQuery<EntityIdentifier>(id, versionSequenceId), Guid.Empty, 0, null, out tr, principal, false).ToList();
        }

        /// <summary>
        /// Insert the entity identifier
        /// </summary>
        public override EntityIdentifier InsertInternal(DataContext context, EntityIdentifier data, IPrincipal principal)
        {
            if (data.Authority != null) data.Authority = data.Authority.EnsureExists(context, principal) as AssigningAuthority;
            data.AuthorityKey = data.Authority?.Key ?? data.AuthorityKey;
            return base.InsertInternal(context, data, principal);
        }

        /// <summary>
        /// Update the data
        /// </summary>
        public override EntityIdentifier UpdateInternal(DataContext context, EntityIdentifier data, IPrincipal principal)
        {
            if (data.Authority != null) data.Authority = data.Authority.EnsureExists(context, principal) as AssigningAuthority;
            data.AuthorityKey = data.Authority?.Key ?? data.AuthorityKey;
            return base.UpdateInternal(context, data, principal);
        }

    }
}
