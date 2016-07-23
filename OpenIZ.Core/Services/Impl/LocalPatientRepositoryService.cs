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
 * Date: 2016-6-28
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Roles;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Data;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Local patient repository service
    /// </summary>
    public class LocalPatientRepositoryService : IPatientRepositoryService
    {

        
        /// <summary>
        /// Locates the specified patient
        /// </summary>
        public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Query(predicate, AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Finds the specified patient with query controls
        /// </summary>
        public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate, int offset, int? count, out int totalCount)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Query(predicate, offset, count, AuthenticationContext.Current.Principal, out totalCount);
        }

        /// <summary>
        /// Gets the specified patient
        /// </summary>
        public IdentifiedData Get(Guid id, Guid versionId)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Get<Guid>(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
        }

        /// <summary>
        /// Insert the specified patient
        /// </summary>
        public Patient Insert(Patient p)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Insert(p, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Merges two patients together
        /// </summary>
        public Patient Merge(Patient survivor, Patient victim)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            // TODO: Do this
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obsoletes the specified patient
        /// </summary>
        public Patient Obsolete(Guid key)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Obsolete(new Patient() { Key = key }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Save / update the patient
        /// </summary>
        public Patient Save(Patient p)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            try
            {
                return persistenceService.Update(p, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
            catch (KeyNotFoundException)
            {
                return persistenceService.Insert(p, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
        }

        /// <summary>
        /// Un-merge two patients
        /// </summary>
        public Patient UnMerge(Patient patient, Guid versionKey)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");

            throw new NotImplementedException();
        }
    }
}
