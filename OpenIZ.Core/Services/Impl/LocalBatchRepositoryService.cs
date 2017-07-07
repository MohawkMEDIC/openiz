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
 * Date: 2016-11-30
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Security;
using System;
using System.Linq;
using OpenIZ.Core.Interfaces;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Local batch repository service
	/// </summary>
	public class LocalBatchRepositoryService : IBatchRepositoryService, IRepositoryService<Bundle>
	{
        public event EventHandler<AuditDataEventArgs> DataCreated;
        public event EventHandler<AuditDataDisclosureEventArgs> DataDisclosed;
        public event EventHandler<AuditDataEventArgs> DataObsoleted;
        public event EventHandler<AuditDataEventArgs> DataUpdated;

        public IEnumerable<Bundle> Find(Expression<Func<Bundle, bool>> query)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<Bundle> Find(Expression<Func<Bundle, bool>> query, int offset, int? count, out int totalResults)
        {
            throw new NotSupportedException();
        }

        public Bundle Get(Guid key)
        {
            throw new NotSupportedException();
        }

        public Bundle Get(Guid key, Guid versionKey)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Insert the bundle
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        /// <returns>Returns the created bundle.</returns>
        /// <exception cref="System.InvalidOperationException">Missing persistence service</exception>
        public Bundle Insert(Bundle bundle)
		{
			bundle = this.Validate(bundle);
			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();
			if (persistence == null)
				throw new InvalidOperationException("Missing persistence service");
			var breService = ApplicationContext.Current.GetService<IBusinessRulesService<Bundle>>();

			bundle = breService?.BeforeInsert(bundle) ?? bundle;
			bundle = persistence.Insert(bundle, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			breService?.AfterInsert(bundle);
			return bundle;
		}

        public Bundle Obsolete(Guid key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Obsolete all the contents in the bundle.
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        /// <returns>Returns the obsoleted bundle.</returns>
        /// <exception cref="System.InvalidOperationException">Missing persistence service</exception>
        public Bundle Obsolete(Bundle bundle)
		{
			bundle = this.Validate(bundle);
			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();
			if (persistence == null)
				throw new InvalidOperationException("Missing persistence service");
			var breService = ApplicationContext.Current.GetService<IBusinessRulesService<Bundle>>();

			bundle = breService?.BeforeObsolete(bundle) ?? bundle;
			bundle = persistence.Obsolete(bundle, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			bundle = breService?.AfterObsolete(bundle) ?? bundle;

			return bundle;
		}

        /// <summary>
        /// Save bundle data
        /// </summary>
        public Bundle Save(Bundle data)
        {
            return this.Update(data);
        }

        /// <summary>
        /// Update the specified data in the bundle.
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        /// <returns>Returns the updated bundle.</returns>
        /// <exception cref="System.InvalidOperationException">Missing persistence service</exception>
        public Bundle Update(Bundle bundle)
		{
			bundle = this.Validate(bundle);

			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();

			if (persistence == null)
			{
				throw new InvalidOperationException("Missing persistence service");
			}

			var breService = ApplicationContext.Current.GetService<IBusinessRulesService<Bundle>>();

			try
			{
				// Entry point
				bundle = breService?.BeforeUpdate(bundle) ?? bundle;

				bundle = persistence.Update(bundle, AuthenticationContext.Current.Principal, TransactionMode.Commit);

                breService?.AfterUpdate(bundle);
                return bundle;
			}
			catch (DataPersistenceException)
			{
				bundle = breService?.BeforeInsert(bundle) ?? bundle;

				bundle = persistence.Insert(bundle, AuthenticationContext.Current.Principal, TransactionMode.Commit);

                breService?.AfterInsert(bundle);
                return bundle;
			}

			return bundle;
		}

		/// <summary>
		/// Validate the bundle and its contents.
		/// </summary>
		/// <param name="bundle">The bundle.</param>
		/// <returns>Returns the bundle.</returns>
		/// <exception cref="DetectedIssueException">If there are any detected issues when validating the bundle.</exception>
		public Bundle Validate(Bundle bundle)
		{
			bundle = bundle.Clean() as Bundle;

			// BRE validation
			var breService = ApplicationContext.Current.GetService<IBusinessRulesService<Bundle>>();
			var issues = breService?.Validate(bundle);
			if (issues?.Any(i => i.Priority == DetectedIssuePriorityType.Error) == true)
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