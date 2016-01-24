using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using OpenIZ.Core.Model;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Diagnostics;
using System.ServiceModel.Web;
using System.IO;
using OpenIZ.Core.Model.Attributes;
using System.Xml.Serialization;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Entities;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Messaging.IMSI.ResourceHandler;
using OpenIZ.Messaging.IMSI.Model;
using System.Net;
using System.Data;

namespace OpenIZ.Messaging.IMSI.Wcf
{
    /// <summary>
    /// Data implementation
    /// </summary>
    [ServiceBehavior(ConfigurationName = "IMSI")]
    public class ImsiServiceBehavior : IImsiServiceContract
    {
        // Trace source
        private TraceSource m_traceSource = new TraceSource("OpenIZ.Messaging.IMSI");

        public IdentifiedData Create(string resourceType, IdentifiedData body)
        {
            throw new NotImplementedException();
        }

        public IdentifiedData CreateUpdate(string resourceType, string id, IdentifiedData body)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the specified object
        /// </summary>
        public IdentifiedData Get(string resourceType, string id)
        {

            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {
                    var retVal = handler.Get(Guid.Parse(id), Guid.Empty);
                    if (retVal == null)
                        throw new FileNotFoundException(id);

                    this.ExpandProperties(retVal);

                    if(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_all"] != "true")
                        retVal.Lock();
                    if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_bundle"] == "true")
                        return Bundle.CreateBundle(retVal);
                    else
                        return retVal;
                }
                else
                    throw new FileNotFoundException(resourceType);

            }
            catch(Exception e)
            {
                return this.ErrorHelper(e, false);
            }
        }

        public IdentifiedData GetVersion(string resourceType, string id, string versionId)
        {
            throw new NotImplementedException();
        }

        public XmlSchema GetSchema(int schemaId)
        {
            try
            {
                XmlSchemas schemaCollection = new XmlSchemas();

                XmlReflectionImporter importer = new XmlReflectionImporter("http://openiz.org/model");
                XmlSchemaExporter exporter = new XmlSchemaExporter(schemaCollection);

                foreach (var cls in typeof(IImsiServiceContract).GetCustomAttributes<ServiceKnownTypeAttribute>().Select(o=>o.Type))
                    exporter.ExportTypeMapping(importer.ImportTypeMapping(cls, "http://openiz.org/model"));

                if (schemaId > schemaCollection.Count)
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return null;
                }
                else
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
                    return schemaCollection[schemaId];
                }                
            }
            catch(Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                return null;
            }
        }

        public IdentifiedData History(string resourceType, string id)
        {
            throw new NotImplementedException();
        }

        public IdentifiedData Search(string resourceType)
        {
            throw new NotImplementedException();
        }

        public DateTime Time()
        {
            throw new NotImplementedException();
        }

        public IdentifiedData Update(string resourceType, string id, IdentifiedData body)
        {
            throw new NotImplementedException();
        }

        #region Helper Methods

        /// <summary>
        /// Throw an appropriate exception based on the caught exception
        /// </summary>
        private ErrorResult ErrorHelper(Exception e, bool returnBundle)
        {

            ErrorResult result = new ErrorResult();
            Trace.TraceError(e.ToString());
            result.Details.Add(new ResultDetail(DetailType.Error, e.Message));

            HttpStatusCode retCode = HttpStatusCode.OK;

            if (e is NotSupportedException)
                retCode = System.Net.HttpStatusCode.MethodNotAllowed;
            else if (e is NotImplementedException)
                retCode = System.Net.HttpStatusCode.NotImplemented;
            else if (e is InvalidDataException)
                retCode = HttpStatusCode.BadRequest;
            else if (e is FileLoadException)
                retCode = System.Net.HttpStatusCode.Gone;
            else if (e is FileNotFoundException || e is ArgumentException)
                retCode = System.Net.HttpStatusCode.NotFound;
            else if (e is ConstraintException)
                retCode = (HttpStatusCode)422;
            else
                retCode = System.Net.HttpStatusCode.InternalServerError;

            WebOperationContext.Current.OutgoingResponse.StatusCode = retCode;
            WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Xml;

         
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", "filename=\"error.xml\"");
            throw new WebFaultException<ErrorResult>(result, retCode);

        }

        /// <summary>
        /// Expand properties
        /// </summary>
        private void ExpandProperties(IdentifiedData returnValue)
        {
            if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_expand"] == null)
                return;
            foreach(var nvs in WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_expand"].Split(','))
            {
                // Get the property the user wants to expand
                object scope = returnValue;
                foreach (var property in nvs.Split('.'))
                {
                    PropertyInfo keyPi = scope.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttribute<XmlElementAttribute>()?.ElementName == property);
                    if (keyPi == null)
                        continue;
                    // Get the backing property
                    PropertyInfo expandProp = scope.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttribute<DelayLoadAttribute>()?.KeyPropertyName == keyPi.Name);
                    if (expandProp != null)
                        scope = expandProp.GetValue(scope);
                    else
                        scope = keyPi.GetValue(scope);
                }
            }
        }
        #endregion
    }
}
