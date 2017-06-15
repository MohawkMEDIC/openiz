using System;
using System.Collections.Generic;
using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Messaging.FHIR;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model;
using OpenIZ.Core.Services;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// Resource handler for acts base
    /// </summary>
    public abstract class RepositoryResourceHandlerBase<TFhirResource, TModel> : ResourceHandlerBase<TFhirResource, TModel>
        where TFhirResource : DomainResourceBase, new()
        where TModel : IdentifiedData, new()
    {

        // Repository service model
        private IRepositoryService<TModel> m_repository;

        /// <summary>
        /// CTOR
        /// </summary>
        public RepositoryResourceHandlerBase()
        {
            ApplicationContext.Current.Started += (o, e) => this.m_repository = ApplicationContext.Current.GetService<IRepositoryService<TModel>>();
        }

        /// <summary>
        /// Create the object
        /// </summary>
        protected override TModel Create(TModel modelInstance, List<IResultDetail> issues, TransactionMode mode)
        {
            return this.m_repository.Insert(modelInstance);
        }

        /// <summary>
        /// Perform an update operation
        /// </summary>
        protected override TModel Update(TModel model, List<IResultDetail> details, TransactionMode mode)
        {
            return this.m_repository.Save(model);
        }

        /// <summary>
        /// Perform a delete operation
        /// </summary>
        protected override TModel Delete(Guid modelId, List<IResultDetail> details)
        {
            return this.m_repository.Obsolete(modelId);
        }

        /// <summary>
        /// Perform a read operation
        /// </summary>
        protected override TModel Read(Identifier<Guid> id, List<IResultDetail> details)
        {
            return this.m_repository.Get(id.Id, id.VersionId);
        }

        /// <summary>
        /// Query for patients.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="issues">The issues.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="totalResults">The total results.</param>
        /// <returns>Returns the list of models which match the given parameters.</returns>
        protected override IEnumerable<TModel> Query(Expression<Func<TModel, bool>> query, List<IResultDetail> issues, Guid queryId, int offset, int count, out int totalResults)
        {
            if (typeof(TModel).GetProperty(nameof(Entity.StatusConceptKey)) != null)
            {
                var obsoletionReference = Expression.MakeBinary(ExpressionType.NotEqual, Expression.Convert(Expression.MakeMemberAccess(query.Parameters[0], typeof(TModel).GetProperty(nameof(Entity.StatusConceptKey))), typeof(Guid)), Expression.Constant(StatusKeys.Obsolete));
                query = Expression.Lambda<Func<TModel, bool>>(Expression.AndAlso(obsoletionReference, query.Body), query.Parameters);
            }

            if (queryId == Guid.Empty)
                return this.m_repository.Find(query, offset, count, out totalResults);
            else
                return (this.m_repository as IPersistableQueryRepositoryService).Find<TModel>(query, offset, count, out totalResults, queryId);

        }
    }
}