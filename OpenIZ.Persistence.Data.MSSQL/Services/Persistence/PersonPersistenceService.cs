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
 * Date: 2016-6-22
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Person persistence service
    /// </summary>
    /// <remarks>This is a little different than the other persisters as we have to 
    /// persist half the object in one set of tables ane the other fields in this
    /// table</remarks>
    public class PersonPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.Person, Data.Person>
    {
        // Map
        public static readonly Dictionary<DatePrecision, Char> PrecisionMap = new Dictionary<DatePrecision, Char>()
        {
            { DatePrecision.Day, 'D' },
            { DatePrecision.Hour, 'h' },
            { DatePrecision.Minute, 'm' },
            { DatePrecision.Month, 'M' },
            { DatePrecision.Second, 's' },
            { DatePrecision.Year, 'Y' }
        };

        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Core.Model.Entities.Person modelInstance, ModelDataContext context, IPrincipal principal)
        {
            var dbPerson = base.FromModelInstance(modelInstance, context, principal) as Data.Person;

            if (modelInstance.DateOfBirthPrecision.HasValue)
                dbPerson.DateOfBirthPrecision = PrecisionMap[modelInstance.DateOfBirthPrecision.Value];
            return dbPerson;
        }

        /// <summary>
        /// Model instance
        /// </summary>
        public override Core.Model.Entities.Person ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {

            var iddat = dataInstance as IDbVersionedData;
            var person = dataInstance as Data.Person ?? context.GetTable<Data.Person>().Where(o => o.EntityVersionId == iddat.VersionId).FirstOrDefault();
            var dbe = dataInstance as Data.EntityVersion ?? context.GetTable<Data.EntityVersion>().Where(o => o.EntityVersionId == person.EntityVersionId).First();
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Entities.Person>(dbe, context, principal);
            retVal.DateOfBirth = person?.DateOfBirth;

            // Reverse lookup
            if (person?.DateOfBirthPrecision.HasValue == true)
                retVal.DateOfBirthPrecision = PrecisionMap.Where(o => o.Value == person.DateOfBirthPrecision).Select(o => o.Key).First();

            return retVal;
        }

        /// <summary>
        /// Inserts the specified person
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override Core.Model.Entities.Person Insert(ModelDataContext context, Core.Model.Entities.Person data, IPrincipal principal)
        {
            var retVal = base.Insert(context, data, principal);
            byte[] sourceKey = retVal.Key.Value.ToByteArray();

            // Language communication
            if (data.LanguageCommunication != null)
                this.m_entityPersister.UpdateVersionedAssociatedItems<Core.Model.Entities.PersonLanguageCommunication, Data.PersonLanguageCommunication>(
                    data.LanguageCommunication,
                    retVal,
                    context,
                    principal);
            return retVal;
        }

        /// <summary>
        /// Update the person entity
        /// </summary>
        public override Core.Model.Entities.Person Update(ModelDataContext context, Core.Model.Entities.Person data, IPrincipal principal)
        {
            var retVal = base.Update(context, data, principal);
            var sourceKey = retVal.Key.Value.ToByteArray();

            // Language communication
            if (data.LanguageCommunication != null)
                this.m_entityPersister.UpdateVersionedAssociatedItems<Core.Model.Entities.PersonLanguageCommunication, Data.PersonLanguageCommunication>(
                    data.LanguageCommunication,
                    retVal,
                    context,
                    principal);
            return retVal;
        }
    }
}
