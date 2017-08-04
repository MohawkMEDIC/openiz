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
 * Date: 2016-6-14
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
using OpenIZ.Core.Model.Patch;
using OpenIZ.Core.Exceptions;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using OpenIZ.Core.Interop;
using OpenIZ.Core;
using System.Data.Linq;
using MARC.HI.EHRS.SVC.Core.Exceptions;

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
        /// Ping the server
        /// </summary>
        public void Ping()
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Create the specified resource
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.LoginAsService)]
        public IdentifiedData Create(string resourceType, IdentifiedData body)
        {
            this.ThrowIfNotReady();

            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {

                    var retVal = handler.Create(body, false);

                    var versioned = retVal as IVersionedEntity;
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Created;
                    WebOperationContext.Current.OutgoingResponse.ETag = retVal.Tag;
                    if (versioned != null)
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.ContentLocation, String.Format("{0}/{1}/{2}/history/{3}",
                            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                            resourceType,
                            retVal.Key,
                            versioned.Key));
                    else
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.ContentLocation, String.Format("{0}/{1}/{2}",
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
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create or update the specified object
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.LoginAsService)]
        public IdentifiedData CreateUpdate(string resourceType, string id, IdentifiedData body)
        {
            this.ThrowIfNotReady();
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {
                    var retVal = handler.Create(body, true);
                    var versioned = retVal as IVersionedEntity;
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Created;
                    WebOperationContext.Current.OutgoingResponse.ETag = retVal.Tag;

                    if (versioned != null)
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.ContentLocation, String.Format("{0}/{1}/{2}/history/{3}",
                            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                            resourceType,
                            retVal.Key,
                            versioned.Key));
                    else
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.ContentLocation, String.Format("{0}/{1}/{2}",
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
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;

            }
        }

        /// <summary>
        /// Get the specified object
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.LoginAsService)]
        public IdentifiedData Get(string resourceType, string id)
        {
            this.ThrowIfNotReady();

            try
            {


                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {
                    var retVal = handler.Get(Guid.Parse(id), Guid.Empty);
                    if (retVal == null)
                        throw new FileNotFoundException(id);

                    WebOperationContext.Current.OutgoingResponse.ETag = retVal.Tag;
                    WebOperationContext.Current.OutgoingResponse.LastModified = retVal.ModifiedOn.DateTime;

                    // HTTP IF headers?
                    if (WebOperationContext.Current.IncomingRequest.IfModifiedSince.HasValue &&
                        retVal.ModifiedOn <= WebOperationContext.Current.IncomingRequest.IfModifiedSince ||
                        WebOperationContext.Current.IncomingRequest.IfNoneMatch?.Any(o => retVal.Tag == o) == true)
                    {
                        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotModified;
                        return null;
                    }
                    else if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_bundle"] == "true" ||
                        WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_all"] == "true")
                    {
                        retVal = retVal.GetLocked();
                        ObjectExpander.ExpandProperties(retVal, OpenIZ.Core.Model.Query.NameValueCollection.ParseQueryString(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.Query));
                        ObjectExpander.ExcludeProperties(retVal, OpenIZ.Core.Model.Query.NameValueCollection.ParseQueryString(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.Query));
                        return Bundle.CreateBundle(retVal);
                    }
                    else
                    {
                        return retVal;
                    }
                }
                else
                    throw new FileNotFoundException(resourceType);

            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;

            }
        }

        /// <summary>
        /// Gets a specific version of a resource
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.LoginAsService)]
        public IdentifiedData GetVersion(string resourceType, string id, string versionId)
        {
            this.ThrowIfNotReady();
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {
                    var retVal = handler.Get(Guid.Parse(id), Guid.Parse(versionId));
                    if (retVal == null)
                        throw new FileNotFoundException(id);



                    if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_bundle"] == "true")
                        return Bundle.CreateBundle(retVal);
                    else
                    {
                        WebOperationContext.Current.OutgoingResponse.ETag = retVal.Tag;

                        return retVal;
                    }
                }
                else
                    throw new FileNotFoundException(resourceType);

            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;

            }
        }

        /// <summary>
        /// Get the schema which defines this service
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
        public XmlSchema GetSchema(int schemaId)
        {
            this.ThrowIfNotReady();
            try
            {
                XmlSchemas schemaCollection = new XmlSchemas();

                XmlReflectionImporter importer = new XmlReflectionImporter("http://openiz.org/model");
                XmlSchemaExporter exporter = new XmlSchemaExporter(schemaCollection);

                foreach (var cls in typeof(IImsiServiceContract).GetCustomAttributes<ServiceKnownTypeAttribute>().Select(o => o.Type))
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
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Gets the recent history an object
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.LoginAsService)]
        public IdentifiedData History(string resourceType, string id)
        {
            this.ThrowIfNotReady();
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);

                if (handler != null)
                {
                    String since = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_since"];
                    Guid sinceGuid = since != null ? Guid.Parse(since) : Guid.Empty;

                    // Query 
                    var retVal = handler.Get(Guid.Parse(id), Guid.Empty) as IVersionedEntity;
                    List<IVersionedEntity> histItm = new List<IVersionedEntity>() { retVal };
                    while (retVal.PreviousVersionKey.HasValue)
                    {
                        retVal = handler.Get(Guid.Parse(id), retVal.PreviousVersionKey.Value) as IVersionedEntity;
                        if(retVal != null)
                            histItm.Add(retVal);
                        // Should we stop fetching?
                        if (retVal?.VersionKey == sinceGuid)
                            break;

                    }

                    // Lock the item
                    return BundleUtil.CreateBundle(histItm.OfType<IdentifiedData>(), histItm.Count, 0, false);
                }
                else
                    throw new FileNotFoundException(resourceType);
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;

            }
        }

        /// <summary>
        /// Perform a search on the specified resource type
        /// </summary>
        [PolicyPermissionAttribute(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.LoginAsService)]
        public IdentifiedData Search(string resourceType)
        {
            this.ThrowIfNotReady();
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {
                    String offset = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_offset"],
                        count = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_count"];

                    var query = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.ToQuery();

                    // Modified on?
                    if (WebOperationContext.Current.IncomingRequest.IfModifiedSince.HasValue)
                        query.Add("modifiedOn", ">" + WebOperationContext.Current.IncomingRequest.IfModifiedSince.Value.ToString("o"));

                    // No obsoletion time?
                    if (typeof(BaseEntityData).IsAssignableFrom(handler.Type) && !query.ContainsKey("obsoletionTime"))
                        query.Add("obsoletionTime", "null");

                    int totalResults = 0;

                    // Lean mode
                    var lean = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_lean"];
                    bool parsedLean = false;
                    bool.TryParse(lean, out parsedLean);


                    var retVal = handler.Query(query, Int32.Parse(offset ?? "0"), Int32.Parse(count ?? "100"), out totalResults).Select(o=>o.GetLocked()).ToList();
                    WebOperationContext.Current.OutgoingResponse.LastModified = retVal.OrderByDescending(o => o.ModifiedOn).FirstOrDefault()?.ModifiedOn.DateTime ?? DateTime.Now;


                    // Last modification time and not modified conditions
                    if ((WebOperationContext.Current.IncomingRequest.IfModifiedSince.HasValue ||
                        WebOperationContext.Current.IncomingRequest.IfNoneMatch != null) &&
                        totalResults == 0)
                    {
                        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotModified;
                        return null;
                    }
                    else
                    {
                        if (query.ContainsKey("_all") || query.ContainsKey("_expand") || query.ContainsKey("_exclude"))
                        {
                            using (WaitThreadPool wtp = new WaitThreadPool())
                            {
                                foreach (var itm in retVal)
                                {
                                    wtp.QueueUserWorkItem((o) => {
                                        try
                                        {
                                            var i = o as IdentifiedData;
                                            ObjectExpander.ExpandProperties(i, query);
                                            ObjectExpander.ExcludeProperties(i, query);
                                        }
                                        catch(Exception e)
                                        {
                                            this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, "Error setting properties: {0}", e);
                                        }
                                    }, itm);
                                }
                                wtp.WaitOne();
                            }
                        }

                       
                        return BundleUtil.CreateBundle(retVal, totalResults, Int32.Parse(offset ?? "0"), parsedLean);
                    }
                }
                else
                    throw new FileNotFoundException(resourceType);
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;

            }
        }


        /// <summary>
        /// Get the server's current time
        /// </summary>
        public DateTime Time()
        {
            this.ThrowIfNotReady();
            return DateTime.Now;
        }

        /// <summary>
        /// Update the specified resource
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.LoginAsService)]
        public IdentifiedData Update(string resourceType, string id, IdentifiedData body)
        {
            this.ThrowIfNotReady();
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {

                    var retVal = handler.Update(body);

                    var versioned = retVal as IVersionedEntity;
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.OK;
                    WebOperationContext.Current.OutgoingResponse.ETag = retVal.Tag;

                    if (versioned != null)
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.ContentLocation, String.Format("{0}/{1}/{2}/history/{3}",
                            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                            resourceType,
                            retVal.Key,
                            versioned.Key));
                    else
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.ContentLocation, String.Format("{0}/{1}/{2}",
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
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;

            }
        }

     
        /// <summary>
        /// Obsolete the specified data
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.LoginAsService)]
        public IdentifiedData Delete(string resourceType, string id)
        {
            this.ThrowIfNotReady();
            try
            {
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);
                if (handler != null)
                {

                    var retVal = handler.Obsolete(Guid.Parse(id));

                    var versioned = retVal as IVersionedEntity;

                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Created;
                    if (versioned != null)
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.ContentLocation, String.Format("{0}/{1}/{2}/history/{3}",
                            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                            resourceType,
                            retVal.Key,
                            versioned.Key));
                    else
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.ContentLocation, String.Format("{0}/{1}/{2}",
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
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;

            }
        }

        /// <summary>
        /// Perform the search but only return the headers
        /// </summary>
        public void HeadSearch(string resourceType)
        {
            this.ThrowIfNotReady();
            WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.Add("_count", "1");
            this.Search(resourceType);
        }

        /// <summary>
        /// Get just the headers
        /// </summary>
        public void GetHead(string resourceType, string id)
        {
            this.ThrowIfNotReady();
            this.Get(resourceType, id);
        }

        /// <summary>
        /// Perform a patch on the serviceo
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="id"></param>
        /// <param name="body"></param>
        public void Patch(string resourceType, string id, Patch body)
        {
            this.ThrowIfNotReady();
            try
            {
                // Validate
                var match = WebOperationContext.Current.IncomingRequest.Headers["If-Match"];
                if (match == null)
                    throw new InvalidOperationException("Missing If-Match header");

                // Match bin
                var versionId = Guid.ParseExact(match, "N");

                // First we load
                var handler = ResourceHandlerUtil.Current.GetResourceHandler(resourceType);

                if (handler == null)
                    throw new FileNotFoundException(resourceType);

                // Next we get the current version
                var existing = handler.Get(Guid.Parse(id), Guid.Empty);
                var force = Convert.ToBoolean(WebOperationContext.Current.IncomingRequest.Headers["X-Patch-Force"] ?? "false");

                if (existing == null)
                    throw new FileNotFoundException($"/{resourceType}/{id}/history/{versionId}");
                else if (existing.Tag != match && !force)
                {
                    this.m_traceSource.TraceEvent(TraceEventType.Error, -3049, "Object {0} ETAG is {1} but If-Match specified {2}", existing.Key, existing.Tag, match);
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Conflict;
                    WebOperationContext.Current.OutgoingResponse.StatusDescription = ApplicationContext.Current.GetLocaleString("DBPE002");
                    return;
                }
                else if (body == null)
                    throw new ArgumentNullException(nameof(body));
                else
                {
                    // Force load all properties for existing
                    var applied = ApplicationContext.Current.GetService<IPatchService>().Patch(body, existing, force);
                    var data = handler.Update(applied);
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NoContent;
                    WebOperationContext.Current.OutgoingResponse.ETag = data.Tag;
                    WebOperationContext.Current.OutgoingResponse.LastModified = applied.ModifiedOn.DateTime;
                    var versioned = (data as IVersionedEntity)?.VersionKey;
                    if (versioned != null)
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.ContentLocation, String.Format("{0}/{1}/{2}/history/{3}",
                                WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                                resourceType,
                                id,
                                versioned));
                    else
                        WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.ContentLocation, String.Format("{0}/{1}/{2}",
                                WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri,
                                resourceType,
                                id));
                }
            }
            catch(PatchAssertionException e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Warning, e.HResult, e.Message);
                throw;
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;

            }
        }

        public Patch GetPatch(string resourceType, string id)
        {
            this.ThrowIfNotReady();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get options
        /// </summary>
        public IdentifiedData Options()
        {
            this.ThrowIfNotReady();
            try
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.OK;
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Allow", $"GET, PUT, POST, OPTIONS, HEAD, DELETE{(ApplicationContext.Current.GetService<IPatchService>() != null ? ", PATCH" : null)}");
                if (ApplicationContext.Current.GetService<IPatchService>() != null)
                    WebOperationContext.Current.OutgoingResponse.Headers.Add("Accept-Patch", "application/xml+oiz-patch");

                // Service options
                var retVal = new ServiceOptions()
                {
                    InterfaceVersion = typeof(IdentifiedData).Assembly.GetName().Version.ToString(),
                    Services = new List<ServiceResourceOptions>()
                    {
                        new ServiceResourceOptions()
                        {
                            ResourceName = null,
                            Verbs = new List<string>() { "OPTIONS" }
                        },
                        new ServiceResourceOptions()
                        {
                            ResourceName = "time",
                            Verbs = new List<string>() { "GET" }
                        }
                    }
                };

                // Get the resources which are supported
                foreach (var itm in ResourceHandlerUtil.Current.Handlers)
                {
                    var svc = new ServiceResourceOptions()
                    {
                        ResourceName = itm.ResourceName,
                        Verbs = new List<string>()
                        {
                            "GET", "PUT", "POST", "HEAD", "DELETE"
                        }
                    };
                    if (ApplicationContext.Current.GetService<IPatchService>() != null)
                        svc.Verbs.Add("PATCH");
                    retVal.Services.Add(svc);
                }

                return retVal;
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }


        /// <summary>
        /// Throw if the service is not ready
        /// </summary>
        public void ThrowIfNotReady()
        {
            if (!ApplicationContext.Current.IsRunning)
                throw new DomainStateException();
        }
    }
}
