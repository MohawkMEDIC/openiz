using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Local batch repository service 
    /// </summary>
    public class LocalBatchRepositoryService
    {
        /// <summary>
		/// Insert the bundle
		/// </summary>
		public Bundle Insert(Bundle data)
        {
            data = this.Validate(data);
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();
            if (persistence == null)
                throw new InvalidOperationException("Missing persistence service");
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<Bundle>>();

            data = breService?.BeforeInsert(data) ?? data;
            data = persistence.Insert(data, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            data = breService?.AfterInsert(data) ?? data;

            return data;
        }

        /// <summary>
        /// Obsolete all the contents in the bundle
        /// </summary>
        public Bundle Obsolete(Bundle obsolete)
        {
            obsolete = this.Validate(obsolete);
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();
            if (persistence == null)
                throw new InvalidOperationException("Missing persistence service");
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<Bundle>>();

            obsolete = breService?.BeforeObsolete(obsolete) ?? obsolete;
            obsolete = persistence.Obsolete(obsolete, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            obsolete = breService?.AfterObsolete(obsolete) ?? obsolete;

            return obsolete;
        }

        /// <summary>
        /// Update the specified data in the bundle
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Bundle Update(Bundle data)
        {
            data = this.Validate(data);
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();
            if (persistence == null)
                throw new InvalidOperationException("Missing persistence service");
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<Bundle>>();

            // Entry point
            data = breService?.BeforeUpdate(data) ?? data;
            data = persistence.Insert(data, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            data = breService?.AfterUpdate(data) ?? data;
            return data;
        }

        /// <summary>
        /// Validate the bundle and its contents
        /// </summary>
        public Bundle Validate(Bundle bundle)
        {
            bundle = bundle.Clean() as Bundle;

            // BRE validation
            var breService = ApplicationContext.Current.GetService<IBusinessRulesService<Bundle>>();
            var issues = breService?.Validate(bundle);
            if (issues.Any(i => i.Priority == DetectedIssuePriorityType.Error))
                throw new DetectedIssueException(issues);

            // Bundle items
            for (int i = 0; i < bundle.Item.Count; i++)
            {
                var itm = bundle.Item[i];
                if (itm is Patient)
                    bundle.Item[i] = ApplicationContext.Current.GetService<IPatientRepositoryService>().Validate(itm as Patient);
                else if (itm is Act)
                    bundle.Item[i] = ApplicationContext.Current.GetService<IActRepositoryService>().Validate(itm as Act);
            }
            return bundle;
        }
    }
}
