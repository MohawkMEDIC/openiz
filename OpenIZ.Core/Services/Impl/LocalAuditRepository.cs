using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Data;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Represents an audit repository which stores and queries audit data.
    /// </summary>
    public class LocalAuditRepository : IAuditRepositoryService, IAuditEventSource
    {
        /// <summary>
        /// Fired when data has been created
        /// </summary>
        public event EventHandler<AuditDataEventArgs> DataCreated;
        /// <summary>
        /// Fired when data has been disclosed
        /// </summary>
        public event EventHandler<AuditDataDisclosureEventArgs> DataDisclosed;
        /// <summary>
        /// Fired when data has been obsoleted
        /// </summary>
        public event EventHandler<AuditDataEventArgs> DataObsoleted;
        /// <summary>
        /// Fired when data has been updated
        /// </summary>
        public event EventHandler<AuditDataEventArgs> DataUpdated;

        /// <summary>
        /// Find the specified data
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<AuditData> Find(Expression<Func<AuditData, bool>> query)
        {
            var tr = 0;
            return this.Find(query, 0, null, out tr);
        }

        /// <summary>
        /// Find with query controls
        /// </summary>
        public IEnumerable<AuditData> Find(Expression<Func<AuditData, bool>> query, int offset, int? count, out int totalResults)
        {
            var service = ApplicationContext.Current.GetService<IDataPersistenceService<AuditData>>();
            if (service == null)
                throw new InvalidOperationException("Cannot find the data persistence service for audits");
            var results = service.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
            this?.DataDisclosed(this, new AuditDataDisclosureEventArgs(query.ToString(), results));
            return results;
        }

        /// <summary>
        /// Gets the specified object
        /// </summary>
        public AuditData Get(Guid key)
        {
            return this.Get(key, Guid.Empty);
        }

        /// <summary>
        /// Gets the specified correlation key
        /// </summary>
        public AuditData Get(object correlationKey)
        {
            return this.Get((Guid)correlationKey, Guid.Empty);
        }

        /// <summary>
        /// Get the specified audit by key
        /// </summary>
        public AuditData Get(Guid key, Guid versionKey)
        {
            var service = ApplicationContext.Current.GetService<IDataPersistenceService<AuditData>>();
            if (service == null)
                throw new InvalidOperationException("Cannot find the data persistence service for audits");
            var result = service.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(key, versionKey), AuthenticationContext.Current.Principal, false);
            this?.DataDisclosed(this, new AuditDataDisclosureEventArgs(key.ToString(), new List<Object>() { result }));
            return result;
        }

        /// <summary>
        /// Insert the specified data
        /// </summary>
        public AuditData Insert(AuditData audit)
        {
            var service = ApplicationContext.Current.GetService<IDataPersistenceService<AuditData>>();
            if (service == null)
                throw new InvalidOperationException("Cannot find the data persistence service for audits");
            var result = service.Insert(audit, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            this?.DataCreated(this, new AuditDataEventArgs(audit));
            return result;
        }

        /// <summary>
        /// Obsolete the specified data
        /// </summary>
        public AuditData Obsolete(Guid key)
        {
            var service = ApplicationContext.Current.GetService<IDataPersistenceService<AuditData>>();
            if (service == null)
                throw new InvalidOperationException("Cannot find the data persistence service for audits");
            var result = service.Obsolete(new AuditData() { CorrelationToken = key }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            this?.DataObsoleted(this, new AuditDataEventArgs(key));
            return result;
        }

        /// <summary>
        /// Save (create or update) the specified object
        /// </summary>
        public AuditData Save(AuditData data)
        {
            var service = ApplicationContext.Current.GetService<IDataPersistenceService<AuditData>>();
            if (service == null)
                throw new InvalidOperationException("Cannot find the data persistence service for audits");

            var existing = service.Get(new Identifier<Guid>(data.CorrelationToken, Guid.Empty), AuthenticationContext.Current.Principal, false);
            if (existing == null)
            {
                data = service.Update(data, AuthenticationContext.Current.Principal, TransactionMode.Commit);
                this?.DataUpdated(this, new AuditDataEventArgs(data));
            }
            else
            {
                data = service.Insert(data, AuthenticationContext.Current.Principal, TransactionMode.Commit);
                this?.DataCreated(this, new AuditDataEventArgs(data));
            }
            return data;
        }
    }
}
