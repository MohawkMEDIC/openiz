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
 * Date: 2016-7-18
 */
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
            var iddat = dataInstance as IDbVersionedData;
            var provider = dataInstance as Data.Provider ?? context.GetTable<Data.Provider>().Where(o => o.EntityVersionId == iddat.VersionId).First();
            var dbe = dataInstance as Data.EntityVersion ?? context.GetTable<Data.EntityVersion>().Where(o => o.EntityVersionId == provider.EntityVersionId).First();
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
        internal override DataLoadOptions GetDataLoadOptions()
        {
            var loadOptions = this.m_entityPersister.GetDataLoadOptions();
            loadOptions.LoadWith<Data.Provider>(cs => cs.ProviderSpecialtyConcept);

            return loadOptions;
        }
    }
}
