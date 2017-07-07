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
 * Date: 2017-1-21
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using System.Security.Principal;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.OrmLite;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Person persistence service
    /// </summary>
    /// <remarks>This is a little different than the other persisters as we have to 
    /// persist half the object in one set of tables ane the other fields in this
    /// table</remarks>
    public class PersonPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.Person, DbPerson, CompositeResult<DbPerson, DbEntityVersion, DbEntity>>
    {
        // Map
        public static readonly Dictionary<DatePrecision, String> PrecisionMap = new Dictionary<DatePrecision, String>()
        {
            { DatePrecision.Day, "D" },
            { DatePrecision.Hour, "h"},
            { DatePrecision.Minute, "m" },
            { DatePrecision.Month, "M" },
            { DatePrecision.Second, "s" },
            { DatePrecision.Year, "Y" }
        };

        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Core.Model.Entities.Person modelInstance, DataContext context, IPrincipal principal)
        {
            var dbPerson = base.FromModelInstance(modelInstance, context, principal) as DbPerson;

            if (modelInstance.DateOfBirthPrecision.HasValue)
                dbPerson.DateOfBirthPrecision = PrecisionMap[modelInstance.DateOfBirthPrecision.Value];
            return dbPerson;
        }

        /// <summary>
        /// Model instance
        /// </summary>
        public Core.Model.Entities.Person ToModelInstance(DbPerson personInstance, DbEntityVersion entityVersionInstance, DbEntity entityInstance, DataContext context, IPrincipal principal)
        {

            var retVal = m_entityPersister.ToModelInstance<Person>(entityVersionInstance, entityInstance, context, principal);
            if (retVal == null || personInstance == null) return retVal;
            retVal.DateOfBirth = personInstance.DateOfBirth;

            // Reverse lookup
            if (!String.IsNullOrEmpty(personInstance?.DateOfBirthPrecision))
                retVal.DateOfBirthPrecision = PrecisionMap.Where(o => o.Value == personInstance.DateOfBirthPrecision).Select(o => o.Key).First();

            retVal.LanguageCommunication = context.Query<DbPersonLanguageCommunication>(v => v.SourceKey == entityInstance.Key && v.EffectiveVersionSequenceId <= entityVersionInstance.VersionSequenceId && (v.ObsoleteVersionSequenceId == null || v.ObsoleteVersionSequenceId > entityVersionInstance.VersionSequenceId))
                    .Select(o => new Core.Model.Entities.PersonLanguageCommunication(o.LanguageCode, o.IsPreferred)
                    {
                        Key = o.Key
                    })
                    .ToList();

            return retVal;
        }

        /// <summary>
        /// Inserts the specified person
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override Core.Model.Entities.Person InsertInternal(DataContext context, Core.Model.Entities.Person data, IPrincipal principal)
        {
            var retVal = base.InsertInternal(context, data, principal);
            //byte[] sourceKey = retVal.Key.Value.ToByteArray();

            // Language communication
            if (data.LanguageCommunication != null)
                this.m_entityPersister.UpdateVersionedAssociatedItems<Core.Model.Entities.PersonLanguageCommunication,DbPersonLanguageCommunication>(
                   data.LanguageCommunication,
                    retVal,
                    context,
                    principal);
            return retVal;
        }

        /// <summary>
        /// Update the person entity
        /// </summary>
        public override Core.Model.Entities.Person UpdateInternal(DataContext context, Core.Model.Entities.Person data, IPrincipal principal)
        {
            var retVal = base.UpdateInternal(context, data, principal);
            var sourceKey = retVal.Key.Value.ToByteArray();

            // Language communication
            if (data.LanguageCommunication != null)
                this.m_entityPersister.UpdateVersionedAssociatedItems<Core.Model.Entities.PersonLanguageCommunication,DbPersonLanguageCommunication>(
                   data.LanguageCommunication,
                    retVal,
                    context,
                    principal);
            return retVal;
        }
    }
}
