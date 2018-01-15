using MARC.HI.EHRS.SVC.Auditing.Data;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Attributes;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Security.Audit;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Security.Audit.Model;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Auditing.ADO.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Event;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core.Data;

namespace OpenIZ.Persistence.Auditing.ADO.Services
{
    /// <summary>
    /// Represents a service which is responsible for the storage of audits
    /// </summary>
    [Description("ADO Audit Repository")]
    [TraceSource(AuditConstants.TraceSourceName)]
    public class AdoAuditRepositoryService : IDataPersistenceService<AuditData>
    {

        // Confiugration
        private AdoConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(AuditConstants.ConfigurationSectionName) as AdoConfiguration;

        // Model map
        private ModelMapper m_mapper = new ModelMapper(typeof(AdoAuditRepositoryService).Assembly.GetManifestResourceStream("OpenIZ.Persistence.Auditing.ADO.Data.Map.ModelMap.xml"));

        // Query builder
        private QueryBuilder m_builder;

        // Trace source name
        private TraceSource m_traceSource = new TraceSource(AuditConstants.TraceSourceName);

        /// <summary>
        /// Fired when data is being inserted
        /// </summary>
        public event EventHandler<PrePersistenceEventArgs<AuditData>> Inserting;
        /// <summary>
        /// Fired when data is has been inserted
        /// </summary>
        public event EventHandler<PostPersistenceEventArgs<AuditData>> Inserted;
        /// <summary>
        /// Fired when data is being updated
        /// </summary>
        public event EventHandler<PrePersistenceEventArgs<AuditData>> Updating;
        /// <summary>
        /// Fired when data is has been inserted
        /// </summary>
        public event EventHandler<PostPersistenceEventArgs<AuditData>> Updated;
        /// <summary>
        /// Fired when data is being obsoleted
        /// </summary>
        public event EventHandler<PrePersistenceEventArgs<AuditData>> Obsoleting;
        /// <summary>
        /// Fired when data is has been inserted
        /// </summary>
        public event EventHandler<PostPersistenceEventArgs<AuditData>> Obsoleted;
        /// <summary>
        /// Fired when data is being retrieved
        /// </summary>
        public event EventHandler<PreRetrievalEventArgs> Retrieving;
        /// <summary>
        /// Fired when data is has been retrieved
        /// </summary>
        public event EventHandler<PostRetrievalEventArgs<AuditData>> Retrieved;
        /// <summary>
        /// Fired when data is being queryed
        /// </summary>
        public event EventHandler<PreQueryEventArgs<AuditData>> Querying;
        /// <summary>
        /// Fired when data is has been queried
        /// </summary>
        public event EventHandler<PostQueryEventArgs<AuditData>> Queried;

        /// <summary>
        /// Create new audit repository service
        /// </summary>
        public AdoAuditRepositoryService()
        {
            this.m_builder = new QueryBuilder(this.m_mapper, this.m_configuration.Provider);
        }

        /// <summary>
        /// Convert a db audit to model 
        /// </summary>
        private AuditData ToModelInstance(DataContext context, CompositeResult<DbAuditData, DbAuditCode> res, bool summary = true)
        {
            var retVal = new AuditData()
            {
                ActionCode = (ActionType)res.Object1.ActionCode,
                EventIdentifier = (EventIdentifierType)res.Object1.EventIdentifier,
                Outcome = (OutcomeIndicator)res.Object1.Outcome,
                Timestamp = res.Object1.Timestamp,
                CorrelationToken = res.Object1.Key
            };

            if (res.Object1.EventTypeCode != null)
            {
                var concept = ApplicationContext.Current.GetService<IConceptRepositoryService>().GetConcept(res.Object2.Code);
                retVal.EventTypeCode = new AuditCode(res.Object2.Code, res.Object2.CodeSystem)
                {
                    DisplayName = concept?.ConceptNames.First()?.Name ?? res.Object2.Code
                };
            }

            // Get actors and objects
            if (!summary)
            {

                // Actors
                var sql = context.CreateSqlStatement<DbAuditActorAssociation>().SelectFrom()
                        .InnerJoin<DbAuditActorAssociation, DbAuditActor>(o => o.TargetKey, o => o.Key)
                        .Join<DbAuditActor, DbAuditCode>("LEFT", o => o.ActorRoleCode, o => o.Key)
                        .Where<DbAuditActorAssociation>(o => o.SourceKey == res.Object1.Key)
                        .Build();

                foreach (var itm in context.Query<CompositeResult<DbAuditActor, DbAuditCode>>(sql))
                    retVal.Actors.Add(new AuditActorData()
                    {
                        UserName = itm.Object1.UserName,
                        UserIsRequestor = itm.Object1.UserIsRequestor,
                        UserIdentifier = itm.Object1.UserIdentifier,
                        ActorRoleCode = new List<AuditCode>()
                        {
                            new AuditCode(itm.Object2.Code, itm.Object2.CodeSystem)
                        }
                    });

                // Objects
                foreach (var itm in context.Query<DbAuditObject>(o => o.AuditId == res.Object1.Key))
                {
                    retVal.AuditableObjects.Add(new AuditableObject()
                    {
                        IDTypeCode = (AuditableObjectIdType?)itm.IDTypeCode,
                        LifecycleType = (AuditableObjectLifecycle?)itm.LifecycleType,
                        NameData = itm.NameData,
                        ObjectId = itm.ObjectId,
                        QueryData = itm.QueryData,
                        Role = (AuditableObjectRole?)itm.Role,
                        Type = (AuditableObjectType)itm.Type
                    });
                }
            }
            else
            {
                // Actors
                // Actors
                var sql = context.CreateSqlStatement<DbAuditActorAssociation>().SelectFrom()
                        .InnerJoin<DbAuditActorAssociation, DbAuditActor>(o => o.TargetKey, o => o.Key)
                        .Join<DbAuditActor, DbAuditCode>("LEFT", o => o.ActorRoleCode, o => o.Key)
                        .Where<DbAuditActorAssociation>(o => o.SourceKey == res.Object1.Key).And<DbAuditActor>(p => p.UserIsRequestor == true)
                        .Build();

                foreach (var itm in context.Query<CompositeResult<DbAuditActor, DbAuditCode>>(sql))
                    retVal.Actors.Add(new AuditActorData()
                    {
                        UserName = itm.Object1.UserName,
                        UserIsRequestor = itm.Object1.UserIsRequestor,
                        UserIdentifier = itm.Object1.UserIdentifier,
                        ActorRoleCode = new List<AuditCode>()
                        {
                            new AuditCode(itm.Object2.Code, itm.Object2.CodeSystem)
                        }
                    });
            }

            return retVal;
        }

