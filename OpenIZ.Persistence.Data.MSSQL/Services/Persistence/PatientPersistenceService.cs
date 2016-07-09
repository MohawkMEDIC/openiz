using OpenIZ.Core.Model.Roles;
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
    /// Persistence service which is used to persist patients
    /// </summary>
    public class PatientPersistenceService : IdentifiedPersistenceService<Core.Model.Roles.Patient, Data.Patient>
    {
        // Entity persisters
        private PersonPersistenceService m_personPersister = new PersonPersistenceService();
        protected EntityPersistenceService m_entityPersister = new EntityPersistenceService();

        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Core.Model.Roles.Patient modelInstance, ModelDataContext context, IPrincipal principal)
        {
            var dbPatient = base.FromModelInstance(modelInstance, context, principal) as Data.Patient;
            
            if (modelInstance.DeceasedDatePrecision.HasValue)
                dbPatient.DeceasedDatePrecision = PersonPersistenceService.PrecisionMap[modelInstance.DeceasedDatePrecision.Value];
            return dbPatient;
        }

        /// <summary>
        /// Model instance
        /// </summary>
        public override Core.Model.Roles.Patient ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var patient = dataInstance as Data.Patient;
            var dbe = context.GetTable<Data.EntityVersion>().Where(o => o.EntityVersionId == patient.EntityVersionId).First();
            var dbp = context.GetTable<Data.Person>().Where(o => o.EntityVersionId == patient.EntityVersionId).First();
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Roles.Patient>(dbe, context, principal);

            retVal.DateOfBirth = dbp.DateOfBirth;
            // Reverse lookup
            if (dbp.DateOfBirthPrecision.HasValue)
                retVal.DateOfBirthPrecision = PersonPersistenceService.PrecisionMap.Where(o => o.Value == dbp.DateOfBirthPrecision).Select(o => o.Key).First();

            retVal.DeceasedDate = patient.DeceasedDate;
            // Reverse lookup
            if (patient.DeceasedDatePrecision.HasValue)
                retVal.DeceasedDatePrecision = PersonPersistenceService.PrecisionMap.Where(o => o.Value == patient.DeceasedDatePrecision).Select(o => o.Key).First();
            retVal.MultipleBirthOrder = (int?)patient.MultipleBirthOrder;
            retVal.GenderConceptKey = patient.GenderConceptId;

            return retVal;
        }

        /// <summary>
        /// Insert the specified person into the database
        /// </summary>
        public override Core.Model.Roles.Patient Insert(ModelDataContext context, Core.Model.Roles.Patient data, IPrincipal principal)
        {
            data.GenderConcept?.EnsureExists(context, principal);
            data.GenderConceptKey = data.GenderConcept?.Key ?? data.GenderConceptKey;

            var inserted = this.m_personPersister.Insert(context, data, principal);
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update the specified person
        /// </summary>
        public override Core.Model.Roles.Patient Update(ModelDataContext context, Core.Model.Roles.Patient data, IPrincipal principal)
        {
            // Ensure exists
            data.GenderConcept?.EnsureExists(context, principal);
            data.GenderConceptKey = data.GenderConcept?.Key ?? data.GenderConceptKey;

            this.m_personPersister.Update(context, data, principal);
            return base.Update(context, data, principal);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override Core.Model.Roles.Patient Obsolete(ModelDataContext context, Core.Model.Roles.Patient data, IPrincipal principal)
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

            return loadOptions;
        }
    }
}
