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

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// FHIR Resource handler base
    /// </summary>
    /// <typeparam name="TFhirResource"></typeparam>
    public class ResourceHandlerBase<TFhirResource, TModel> : IFhirResourceHandler
        where TFhirResource : ResourceBase, new()
        where TModel : IdentifiedData, new()
    {

        // Model mapper
        protected static ModelMapper s_mapper = new ModelMapper(typeof(ResourceHandlerBase<,>).Assembly.GetManifestResourceStream("OpenIZ.Messaging.FHIR.ModelMap.xml"));

        // Persistence service
        protected IDataPersistenceService<TModel> m_persistence = null;

        // Trace source
        protected TraceSource m_traceSource = new TraceSource("OpenIZ.Messaging.FHIR");

        /// <summary>
        /// Constructor
        /// </summary>
        public ResourceHandlerBase()
        {
            this.m_persistence = ApplicationContext.Current.GetService<IDataPersistenceService<TModel>>();
            
            if (this.m_persistence == null)
                throw new InvalidOperationException(String.Format("Cannot find the appropriate persistence handler for {0}", typeof(TModel)));

        }

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
        public virtual FhirOperationResult Create(ResourceBase target, TransactionMode mode)
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
                Results = new List<ResourceBase>() { this.MapToFhir(result) },
                Details = issues,
                Outcome = issues.Exists(o=>o.Type == MARC.Everest.Connectors.ResultDetailType.Error) ? ResultCode.Error : ResultCode.Accepted
            };

        }

        /// <summary>
        /// Map to FHIR
        /// </summary>
        protected virtual TFhirResource MapToFhir(TModel model)
        {
            return s_mapper.MapModelInstance<TModel, TFhirResource>(model);
        }

        /// <summary>
        /// Map to model
        /// </summary>
        protected virtual TModel MapToModel(TFhirResource resource)
        {
            return s_mapper.MapDomainInstance<TFhirResource, TModel>(resource);
        }

        /// <summary>
        /// Perform the actual persistence of the insert
        /// </summary>
        protected virtual TModel Create(TModel modelInstance, List<IResultDetail> issues, TransactionMode mode)
        {
            return this.m_persistence.Insert(modelInstance, AuthenticationContext.Current.Principal, mode);
        }

        /// <summary>
        /// Delete the specified patient identifier
        /// </summary>
        protected virtual TModel Delete(TModel model, List<IResultDetail> details, TransactionMode mode)
        {
            return this.m_persistence.Obsolete(model, AuthenticationContext.Current.Principal, mode);
        }

        /// <summary>
        /// Retrieve the specified patient
        /// </summary>
        protected virtual TModel Read(Identifier<Guid> id, List<IResultDetail> details)
        {
            return this.m_persistence.Get(id, AuthenticationContext.Current.Principal, false);
        }

        /// <summary>
        /// Update the specified model
        /// </summary>
        protected virtual TModel Update(TModel model, List<IResultDetail> details, TransactionMode mode)
        {
            return this.m_persistence.Update(model, AuthenticationContext.Current.Principal, mode);
        }

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

            var modelObj = this.m_persistence.Get(new Identifier<Guid>(guidId), AuthenticationContext.Current.Principal, true);
            if (modelObj == null)
                throw new KeyNotFoundException();

            // Do the deletion
            List<IResultDetail> details = new List<IResultDetail>();
            var result = this.Delete(modelObj, details, mode);

            // Return fhir operation result
            return new FhirOperationResult()
            {
                Results = new List<ResourceBase>() { this.MapToFhir(result) },
                Details = details,
                Outcome = details.Exists(o => o.Type == MARC.Everest.Connectors.ResultDetailType.Error) ? ResultCode.Error : ResultCode.Accepted
            };
        }

        public FhirQueryResult Query(NameValueCollection parameters)
        {
            throw new NotImplementedException();
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
                Results = new List<ResourceBase>() { this.MapToFhir(result) },
                Details = details
            };
        }

        /// <summary>
        /// Updates the specified record
        /// </summary>
        public FhirOperationResult Update(string id, ResourceBase target, TransactionMode mode)
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
                Results = new List<ResourceBase>() { this.MapToFhir(result) },
                Details = issues,
                Outcome = issues.Exists(o => o.Type == MARC.Everest.Connectors.ResultDetailType.Error) ? ResultCode.Error : ResultCode.Accepted
            };

        }
    }
}