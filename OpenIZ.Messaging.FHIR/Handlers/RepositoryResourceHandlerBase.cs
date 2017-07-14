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
 * Date: 2017-6-15
 */

using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
		protected IRepositoryService<TModel> m_repository;

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
		/// Create concept set filter based on act type
		/// </summary>
		protected Expression CreateConceptSetFilter(Guid conceptSetKey, ParameterExpression queryParameter)
		{
			var conceptSetRef = Expression.MakeMemberAccess(Expression.MakeMemberAccess(queryParameter, typeof(Act).GetProperty(nameof(Act.TypeConcept))), typeof(Concept).GetProperty(nameof(Concept.ConceptSets)));
			var lParam = Expression.Parameter(typeof(ConceptSet));
			var conceptSetFilter = Expression.MakeBinary(ExpressionType.Equal, Expression.Convert(Expression.MakeMemberAccess(lParam, typeof(ConceptSet).GetProperty(nameof(ConceptSet.Key))), typeof(Guid)), Expression.Constant(conceptSetKey));
			return Expression.Call((MethodInfo)typeof(Enumerable).GetGenericMethod("Any", new Type[] { typeof(ConceptSet) }, new Type[] { typeof(IEnumerable<ConceptSet>), typeof(Func<ConceptSet, bool>) }), conceptSetRef, Expression.Lambda(conceptSetFilter, lParam));
		}

		/// <summary>
		/// Perform a delete operation
		/// </summary>
		protected override TModel Delete(Guid modelId, List<IResultDetail> details)
		{
			return this.m_repository.Obsolete(modelId);
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

		/// <summary>
		/// Perform a read operation
		/// </summary>
		protected override TModel Read(Identifier<Guid> id, List<IResultDetail> details)
		{
			return this.m_repository.Get(id.Id, id.VersionId);
		}

		/// <summary>
		/// Perform an update operation
		/// </summary>
		protected override TModel Update(TModel model, List<IResultDetail> details, TransactionMode mode)
		{
			return this.m_repository.Save(model);
		}
	}
}