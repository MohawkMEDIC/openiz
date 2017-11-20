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
 * Date: 2016-8-14
 */

using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Messaging.FHIR;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Handlers;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using OpenIZ.Messaging.FHIR.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel.Web;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.FHIR.Handlers
{
	/// <summary>
	/// Represents a base FHIR resource handler.
	/// </summary>
	/// <typeparam name="TFhirResource">The type of the t FHIR resource.</typeparam>
	/// <typeparam name="TModel">The type of the t model.</typeparam>
	/// <seealso cref="MARC.HI.EHRS.SVC.Messaging.FHIR.Handlers.IFhirResourceHandler" />
	public abstract class ResourceHandlerBase<TFhirResource, TModel> : IFhirResourceHandler
		where TFhirResource : DomainResourceBase, new()
		where TModel : IdentifiedData, new()

	{
		/// <summary>
		/// The trace source instance.
		/// </summary>
		protected TraceSource traceSource = new TraceSource("OpenIZ.Messaging.FHIR");

		/// <summary>
		/// Gets the name of the resource.
		/// </summary>
		/// <value>The name of the resource.</value>
		public string ResourceName => typeof(TFhirResource).GetCustomAttribute<XmlRootAttribute>().ElementName;

		/// <summary>
		/// Create the specified resource.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>FhirOperationResult.</returns>
		/// <exception cref="System.ArgumentNullException">target</exception>
		/// <exception cref="System.IO.InvalidDataException"></exception>
		/// <exception cref="System.Data.SyntaxErrorException"></exception>
		public virtual FhirOperationResult Create(DomainResourceBase target, TransactionMode mode)
		{
			this.traceSource.TraceInformation("Creating resource {0} ({1})", this.ResourceName, target);

			if (target == null)
				throw new ArgumentNullException(nameof(target));
			else if (!(target is TFhirResource))
				throw new InvalidDataException();

			// We want to map from TFhirResource to TModel
			var modelInstance = this.MapToModel(target as TFhirResource, WebOperationContext.Current);
			if (modelInstance == null)
				throw new SyntaxErrorException(ApplicationContext.Current.GetLocaleString("MSGE001"));

			List<IResultDetail> issues = new List<IResultDetail>();
			var result = this.Create(modelInstance, issues, mode);

			// Return fhir operation result
			return new FhirOperationResult()
			{
				Results = new List<DomainResourceBase>() { this.MapToFhir(result, WebOperationContext.Current) },
				Details = issues,
				Outcome = issues.Exists(o => o.Type == MARC.Everest.Connectors.ResultDetailType.Error) ? ResultCode.Error : ResultCode.Accepted
			};
		}

		/// <summary>
		/// Deletes a specified resource.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>FhirOperationResult.</returns>
		/// <exception cref="System.ArgumentNullException">id</exception>
		/// <exception cref="System.ArgumentException"></exception>
		public FhirOperationResult Delete(string id, TransactionMode mode)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException(nameof(id));

			this.traceSource.TraceInformation("Deleting resource {0}/{1}", this.ResourceName, id);

			// Delete
			var guidId = Guid.Empty;
			if (!Guid.TryParse(id, out guidId))
				throw new ArgumentException(ApplicationContext.Current.GetLocaleString("MSGE002"));

			// Do the deletion
			List<IResultDetail> details = new List<IResultDetail>();

			var result = this.Delete(guidId, details);

			// Return fhir operation result
			return new FhirOperationResult()
			{
				Results = new List<DomainResourceBase>() { this.MapToFhir(result, WebOperationContext.Current) },
				Details = details,
				Outcome = details.Exists(o => o.Type == MARC.Everest.Connectors.ResultDetailType.Error) ? ResultCode.Error : ResultCode.Accepted
			};
		}

		/// <summary>
		/// Queries for a specified resource.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns>Returns the FHIR query result containing the results of the query.</returns>
		/// <exception cref="System.ArgumentNullException">parameters</exception>
		public virtual FhirQueryResult Query(System.Collections.Specialized.NameValueCollection parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException(nameof(parameters));

			Core.Model.Query.NameValueCollection imsiQuery = null;
			FhirQuery query = QueryRewriter.RewriteFhirQuery<TFhirResource, TModel>(parameters, out imsiQuery);

			// Do the query
			int totalResults = 0;
			List<IResultDetail> issues = new List<IResultDetail>();
			var predicate = QueryExpressionParser.BuildLinqExpression<TModel>(imsiQuery);
			var imsiResults = this.Query(predicate, issues, query.QueryId, query.Start, query.Quantity, out totalResults);
			var webOperationContext = WebOperationContext.Current;

			// Return FHIR query result
			return new FhirQueryResult()
			{
				Details = issues,
				Outcome = ResultCode.Accepted,
				Results = imsiResults.AsParallel().Select(o => this.MapToFhir(o, webOperationContext)).OfType<DomainResourceBase>().ToList(),
				Query = query,
				TotalResults = totalResults
			};
		}

		/// <summary>
		/// Retrieves a specific resource.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>Returns the FHIR operation result containing the retrieved resource.</returns>
		/// <exception cref="System.ArgumentNullException">id</exception>
		/// <exception cref="System.ArgumentException">
		/// </exception>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
		public FhirOperationResult Read(string id, string versionId)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException(nameof(id));

			Guid guidId = Guid.Empty, versionGuidId = Guid.Empty;
			if (!Guid.TryParse(id, out guidId))
				throw new ArgumentException(ApplicationContext.Current.GetLocaleString("MSGE002"));
			if (!String.IsNullOrEmpty(versionId) && !Guid.TryParse(versionId, out versionGuidId))
				throw new ArgumentException(ApplicationContext.Current.GetLocaleString("MSGE002"));

			List<IResultDetail> details = new List<IResultDetail>();
			var result = this.Read(new Identifier<Guid>(guidId, versionGuidId), details);
			if (result == null)
				throw new KeyNotFoundException();

			// FHIR Operation result
			return new FhirOperationResult()
			{
				Outcome = ResultCode.Accepted,
				Results = new List<DomainResourceBase>() { this.MapToFhir(result, WebOperationContext.Current) },
				Details = details
			};
		}

		/// <summary>
		/// Updates the specified resource.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="target">The target.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>Returns the FHIR operation result containing the updated resource.</returns>
		/// <exception cref="System.ArgumentNullException">target</exception>
		/// <exception cref="System.IO.InvalidDataException"></exception>
		/// <exception cref="System.Data.SyntaxErrorException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.Reflection.AmbiguousMatchException"></exception>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
		public FhirOperationResult Update(string id, DomainResourceBase target, TransactionMode mode)
		{
			this.traceSource.TraceInformation("Updating resource {0}/{1} ({2})", this.ResourceName, id, target);

			if (target == null)
				throw new ArgumentNullException(nameof(target));
			else if (!(target is TFhirResource))
				throw new InvalidDataException();

			// We want to map from TFhirResource to TModel
			var modelInstance = this.MapToModel(target as TFhirResource, WebOperationContext.Current);
			if (modelInstance == null)
				throw new SyntaxErrorException(ApplicationContext.Current.GetLocaleString("MSGE001"));

			// Guid identifier
			var guidId = Guid.Empty;
			if (!Guid.TryParse(id, out guidId))
				throw new ArgumentException(ApplicationContext.Current.GetLocaleString("MSGE002"));

			// Model instance key does not equal path
			if (modelInstance.Key != Guid.Empty && modelInstance.Key != guidId)
				throw new AmbiguousMatchException(ApplicationContext.Current.GetLocaleString("MSGE003"));
			else if (modelInstance.Key == Guid.Empty)
				modelInstance.Key = guidId;
			else
				throw new KeyNotFoundException();

			List<IResultDetail> issues = new List<IResultDetail>();
			var result = this.Update(modelInstance, issues, mode);

			// Return fhir operation result
			return new FhirOperationResult
			{
				Results = new List<DomainResourceBase> { this.MapToFhir(result, WebOperationContext.Current) },
				Details = issues,
				Outcome = issues.Exists(o => o.Type == MARC.Everest.Connectors.ResultDetailType.Error) ? ResultCode.Error : ResultCode.Accepted
			};
		}

		/// <summary>
		/// Creates the specified model instance.
		/// </summary>
		/// <param name="modelInstance">The model instance.</param>
		/// <param name="issues">The issues.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>Returns the created model.</returns>
		protected abstract TModel Create(TModel modelInstance, List<IResultDetail> issues, TransactionMode mode);

		/// <summary>
		/// Deletes the specified model identifier.
		/// </summary>
		/// <param name="modelId">The model identifier.</param>
		/// <param name="details">The details.</param>
		/// <returns>Returns the deleted model.</returns>
		protected abstract TModel Delete(Guid modelId, List<IResultDetail> details);

		/// <summary>
		/// Maps a model instance to a FHIR instance.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Returns the mapped FHIR resource.</returns>
		protected abstract TFhirResource MapToFhir(TModel model, WebOperationContext webOperationContext);

		/// <summary>
		/// Maps a FHIR resource to a model instance.
		/// </summary>
		/// <param name="resource">The resource.</param>
		/// <returns>Returns the mapped model.</returns>
		protected abstract TModel MapToModel(TFhirResource resource, WebOperationContext webOperationContext);

		/// <summary>
		/// Queries the specified query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="issues">The issues.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <returns>Returns the list of models which match the given parameters.</returns>
		protected abstract IEnumerable<TModel> Query(Expression<Func<TModel, bool>> query, List<IResultDetail> issues, Guid queryId, int offset, int count, out int totalResults);

		/// <summary>
		/// Reads the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="details">The details.</param>
		/// <returns>Returns the model which matches the given id.</returns>
		protected abstract TModel Read(Identifier<Guid> id, List<IResultDetail> details);

		/// <summary>
		/// Updates the specified model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="details">The details.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>Returns the updated model.</returns>
		protected abstract TModel Update(TModel model, List<IResultDetail> details, TransactionMode mode);
	}
}