        /// <summary>
        /// Insert the specified audit into the database
        /// </summary>
        public AuditData Insert(AuditData storageData, IPrincipal principal, TransactionMode mode)
        {

            // Pre-event trigger
            var preEvtData = new PrePersistenceEventArgs<AuditData>(storageData, principal);
            this.Inserting?.Invoke(this, preEvtData);
            if (preEvtData.Cancel)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Warning, 0, "Pre-Event handler indicates abort insert {0}", storageData);
                return storageData;
            }

            // Insert
            using (var context = this.m_configuration.Provider.GetWriteConnection())
            {
                IDbTransaction tx = null;
                try
                {
                    
                    tx = context.BeginTransaction();

                    // Insert core
                    var dbAudit = this.m_mapper.MapModelInstance<AuditData, DbAuditData>(storageData);

                    var eventId = storageData.EventTypeCode;
                    if (eventId != null)
                    {
                        var existing = context.FirstOrDefault<DbAuditCode>(o => o.Code == eventId.Code && o.CodeSystem == eventId.CodeSystem);
                        if (existing == null)
                        {
                            Guid codeId = Guid.NewGuid();
                            dbAudit.EventTypeCode = codeId;
                            context.Insert(new DbAuditCode() { Code = eventId.Code, CodeSystem = eventId.CodeSystem, Key = codeId });
                        }
                        else
                            dbAudit.EventTypeCode = existing.Key;
                    }

                    dbAudit.CreationTime = DateTime.Now;
                    storageData.CorrelationToken = Guid.NewGuid();
                    dbAudit.Key = storageData.CorrelationToken;
                    context.Insert(dbAudit);

                    // Insert secondary properties
                    if (storageData.Actors != null)
                        foreach (var act in storageData.Actors)
                        {
                            var dbAct = context.FirstOrDefault<DbAuditActor>(o => o.UserName == act.UserName);
                            if (dbAct == null)
                            {
                                dbAct = this.m_mapper.MapModelInstance<AuditActorData, DbAuditActor>(act);
                                dbAct.Key = Guid.NewGuid();
                                context.Insert(dbAct);
                                var roleCode = act.ActorRoleCode?.FirstOrDefault();
                                if (roleCode != null)
                                {
                                    var existing = context.FirstOrDefault<DbAuditCode>(o => o.Code == roleCode.Code && o.CodeSystem == roleCode.CodeSystem);
                                    if (existing == null)
                                    {
                                        dbAct.ActorRoleCode = Guid.NewGuid();
                                        context.Insert(new DbAuditCode() { Code = roleCode.Code, CodeSystem = roleCode.CodeSystem, Key = dbAct.ActorRoleCode });
                                    }
                                    else
                                        dbAct.ActorRoleCode = existing.Key;
                                }

                            }
                            context.Insert(new DbAuditActorAssociation()
                            {
                                TargetKey = dbAct.Key,
                                SourceKey = dbAudit.Key,
                                Key = Guid.NewGuid()
                            });
                        }

                    // Audit objects
                    if (storageData.AuditableObjects != null)
                        foreach (var ao in storageData.AuditableObjects)
                        {
                            var dbAo = this.m_mapper.MapModelInstance<AuditableObject, DbAuditObject>(ao);
                            dbAo.IDTypeCode = (int)(ao.IDTypeCode ?? 0);
                            dbAo.LifecycleType = (int)(ao.LifecycleType ?? 0);
                            dbAo.Role = (int)(ao.Role ?? 0);
                            dbAo.Type = (int)(ao.Type);
                            dbAo.AuditId = dbAudit.Key;
                            dbAo.Key = Guid.NewGuid();
                            context.Insert(dbAo);
                        }

                    if (mode == TransactionMode.Commit)
                        tx.Commit();
                    else
                        tx.Rollback();

                    var args = new PostPersistenceEventArgs<AuditData>(storageData, principal)
                    {
                        Mode = mode
                    };

                    this.Inserted?.Invoke(this, args);

                    return storageData;
                }
                catch (Exception ex)
                {
                    tx?.Rollback();
                    this.m_traceSource.TraceError("Error inserting audit: {0}", ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Update the audit - Not supported
        /// </summary>
        public AuditData Update(AuditData storageData, IPrincipal principal, TransactionMode mode)
        {
            throw new NotSupportedException("Updates not permitted");
        }

        /// <summary>
        /// Obsolete the audit - Not supported
        /// </summary>
        public AuditData Obsolete(AuditData storageData, IPrincipal principal, TransactionMode mode)
        {
            throw new NotSupportedException("Obsoletion of audits not permitted");
        }

        /// <summary>
        /// Gets the specified object by identifier
        /// </summary>
        public AuditData Get<TIdentifier>(MARC.HI.EHRS.SVC.Core.Data.Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
        {

            var preEvtData = new PreRetrievalEventArgs(containerId, principal);
            this.Retrieving?.Invoke(this, preEvtData);
            if(preEvtData.Cancel)
            {
                this.m_traceSource.TraceWarning("Pre-retrieval event indicates cancel {0}", containerId);
                return null;
            }

            try
            {
                var pk = containerId as Identifier<Guid>;

                // Fetch 
                using (var context = this.m_configuration.Provider.GetReadonlyConnection())
                {
                    var sql = this.m_builder.CreateQuery<AuditData>(o => o.CorrelationToken == pk.Id).Limit(1).Build();
                    var res = context.FirstOrDefault(typeof(CompositeResult<DbAuditData, DbAuditCode>), sql);
                    var result = this.ToModelInstance(context, res as CompositeResult<DbAuditData, DbAuditCode>, false);

                    var postEvtData = new PostRetrievalEventArgs<AuditData>(result, principal);
                    this.Retrieved?.Invoke(this, postEvtData);

                    return postEvtData.Data;

                }
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceError("Error retrieving audit {0} : {1}", containerId.Id, e);
                throw;
            }
        }

        /// <summary>
        /// Return a count of audits matching the query
        /// </summary>
        public int Count(Expression<Func<AuditData, bool>> query, IPrincipal authContext)
        {
            var tr = 0;
            this.Query(query, 0, null, authContext, out tr);
            return tr;
        }

        /// <summary>
        /// Execute a query
        /// </summary>
        public IEnumerable<AuditData> Query(Expression<Func<AuditData, bool>> query, IPrincipal authContext)
        {
            int tr = 0;
            return this.Query(query, 0, null, authContext, out tr);
        }

        /// <summary>
        /// Executes a query for the specified objects
        /// </summary>
        public IEnumerable<AuditData> Query(Expression<Func<AuditData, bool>> query, int offset, int? count, IPrincipal authContext, out int totalCount)
        {

            var preEvtData = new PreQueryEventArgs<AuditData>(query, authContext);
            this.Querying?.Invoke(this, preEvtData);
            if(preEvtData.Cancel)
            {
                this.m_traceSource.TraceWarning("Pre-event handler for query indicates cancel : {0}", query);
                totalCount = 0;
                return null;
            }

            try
            {
                using (var context = this.m_configuration.Provider.GetReadonlyConnection())
                {
                    var sql = this.m_builder.CreateQuery(query).Build();
                    sql = sql.OrderBy<DbAuditData>(o => o.Timestamp, SortOrderType.OrderByDescending);

                    // Total results
                    totalCount = context.Count(sql);

                    // Query control
                    if (count.HasValue)
                        sql.Limit(count.Value);
                    if (offset > 0)
                    {
                        if (count == 0)
                            sql.Limit(100).Offset(offset);
                        else
                            sql.Offset(offset);
                    }
                    sql = sql.Build();
                    var itm = context.Query<CompositeResult<DbAuditData, DbAuditCode>>(sql);
                    AuditUtil.AuditAuditLogUsed(ActionType.Read, OutcomeIndicator.Success, sql.ToString(), itm.Select(o => o.Object1.Key).ToArray());
                    var results = itm.Select(o => this.ToModelInstance(context, o)).ToList().AsQueryable();

                    // Event args
                    var postEvtArgs = new PostQueryEventArgs<AuditData>(query, results, authContext);
                    this.Queried?.Invoke(this, postEvtArgs);
                    return postEvtArgs.Results;

                }
            }
            catch (Exception e)
            {
                AuditUtil.AuditAuditLogUsed(ActionType.Read, OutcomeIndicator.EpicFail, query.ToString());
                this.m_traceSource.TraceError("Could not query audit {0}: {1}", query, e);
                throw;
            }
        }
    }
}
