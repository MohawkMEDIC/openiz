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
 * Date: 2017-4-5
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Entities;
using MARC.HI.EHRS.SVC.Core.Data;
using System.Linq;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Interfaces;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Represents a base class for entity repository services
    /// </summary>
    public abstract class LocalEntityRepositoryServiceBase : IPersistableQueryRepositoryService,
        IAuditEventSource,
        IFastQueryRepositoryService
    {
        public event EventHandler<AuditDataEventArgs> DataCreated;
        public event EventHandler<AuditDataEventArgs> DataUpdated;
        public event EventHandler<AuditDataEventArgs> DataObsoleted;
        public event EventHandler<AuditDataDisclosureEventArgs> DataDisclosed;

        /// <summary>
        /// Find with stored query parameters
        /// </summary>
        public IEnumerable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> query, int offset, int? count, out int totalResults, Guid queryId) where TEntity : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TEntity>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"Unable to locate {typeof(IDataPersistenceService<TEntity>).FullName}");
            }

            var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TEntity>();

            IEnumerable<TEntity> results = null;
            if (queryId != Guid.Empty && persistenceService is IStoredQueryDataPersistenceService<TEntity>)
                results = (persistenceService as IStoredQueryDataPersistenceService<TEntity>).Query(query, queryId, offset, count, AuthenticationContext.Current.Principal, out totalResults);
            else
                results = persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);

            return businessRulesService != null ? businessRulesService.AfterQuery(results) : results;
        }

        /// <summary>
        /// Performs insert of object
        /// </summary>
        protected TEntity Insert<TEntity>(TEntity entity) where TEntity : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TEntity>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<TEntity>)}");
            }

            this.Validate(entity);

            var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TEntity>();

            entity = businessRulesService?.BeforeInsert(entity) ?? entity;

            entity = persistenceService.Insert(entity, AuthenticationContext.Current.Principal, TransactionMode.Commit);

            this.DataCreated?.Invoke(this, new AuditDataEventArgs(entity));
            businessRulesService?.AfterInsert(entity);

            return entity;
        }

        /// <summary>
        /// Obsolete the specified data
        /// </summary>
        protected TEntity Obsolete<TEntity>(Guid key) where TEntity : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TEntity>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<TEntity>)}");
            }

            var entity = persistenceService.Get(new Identifier<Guid>(key), AuthenticationContext.Current.Principal, true);

            if (entity == null)
            {
                throw new InvalidOperationException("Entity Relationship not found");
            }

            var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TEntity>();

            entity = businessRulesService?.BeforeObsolete(entity) ?? entity;

            entity = persistenceService.Obsolete(entity, AuthenticationContext.Current.Principal, TransactionMode.Commit);

            return businessRulesService?.AfterObsolete(entity) ?? entity;
        }

        /// <summary>
        /// Get specified data from persistence
        /// </summary>
        protected TEntity Get<TEntity>(Guid key, Guid versionKey) where TEntity : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TEntity>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<TEntity>)}");
            }

            var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TEntity>();

            var result = persistenceService.Get(new Identifier<Guid>(key, versionKey), AuthenticationContext.Current.Principal, true);

            return businessRulesService?.AfterRetrieve(result) ?? result;

        }

        /// <summary>
        /// Save the specified entity (insert or update)
        /// </summary>
        protected TEntity Save<TEntity>(TEntity data) where TEntity : IdentifiedData
        {


            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TEntity>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<TEntity>)}");
            }

            this.Validate(data);

            var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TEntity>();

            try
            {
                TEntity old = null;

                if (data.Key.HasValue)
                {
                    old = persistenceService.Get(new Identifier<Guid>(data.Key.Value), AuthenticationContext.Current.Principal, true);
                }

                // HACK: Lookup by ER src<>trg
                if (old == null && typeof(TEntity) == typeof(EntityRelationship))
                {
                    var tr = 0;
                    var erd = data as EntityRelationship;
                    old = (TEntity)(persistenceService as IDataPersistenceService<EntityRelationship>).Query(o => o.SourceEntityKey == erd.SourceEntityKey && o.TargetEntityKey == erd.TargetEntityKey, 0, 1, AuthenticationContext.Current.Principal, out tr).OfType<Object>().FirstOrDefault();
                }

                data = businessRulesService?.BeforeUpdate(data) ?? data;
                data = persistenceService.Update(data, AuthenticationContext.Current.Principal, TransactionMode.Commit);
                this.DataUpdated?.Invoke(this, new AuditDataEventArgs(data));
                businessRulesService?.AfterUpdate(data);
                return data;
            }
            catch (KeyNotFoundException)
            {
                data = businessRulesService?.BeforeInsert(data) ?? data;
                data = persistenceService.Insert(data, AuthenticationContext.Current.Principal, TransactionMode.Commit);
                this.DataCreated?.Invoke(this, new AuditDataEventArgs(data));
                businessRulesService?.AfterInsert(data);
                return data;
            }

            return data;
        }

        /// <summary>
        /// Validate a patient before saving
        /// </summary>
        internal TEntity Validate<TEntity>(TEntity p) where TEntity : IdentifiedData
        {
            p = (TEntity)p.Clean(); // clean up messy data

            var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TEntity>();

            var details = businessRulesService?.Validate(p) ?? new List<DetectedIssue>();

            if (details.Any(d => d.Priority == DetectedIssuePriorityType.Error))
            {
                throw new DetectedIssueException(details);
            }
            return p;
        }

        /// <summary>
        /// Perform a faster version of the query for an object
        /// </summary>
        public IEnumerable<TEntity> FindFast<TEntity>(Expression<Func<TEntity, bool>> query, int offset, int? count, out int totalResults, Guid queryId) where TEntity : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IFastQueryDataPersistenceService<TEntity>>();

            if (persistenceService == null)
            {
                return this.Find<TEntity>(query, offset, count, out totalResults, queryId);
            }

            var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TEntity>();

            IEnumerable<TEntity> results = null;
            results = persistenceService.QueryFast(query, queryId, offset, count, AuthenticationContext.Current.Principal, out totalResults);

            return businessRulesService != null ? businessRulesService.AfterQuery(results) : results;
        }
    }
}