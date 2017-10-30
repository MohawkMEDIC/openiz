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
 * Date: 2016-8-3
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OpenIZ.Core.Model;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Interfaces;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents an act repository service.
	/// </summary>
	/// <seealso cref="IActRepositoryService" />
	/// <seealso cref="Services.IRepositoryService{Act}" />
	/// <seealso cref="Services.IRepositoryService{SubstanceAdministration}" />
	/// <seealso cref="Services.IRepositoryService{QuantityObservation}" />
	/// <seealso cref="Services.IRepositoryService{PatientEncounter}" />
	/// <seealso cref="Services.IRepositoryService{CodedObservation}" />
	/// <seealso cref="Services.IRepositoryService{TextObservation}" />
	public partial class LocalActRepositoryService : IActRepositoryService, 
        IRepositoryService<Act>,
        IPersistableQueryRepositoryService,
        IFastQueryRepositoryService
	{

        // Events for audit
        public event EventHandler<AuditDataEventArgs> DataCreated;
        public event EventHandler<AuditDataEventArgs> DataUpdated;
        public event EventHandler<AuditDataEventArgs> DataObsoleted;
        public event EventHandler<AuditDataDisclosureEventArgs> DataDisclosed;

        /// <summary>
        /// Finds acts based on a specific query.
        /// </summary>
        /// <typeparam name="TAct">The type of the t act.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="totalResults">The total results.</param>
        /// <returns>Returns a list of acts which match the specific query.</returns>
        /// <exception cref="System.InvalidOperationException">Unable to locate persistence service.</exception>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public IEnumerable<TAct> Find<TAct>(Expression<Func<TAct, bool>> query, int offset, int? count, out int totalResults) where TAct : Act
		{
            return this.Find<TAct>(query, offset, count, out totalResults, Guid.Empty);
		}

        /// <summary>
        /// Finds the specified data
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns a list of identified data.</returns>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public IEnumerable<Act> Find(Expression<Func<Act, bool>> query)
		{
            int tr;
            return this.Find<Act>(query, 0, null, out tr);
		}

        /// <summary>
        /// Find specified act
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="totalResults">The total results.</param>
        /// <returns>Returns a list of identified data.</returns>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public IEnumerable<Act> Find(Expression<Func<Act, bool>> query, int offset, int? count, out int totalResults)
		{
			return this.Find<Act>(query, offset, count, out totalResults);
		}

        /// <summary>
        /// Get the specified act.
        /// </summary>
        /// <typeparam name="TAct">The type of the act.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="versionId">The version identifier.</param>
        /// <returns>Returns the act.</returns>
        /// <exception cref="System.InvalidOperationException">If the persistence service is not found</exception>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public TAct Get<TAct>(Guid key, Guid versionId) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();

			var result = persistenceService.Get<Guid>(new Identifier<Guid>(key, versionId), AuthenticationContext.Current.Principal, true);

            this.DataDisclosed?.Invoke(this, new AuditDataDisclosureEventArgs(key.ToString(), new object[] { result }));

            return businessRulesService != null ? businessRulesService.AfterRetrieve(result) : result;
		}

        /// <summary>
        /// Gets the specified model.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the model.</returns>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public Act Get(Guid key)
		{
			return this.Get<Act>(key, Guid.Empty);
		}


        /// <summary>
        /// Gets the specified model.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the model.</returns>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public Act Get(Guid key, Guid versionKey)
        {
            return this.Get<Act>(key, versionKey);
        }

        /// <summary>
        /// Insert the specified act.
        /// </summary>
        /// <typeparam name="TAct">The type of the act.</typeparam>
        /// <param name="act">The act.</param>
        /// <returns>Returns the act.</returns>
        /// <exception cref="System.InvalidOperationException">TAct</exception>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public TAct Insert<TAct>(TAct act) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
			}

			var businessRulesService = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>();

			act = businessRulesService != null ? businessRulesService.BeforeInsert(act) : act;

			act = persistenceService.Insert(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

            this.DataCreated?.Invoke(this, new AuditDataEventArgs(act));

            businessRulesService?.AfterInsert(act);
            return act;
		}

        /// <summary>
        /// Inserts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>TModel.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public Act Insert(Act data)
		{
            return this.Insert<Act>(data);
		}

        /// <summary>
        /// Obsoletes a specific act.
        /// </summary>
        /// <typeparam name="TAct">The type of the act.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Returns the act.</returns>
        /// <exception cref="System.InvalidOperationException">The persistence service is not found.</exception>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.DeleteClinicalData)]
        public TAct Obsolete<TAct>(Guid key) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
			}

			var act = persistenceService.Get<Guid>(new Identifier<Guid>(key), AuthenticationContext.Current.Principal, true);

			if (act == null)
				throw new KeyNotFoundException(key.ToString());

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();

			act = businessRulesService != null ? businessRulesService.BeforeObsolete(act) : act;

			act = persistenceService.Obsolete(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

            this.DataObsoleted?.Invoke(this, new AuditDataEventArgs(act));

            return businessRulesService != null ? businessRulesService.AfterObsolete(act) : act;
		}

        /// <summary>
        /// Insert or update the specified act
        /// </summary>
        public TAct Nullify<TAct>(TAct act) where TAct : Act
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
            }

            var businessRulesService = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>();

            act = businessRulesService != null ? businessRulesService.BeforeUpdate(act) : act;

            act.StatusConceptKey = StatusKeys.Nullified;
            act = persistenceService.Update(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

            this.DataObsoleted?.Invoke(this, new AuditDataEventArgs(act));

            businessRulesService?.AfterUpdate(act);
            return act;

        }

        /// <summary>
        /// Insert or update the specified act
        /// </summary>
        public TAct Cancel<TAct>(TAct act) where TAct : Act
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
            }

            var businessRulesService = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>();

            act = businessRulesService != null ? businessRulesService.BeforeUpdate(act) : act;

            act.StatusConceptKey = StatusKeys.Cancelled;
            act = persistenceService.Update(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

            this.DataObsoleted?.Invoke(this, new AuditDataEventArgs(act));

            businessRulesService?.AfterUpdate(act);
            return act;

        }

        /// <summary>
        /// Obsoletes the specified data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the model.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.DeleteClinicalData)]
        public Act Obsolete(Guid key)
        {
	        return this.Obsolete<Act>(key);
        }

        /// <summary>
        /// Inserts or updates the specific act.
        /// </summary>
        /// <typeparam name="TAct">The type of the act.</typeparam>
        /// <param name="act">The act.</param>
        /// <returns>Returns the act.</returns>
        /// <exception cref="System.InvalidOperationException">TAct</exception>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public TAct Save<TAct>(TAct act) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();

			// validate the act
			this.Validate<TAct>(act);

			try
			{
				act = businessRulesService != null ? businessRulesService.BeforeUpdate(act) : act;

				act = persistenceService.Update(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

                businessRulesService?.AfterUpdate(act);

                this.DataUpdated?.Invoke(this, new AuditDataEventArgs(act));
                return act;
			}
			catch (KeyNotFoundException)
			{
				act = businessRulesService != null ? businessRulesService.BeforeInsert(act) : act;

				act = persistenceService.Insert(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

                this.DataCreated?.Invoke(this, new AuditDataEventArgs(act));
                businessRulesService.AfterInsert(act);

                return act;
			}

			return act;
		}

        /// <summary>
        /// Saves the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Returns the model.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public Act Save(Act data)
        {
	        return this.Save<Act>(data);
        }

		/// <summary>
		/// Validates an act.
		/// </summary>
		/// <typeparam name="TAct">The type of the act.</typeparam>
		/// <param name="data">The data.</param>
		/// <returns>TAct.</returns>
		/// <exception cref="DetectedIssueException">If there are any validation errors detected.</exception>
		/// <exception cref="System.InvalidOperationException">If the data is null.</exception>
		public TAct Validate<TAct>(TAct data) where TAct : Act
		{
			var businessRulesService = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>();

			if (businessRulesService == null)
			{
				return data;
			}

			var details = businessRulesService.Validate(data);

			if (details.Any(d => d.Priority == DetectedIssuePriorityType.Error))
			{
				throw new DetectedIssueException(details);
			}

			// Correct author information and controlling act information
			data = data.Clean() as TAct;

			if (data == null)
			{
				throw new InvalidOperationException($"{nameof(data)} cannot be null");
			}

			var userService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			var currentUserEntity = userService.GetUserEntity(AuthenticationContext.Current.Principal.Identity);

			if (data.Participations.All(o => o.ParticipationRoleKey != ActParticipationKey.Authororiginator))
			{
				data.Participations.Add(new ActParticipation(ActParticipationKey.Authororiginator, currentUserEntity));
			}

			return data;
		}

        /// <summary>
        /// Find the specified object
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public IEnumerable<TAct> Find<TAct>(Expression<Func<TAct, bool>> query, int offset, int? count, out int totalResults, Guid queryId) where TAct : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"Unable to locate persistence service {nameof(IDataPersistenceService<TAct>)}");
            }

            var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();
            IEnumerable<TAct> results = null;
            if (queryId != Guid.Empty && persistenceService is IStoredQueryDataPersistenceService<TAct>)
                results = (persistenceService as IStoredQueryDataPersistenceService<TAct>).Query(query, queryId, offset, count, AuthenticationContext.Current.Principal, out totalResults);
            else
                results = persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);

            // Disclosure event
            this.DataDisclosed?.Invoke(this, new AuditDataDisclosureEventArgs(query.ToString(), results));
            return businessRulesService?.AfterQuery(results) ?? results;
        }

        /// <summary>
        /// Perform a fast search with the specified query parameters
        /// </summary>
        public IEnumerable<TAct> FindFast<TAct>(Expression<Func<TAct, bool>> query, int offset, int? count, out int totalResults, Guid queryId) where TAct : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IFastQueryDataPersistenceService<TAct>>() ;

            if (persistenceService == null)
            {
                return this.Find<TAct>(query, offset, count, out totalResults, queryId);
            }

            var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();
            IEnumerable<TAct> results = null;
            results = persistenceService.QueryFast(query, queryId, offset, count, AuthenticationContext.Current.Principal, out totalResults);

            // Disclosure event
            this.DataDisclosed?.Invoke(this, new AuditDataDisclosureEventArgs(query.ToString(), results));
            return businessRulesService?.AfterQuery(results) ?? results;
        }
    }
}