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
    /// Provider persistence service
    /// </summary>
    public class ProviderPersistenceService : IdentifiedPersistenceService<Core.Model.Roles.Provider, Data.Provider>
    {
        // Entity persisters
        private PersonPersistenceService m_personPersister = new PersonPersistenceService();
        protected EntityPersistenceService m_entityPersister = new EntityPersistenceService();

        /// <summary>
        /// Model instance
        /// </summary>
        public override Core.Model.Roles.Provider ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var provider = dataInstance as Data.Provider;
            var dbe = context.GetTable<Data.EntityVersion>().Where(o => o.EntityVersionId == provider.EntityVersionId).First();
            var dbp = context.GetTable<Data.Person>().Where(o => o.EntityVersionId == provider.EntityVersionId).First();
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Roles.Provider>(dbe, context, principal);

            retVal.DateOfBirth = dbp.DateOfBirth;
            // Reverse lookup
            if (dbp.DateOfBirthPrecision.HasValue)
                retVal.DateOfBirthPrecision = PersonPersistenceService.PrecisionMap.Where(o => o.Value == dbp.DateOfBirthPrecision).Select(o => o.Key).First();

            retVal.ProviderSpecialtyKey = provider.ProviderSpecialtyConceptId;

            return retVal;
        }

        /// <summary>
        /// Insert the specified person into the database
        /// </summary>
        public override Core.Model.Roles.Provider Insert(ModelDataContext context, Core.Model.Roles.Provider data, IPrincipal principal)
        {
            data.ProviderSpecialty?.EnsureExists(context, principal);
            data.ProviderSpecialtyKey = data.ProviderSpecialty?.Key ?? data.ProviderSpecialtyKey;

            var inserted = this.m_personPersister.Insert(context, data, principal);
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update the specified person
        /// </summary>
        public override Core.Model.Roles.Provider Update(ModelDataContext context, Core.Model.Roles.Provider data, IPrincipal principal)
        {
            // Ensure exists
            data.ProviderSpecialty?.EnsureExists(context, principal);
            data.ProviderSpecialtyKey = data.ProviderSpecialty?.Key ?? data.ProviderSpecialtyKey;

            this.m_personPersister.Update(context, data, principal);
            return base.Update(context, data, principal);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override Core.Model.Roles.Provider Obsolete(ModelDataContext context, Core.Model.Roles.Provider data, IPrincipal principal)
        {
            var retVal = this.m_personPersister.Obsolete(context, data, principal);
            return data;
        }

        /// <summary>
        /// Get data load options
        /// </summary>
        /// <returns></returns>
        protected override DataLoadOptions GetDataLoadOptions()
        {
            var loadOptions = base.GetDataLoadOptions();
            loadOptions.LoadWith<Data.EntityVersion>(cs => cs.StatusConcept);
            loadOptions.LoadWith<Data.EntityVersion>(cs => cs.TypeConcept);
            loadOptions.LoadWith<Data.Entity>(cs => cs.EntityTags);
            loadOptions.LoadWith<Data.Entity>(cs => cs.EntityNames);
            loadOptions.LoadWith<Data.Entity>(cs => cs.EntityIdentifiers);
            loadOptions.LoadWith<Data.Entity>(cs => cs.EntityAddresses);
            loadOptions.LoadWith<Data.Entity>(cs => cs.EntityTelecomAddresses);
            loadOptions.LoadWith<Data.EntityName>(cs => cs.EntityNameComponents);
            loadOptions.LoadWith<Data.EntityAddress>(cs => cs.EntityAddressComponents);
            loadOptions.LoadWith<Data.EntityNameComponent>(cs => cs.PhoneticValue);
            loadOptions.LoadWith<Data.EntityAddressComponent>(cs => cs.EntityAddressComponentValue);
            loadOptions.LoadWith<Data.EntityIdentifier>(cs => cs.AssigningAuthority);
            loadOptions.LoadWith<Data.EntityTelecomAddress>(cs => cs.TelecomUseConcept);
            loadOptions.LoadWith<Data.EntityAssociation>(cs => cs.AssociationTypeConcept);
            loadOptions.LoadWith<Data.EntityExtension>(cs => cs.ExtensionType);
            loadOptions.LoadWith<Data.Provider>(cs => cs.ProviderSpecialtyConcept);

            return loadOptions;
        }
    }
}
