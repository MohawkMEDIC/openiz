/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-24
 */
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
using System.Collections;
using OpenIZ.Core.Security.Attribute;
using System.Security.Permissions;
using OpenIZ.Core.Security;
using OpenIZ.Messaging.IMSI.Util;
using OpenIZ.Core.Model.Interfaces;
using MARC.Everest.Threading;
using System.Collections.Specialized;

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

        // Lock object
        private object m_lockObject = new object();

        /// <summary>
        /// Load cache
        /// </summary>
        private Dictionary<Object, Object> m_loadCache = new Dictionary<Object, Object>();

        /// <summary>
        /// Create the specified resource
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public IdentifiedData Create(string resourceType, IdentifiedData body)
        {
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {

                    var retVal = handler.Create(body, false);

                    var versioned = retVal as IVersionedEntity;
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Created;
                    if(versioned != null)
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpRequestHeader.ContentLocation, String.Format("{0}/{1}/{2}/history/{3}",
                            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                            resourceType,
                            retVal.Key,
                            versioned.Key));
                    else
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpRequestHeader.ContentLocation, String.Format("{0}/{1}/{2}",
                            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                            resourceType,
                            retVal.Key));

                    return retVal;
                }
                else
                    throw new FileNotFoundException(resourceType);

            }
            catch (Exception e)
            {
                return this.ErrorHelper(e, false);
            }
        }

        /// <summary>
        /// Create or update the specified object
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public IdentifiedData CreateUpdate(string resourceType, string id, IdentifiedData body)
        {
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {

                    var retVal = handler.Create(body, true);

                    var versioned = retVal as IVersionedEntity;
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Created;
                    if (versioned != null)
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpRequestHeader.ContentLocation, String.Format("{0}/{1}/{2}/history/{3}",
                            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                            resourceType,
                            retVal.Key,
                            versioned.Key));
                    else
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpRequestHeader.ContentLocation, String.Format("{0}/{1}/{2}",
                            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                            resourceType,
                            retVal.Key));

                    return retVal;
                }
                else
                    throw new FileNotFoundException(resourceType);

            }
            catch (Exception e)
            {
                return this.ErrorHelper(e, false);
            }
        }

        /// <summary>
        /// Get the specified object
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
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

                    this.ExpandProperties(retVal, WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters);

                    if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_all"] != "true")
                        retVal.Lock();

                    if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_bundle"] == "true")
                        return Bundle.CreateBundle(retVal);
                    else
                    {
                        retVal.Lock();
                        return retVal;
                    }
                }
                else
                    throw new FileNotFoundException(resourceType);

            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                return this.ErrorHelper(e, false);
            }
        }

        /// <summary>
        /// Gets a specific version of a resource
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public IdentifiedData GetVersion(string resourceType, string id, string versionId)
        {
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {
                    var retVal = handler.Get(Guid.Parse(id), Guid.Parse(versionId));
                    if (retVal == null)
                        throw new FileNotFoundException(id);

                    this.ExpandProperties(retVal, WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters);

                    if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_all"] != "true")
                        retVal.Lock();
                    if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_bundle"] == "true")
                        return Bundle.CreateBundle(retVal);
                    else
                        return retVal;
                }
                else
                    throw new FileNotFoundException(resourceType);

            }
            catch (Exception e)
            {
                return this.ErrorHelper(e, false);
            }
        }

        /// <summary>
        /// Get the schema which defines this service
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AccessAdministrativeFunction)]
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

        /// <summary>
        /// Gets the recent history an object
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public IdentifiedData History(string resourceType, string id)
        {
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);

                if (handler != null)
                {
                    String since = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_since"];
                    Guid sinceGuid = since != null ? Guid.Parse(since) : Guid.Empty;

                    // Query 
                    var retVal = handler.Get(Guid.Parse(id), Guid.Empty);
                    var histItm = retVal;
                    while (histItm != null)
                    {
                        this.ExpandProperties(histItm, WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters);
                        histItm = (histItm as IVersionedEntity)?.PreviousVersion as IdentifiedData;

                        // Should we stop fetching?
                        if ((histItm as IVersionedEntity)?.VersionKey == sinceGuid)
                            break;

                    }

                    // Lock the item
                    histItm.Lock();

                    return Bundle.CreateBundle(retVal);
                }
                else
                    throw new FileNotFoundException(resourceType);
            }
            catch (Exception e)
            {
                return this.ErrorHelper(e, false);
            }
        }

        /// <summary>
        /// Perform a search on the specified resource type
        /// </summary>
        [PolicyPermissionAttribute(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.QueryClinicalData)]
        public IdentifiedData Search(string resourceType)
        {
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {
                    String offset = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_offset"],
                        count = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_count"];
                    int totalResults = 0;
                    IEnumerable<IdentifiedData> retVal = handler.Query(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters, Int32.Parse(offset ?? "0"), Int32.Parse(count ?? "100"), out totalResults);

                    using (WaitThreadPool wtp = new WaitThreadPool(Environment.ProcessorCount * 4))
                    {
                        foreach (var itm in retVal)
                            wtp.QueueUserWorkItem(o=>this.ExpandProperties(itm, o as NameValueCollection), WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters);
                        wtp.WaitOne();

                        foreach (var itm in retVal)
                            wtp.QueueUserWorkItem(o => (o as IdentifiedData).Lock(), itm);

                        wtp.WaitOne();
                    }
                    return BundleUtil.CreateBundle(retVal, totalResults, Int32.Parse(offset ?? "0"));
                }
                else
                    throw new FileNotFoundException(resourceType);
            }
            catch(Exception e)
            {
                return this.ErrorHelper(e, false);
            }
        }

        /// <summary>
        /// Get the server's current time
        /// </summary>
        public DateTime Time()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Update the specified resource
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public IdentifiedData Update(string resourceType, string id, IdentifiedData body)
        {
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {

                    var retVal = handler.Update(body);

                    var versioned = retVal as IVersionedEntity;
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.OK;
                    if (versioned != null)
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpRequestHeader.ContentLocation, String.Format("{0}/{1}/{2}/history/{3}",
                            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                            resourceType,
                            retVal.Key,
                            versioned.Key));
                    else
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpRequestHeader.ContentLocation, String.Format("{0}/{1}/{2}",
                            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                            resourceType,
                            retVal.Key));

                    return retVal;
                }
                else
                    throw new FileNotFoundException(resourceType);

            }
            catch (Exception e)
            {
                return this.ErrorHelper(e, false);
            }
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
            //WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Xml;

            this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());

            return result;

        }

        /// <summary>
        /// Expand properties
        /// </summary>
        private void ExpandProperties(IdentifiedData returnValue, NameValueCollection qp)
        {
            if (qp["_expand"] == null)
                return;

            foreach (var nvs in qp["_expand"].Split(','))
            {
                // Get the property the user wants to expand
                object scope = returnValue;
                foreach (var property in nvs.Split('.'))
                {
                    if (scope is IList)
                    {
                        foreach (var sc in scope as IList)
                        {
                            PropertyInfo keyPi = sc.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttribute<XmlElementAttribute>()?.ElementName == property);
                            if (keyPi == null)
                                continue;
                            // Get the backing property
                            PropertyInfo expandProp = sc.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttribute<DelayLoadAttribute>()?.KeyPropertyName == keyPi.Name);
                            if (expandProp != null)
                                scope = expandProp.GetValue(sc);
                            else
                                scope = keyPi.GetValue(sc);

                        }
                    }
                    else
                    {
                        PropertyInfo keyPi = scope.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttribute<XmlElementAttribute>()?.ElementName == property);
                        if (keyPi == null)
                            continue;
                        // Get the backing property
                        PropertyInfo expandProp = scope.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttribute<DelayLoadAttribute>()?.KeyPropertyName == keyPi.Name);

                        Object existing = null;
                        Object keyValue = keyPi.GetValue(scope);

                        if (expandProp != null && expandProp.CanWrite && this.m_loadCache.TryGetValue(keyValue, out existing))
                        {
                            expandProp.SetValue(scope, existing);
                            scope = existing;
                        }
                        else
                        {
                            if (expandProp != null)
                            {
                                scope = expandProp.GetValue(scope);
                                lock(this.m_lockObject)
                                    if(!this.m_loadCache.ContainsKey(keyValue))
                                        this.m_loadCache.Add(keyValue, scope);
                            }
                            else
                                scope = keyValue;
                        }
                    }
                }
            }
        }
        

        /// <summary>
        /// Obsolete the specified data
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.DeleteClinicalData)]
        public IdentifiedData Delete(string resourceType, string id)
        {
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {

                    var retVal = handler.Obsolete(Guid.Parse(id));

                    var versioned = retVal as IVersionedEntity;

                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Created;
                    if (versioned != null)
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpRequestHeader.ContentLocation, String.Format("{0}/{1}/{2}/history/{3}",
                            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                            resourceType,
                            retVal.Key,
                            versioned.Key));
                    else
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpRequestHeader.ContentLocation, String.Format("{0}/{1}/{2}",
                            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                            resourceType,
                            retVal.Key));

                    return retVal;
                }
                else
                    throw new FileNotFoundException(resourceType);

            }
            catch (Exception e)
            {
                return this.ErrorHelper(e, false);
            }
        }
        #endregion
    }
}
