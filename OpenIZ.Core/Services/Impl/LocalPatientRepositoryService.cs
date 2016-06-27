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


        // Repository service
        private IDataPersistenceService<Patient> m_persistenceService;

        /// <summary>
        /// Concept set ctor
        /// </summary>
        public LocalPatientRepositoryService()
        {
            ApplicationContext.Current.Started += (o, e) => this.m_persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
        }

        /// <summary>
        /// Locates the specified patient
        /// </summary>
        public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate)
        {
            return this.m_persistenceService.Query(predicate, AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Finds the specified patient with query controls
        /// </summary>
        public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate, int offset, int? count, out int totalCount)
        {
            return this.m_persistenceService.Query(predicate, offset, count, AuthenticationContext.Current.Principal, out totalCount);
        }

        /// <summary>
        /// Gets the specified patient
        /// </summary>
        public IdentifiedData Get(Guid id, Guid versionId)
        {
            return this.m_persistenceService.Get<Guid>(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
        }

        /// <summary>
        /// Insert the specified patient
        /// </summary>
        public Patient Insert(Patient p)
        {
            return this.m_persistenceService.Insert(p, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Merges two patients together
        /// </summary>
        public Patient Merge(Patient survivor, Patient victim)
        {
            // TODO: Do this
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obsoletes the specified patient
        /// </summary>
        public Patient Obsolete(Guid key)
        {
            return this.m_persistenceService.Obsolete(new Patient() { Key = key }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Save / update the patient
        /// </summary>
        public Patient Save(Patient p)
        {
            try
            {
                return this.m_persistenceService.Update(p, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
            catch (KeyNotFoundException)
            {
                return this.m_persistenceService.Insert(p, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
        }

        /// <summary>
        /// Un-merge two patients
        /// </summary>
        public Patient UnMerge(Patient patient, Guid versionKey)
        {
            throw new NotImplementedException();
        }
    }
}
