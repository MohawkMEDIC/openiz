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
using System;
using OpenIZ.Core;
using System.Reflection;
using System.Collections.Specialized;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Messaging.FHIR;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Handlers;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using System.Xml.Serialization;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Map;
using System.IO;
using MARC.HI.EHRS.SVC.Core;
using MARC.Everest.Connectors;
using System.Collections.Generic;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Data;
using System.Diagnostics;
using System.Data;
using OpenIZ.Messaging.FHIR.Util;
using System.Linq;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Query;

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// FHIR Resource handler base
    /// </summary>
    /// <typeparam name="TFhirResource"></typeparam>
    public abstract class ResourceHandlerBase<TFhirResource, TModel> : IFhirResourceHandler
        where TFhirResource : DomainResourceBase, new()
        where TModel : IdentifiedData, new()
    {

        // Model mapper
        protected static ModelMapper s_mapper = new ModelMapper(typeof(ResourceHandlerBase<,>).Assembly.GetManifestResourceStream("OpenIZ.Messaging.FHIR.ModelMap.xml"));

        // Trace source
        protected TraceSource m_traceSource = new TraceSource("OpenIZ.Messaging.FHIR");

        /// <summary>
        /// Get the resource name
        /// </summary>
        public string ResourceName
        {
            get
            {
                var rootAttribute = typeof(TFhirResource).GetCustomAttribute<XmlRootAttribute>();
                return rootAttribute.ElementName;
            }
        }

        /// <summary>
        /// Create the specified resource
        /// </summary>
        public virtual FhirOperationResult Create(DomainResourceBase target, TransactionMode mode)
        {
            this.m_traceSource.TraceInformation("Creating resource {0} ({1})", this.ResourceName, target);

            if (target == null)
                throw new ArgumentNullException(nameof(target));
            else if (!(target is TFhirResource))
                throw new InvalidDataException();

            // We want to map from TFhirResource to TModel
            var modelInstance = this.MapToModel(target as TFhirResource);
            if (modelInstance == null)
                throw new SyntaxErrorException(ApplicationContext.Current.GetLocaleString("MSGE001"));

            List<IResultDetail> issues = new List<IResultDetail>();
            var result = this.Create(modelInstance, issues, mode);

            // Return fhir operation result
            return new FhirOperationResult()
            {
                Results = new List<DomainResourceBase>() { this.MapToFhir(result) },
                Details = issues,
                Outcome = issues.Exists(o=>o.Type == MARC.Everest.Connectors.ResultDetailType.Error) ? ResultCode.Error : ResultCode.Accepted
            };

        }

        /// <summary>
        /// Map to FHIR
        /// </summary>
        protected abstract TFhirResource MapToFhir(TModel model);

        /// <summary>
        /// Map to model
        /// </summary>
        protected abstract TModel MapToModel(TFhirResource resource);

        /// <summary>
        /// Perform the actual persistence of the insert
        /// </summary>
        protected abstract TModel Create(TModel modelInstance, List<IResultDetail> issues, TransactionMode mode);

        /// <summary>
        /// Query the specified result out of the DB
        /// </summary>
        protected abstract IEnumerable<TModel> Query(Expression<Func<TModel, bool>> query, List<IResultDetail> issues, int offset, int count, out int totalResults);

        /// <summary>
        /// Delete the specified patient identifier
        /// </summary>
        protected abstract TModel Delete(Guid modelId, List<IResultDetail> details);

        /// <summary>
        /// Retrieve the specified patient
        /// </summary>
        protected abstract TModel Read(Identifier<Guid> id, List<IResultDetail> details);
        
        /// <summary>
        /// Update the specified model
        /// </summary>
        protected abstract TModel Update(TModel model, List<IResultDetail> details, TransactionMode mode);

        /// <summary>
        /// Deletes the specified resource
        /// </summary>
        public FhirOperationResult Delete(string id, TransactionMode mode)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            this.m_traceSource.TraceInformation("Deleting resource {0}/{1}", this.ResourceName, id);

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
                Results = new List<DomainResourceBase>() { this.MapToFhir(result) },
                Details = details,
                Outcome = details.Exists(o => o.Type == MARC.Everest.Connectors.ResultDetailType.Error) ? ResultCode.Error : ResultCode.Accepted
            };
        }

        /// <summary>
        /// Query for the specified data
        /// </summary>
        /// <returns></returns>
        public FhirQueryResult Query(System.Collections.Specialized.NameValueCollection parameters)
        {
            
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Core.Model.Query.NameValueCollection imsiQuery = null;
            FhirQuery query = QueryRewriter.RewriteFhirQuery<TFhirResource, TModel>(parameters, out imsiQuery);

            // Do the query
            int totalResults = 0;
            List<IResultDetail> issues = new List<IResultDetail>();
            var predicate = QueryExpressionParser.BuildLinqExpression<TModel>(imsiQuery);
            var imsiResults = this.Query(predicate, issues, query.Start, query.Quantity, out totalResults);


            // Return FHIR query result
            return new FhirQueryResult()
            {
                Details = issues,
                Outcome = ResultCode.Accepted,
                Results = imsiResults.Select(o=>this.MapToFhir(o)).OfType<DomainResourceBase>().ToList(),
                Query = query,
                TotalResults = totalResults
            };
        }

        /// <summary>
        /// Perform the retrieve operation
        /// </summary>
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
                Results = new List<DomainResourceBase>() { this.MapToFhir(result) },
                Details = details
            };
        }

        /// <summary>
        /// Updates the specified record
        /// </summary>
        public FhirOperationResult Update(string id, DomainResourceBase target, TransactionMode mode)
        {
            this.m_traceSource.TraceInformation("Updating resource {0}/{1} ({2})", this.ResourceName, id, target);

            if (target == null)
                throw new ArgumentNullException(nameof(target));
            else if (!(target is TFhirResource))
                throw new InvalidDataException();

            // We want to map from TFhirResource to TModel
            var modelInstance = this.MapToModel(target as TFhirResource);
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
            return new FhirOperationResult()
            {
                Results = new List<DomainResourceBase>() { this.MapToFhir(result) },
                Details = issues,
                Outcome = issues.Exists(o => o.Type == MARC.Everest.Connectors.ResultDetailType.Error) ? ResultCode.Error : ResultCode.Accepted
            };

        }
    }
}