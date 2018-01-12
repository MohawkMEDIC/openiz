/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
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
    public class PatientPersistenceService : SimpleVersionedEntityPersistenceService<Core.Model.Roles.Patient, Data.Patient>
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
            var iddat = dataInstance as IDbVersionedData;
            var patient = dataInstance as Data.Patient ?? context.GetTable<Data.Patient>().Where(o => o.EntityVersionId == iddat.VersionId).First();
            var dbe = dataInstance as Data.EntityVersion ?? context.GetTable<Data.EntityVersion>().Where(o => o.EntityVersionId == patient.EntityVersionId).First();
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
            if (dbe.Entity.PersonLanguageCommunicationsPersonEntityId != null)
                retVal.LanguageCommunication = dbe.Entity.PersonLanguageCommunicationsPersonEntityId.Where(v => v.EffectiveVersionSequenceId <= dbe.VersionSequenceId && (v.ObsoleteVersionSequenceId == null || v.ObsoleteVersionSequenceId >= dbe.VersionSequenceId))
                    .Select(o => new Core.Model.Entities.PersonLanguageCommunication(o.LanguageCommunication, o.PreferenceIndicator)
                    {
                        Key = o.PersonLanguageCommunicationId
                    })
                    .ToList();

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
            return base.Insert(context, data, principal);
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
        internal override DataLoadOptions GetDataLoadOptions()
        {
            var loadOptions = this.m_entityPersister.GetDataLoadOptions();
            loadOptions.LoadWith<Data.Patient>(cs => cs.GenderConcept);

            return loadOptions;
        }
    }
}
