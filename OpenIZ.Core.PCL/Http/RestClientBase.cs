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
 * Date: 2016-8-2
 */
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Http.Description;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors.LZMA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenIZ.Core.Http
{
    /// <summary>
    /// Represents a simple rest client
    /// </summary>
    public abstract class RestClientBase : IRestClient
    {
        // Configuration
        private IRestClientDescription m_configuration;
        private ICredentialProvider m_credentialProvider;
        // Get tracer
        private static Tracer s_tracer = Tracer.GetTracer(typeof(RestClientBase));

        /// <summary>
        /// Fired on request
        /// </summary>
        public event EventHandler<RestRequestEventArgs> Requesting;

        /// <summary>
        /// Fired on response
        /// </summary>
        public event EventHandler<RestResponseEventArgs> Responded;

        /// <summary>
        /// Fired on response
        /// </summary>
        public event EventHandler<RestResponseEventArgs> Responding;

        /// <summary>
        /// Progress has changed
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// Fire that progress has changed
        /// </summary>
        protected void FireProgressChanged(object state, float progress)
        {
            ProgressChangedEventArgs e = new ProgressChangedEventArgs(progress, state);
            this.ProgressChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIZ.Core.Http.RestClient"/> class.
        /// </summary>
        public RestClientBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIZ.Core.Http.RestClient"/> class.
        /// </summary>
        /// <param name="binder">The serialization binder to use.</param>
        public RestClientBase(IRestClientDescription config)
        {
            this.m_configuration = config;
        }

        /// <summary>
        /// Create the query string from a list of query parameters
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static String CreateQueryString(NameValueCollection query)
        {
            String queryString = String.Empty;
            foreach (var kv in query)
            {
                foreach (var v in kv.Value)
                {
                    queryString += String.Format("{0}={1}&", kv.Key, Uri.EscapeDataString(v));
                }
            }
            if (queryString.Length > 0)
                return queryString.Substring(0, queryString.Length - 1);
            else
                return queryString;
        }

        /// <summary>
        /// Create the HTTP request
        /// </summary>
        /// <param name="url">URL.</param>
        protected virtual WebRequest CreateHttpRequest(String resourceName, NameValueCollection query)
        {
            // URL is relative to base address
            if (this.Description.Endpoint.Count == 0)
                throw new InvalidOperationException("No endpoints found, is the interface configured properly?");
            Uri baseUrl = new Uri(this.Description.Endpoint[0].Address);
            UriBuilder uriBuilder = new UriBuilder(baseUrl);

            if (!String.IsNullOrEmpty(resourceName))
                uriBuilder.Path += "/" + resourceName;

            // HACK:
            uriBuilder.Path = uriBuilder.Path.Replace("//", "/");
            
            // Add query string
            if (query != null)
                uriBuilder.Query = CreateQueryString(query);

            Uri uri = uriBuilder.Uri;

            // Log
            s_tracer.TraceVerbose("Constructing WebRequest to {0}", uriBuilder);

            // Add headers
            HttpWebRequest retVal = (HttpWebRequest)HttpWebRequest.Create(uri.ToString());

            if (this.Credentials == null &&
                this.Description.Binding.Security?.CredentialProvider != null &&
                this.Description.Binding.Security?.PreemptiveAuthentication == true)
                this.Credentials = this.Description.Binding.Security.CredentialProvider.GetCredentials(this);

            if (this.Credentials != null)
            {
                foreach (var kv in this.Credentials.GetHttpHeaders())
                {
                    s_tracer.TraceVerbose("Adding header {0}:{1}", kv.Key, kv.Value);
                    retVal.Headers[kv.Key] = kv.Value;
                }
            }

            // Compress?
            if (this.Description.Binding.Optimize)
                retVal.Headers[HttpRequestHeader.AcceptEncoding] =  "lzma,bzip2,gzip,deflate";


            // Return type?
            if (!String.IsNullOrEmpty(this.Accept))
            {
                s_tracer.TraceVerbose("Accepts {0}", this.Accept);
                retVal.Accept = this.Accept;
            }

            return retVal;
        }

        #region IRestClient implementation



        /// <summary>
        /// Gets the specified item
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="queryString">Query string.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <param name="url">URL.</param>
        /// <typeparam name="TResult">The 1st type parameter.</typeparam>
        public TResult Get<TResult>(string url)
        {
            return this.Get<TResult>(url, null);
        }

        /// <summary>
        /// Gets a inumerable result set of type T
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="query">Query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <param name="url">URL.</param>
        /// <typeparam name="TResult">The 1st type parameter.</typeparam>
        public TResult Get<TResult>(string url, params KeyValuePair<string, object>[] query)
        {
            return this.Invoke<Object, TResult>("GET", url, null, null, query);
        }

        /// <summary>
        /// Gets the specified URL in raw format
        /// </summary>
        public byte[] Get(String url)
        {
            NameValueCollection parameters = new NameValueCollection();

            try
            {


                var requestEventArgs = new RestRequestEventArgs("GET", url, null, null, null);
                this.Requesting?.Invoke(this, requestEventArgs);
                if (requestEventArgs.Cancel)
                {
                    s_tracer.TraceVerbose("HTTP request cancelled");
                    return null;
                }

                // Invoke
                var httpWebReq = this.CreateHttpRequest(url, requestEventArgs.Query);
                httpWebReq.Method = "GET";

                // Get the responst
                byte[] retVal = null;
                Exception requestException = null;
                var httpTask = httpWebReq.GetResponseAsync().ContinueWith(o =>
                {
                    if (o.IsFaulted)
                        requestException = o.Exception.InnerExceptions.First();
                    else
                        try
                        {
                            this.Responding?.Invoke(this, new RestResponseEventArgs("GET", url, null, null, null, 200, o.Result.ContentLength));

                            byte[] buffer = new byte[2048];
                            int br = 1;
                            using (var ms = new MemoryStream())
                            using (var httpStream = o.Result.GetResponseStream())
                            {
                                while (br > 0)
                                {
                                    br = httpStream.Read(buffer, 0, 2048);
                                    ms.Write(buffer, 0, br);
                                    // Raise event 
                                    this.FireProgressChanged(o.Result.ContentType, ms.Length / (float)o.Result.ContentLength);
                                }


                                ms.Seek(0, SeekOrigin.Begin);

                                switch (o.Result.Headers["Content-Encoding"])
                                {
                                    case "deflate":
                                        using (var dfs = new DeflateStream(ms, CompressionMode.Decompress, leaveOpen: true))
                                        using (var oms = new MemoryStream())
                                        {
                                            dfs.CopyTo(oms);
                                            retVal = oms.ToArray();
                                        }
                                        break;
                                    case "gzip":
                                        using (var gzs = new GZipStream(ms, CompressionMode.Decompress, leaveOpen: true))
                                        using (var oms = new MemoryStream())
                                        {
                                            gzs.CopyTo(oms);
                                            retVal = oms.ToArray();
                                        }
                                            break;
                                    case "bzip2":
                                        using (var lzmas = new BZip2Stream(ms, CompressionMode.Decompress, leaveOpen: true))
                                        using (var oms = new MemoryStream())
                                        {
                                            lzmas.CopyTo(oms);
                                            retVal = oms.ToArray();
                                        }
                                        break;
                                    case "lzma":
                                        using (var lzmas = new LZipStream(ms, CompressionMode.Decompress, leaveOpen: true))
                                        using (var oms = new MemoryStream())
                                        {
                                            lzmas.CopyTo(oms);
                                            retVal = oms.ToArray();
                                        }
                                        break;
                                    default:
                                        retVal = ms.ToArray();
                                        break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            s_tracer.TraceError("Error downloading {0}: {1}", url, e.Message);
                        }
                }, TaskContinuationOptions.LongRunning);
                httpTask.Wait();
                if (requestException != null)
                    throw requestException;


                this.Responded?.Invoke(this, new RestResponseEventArgs("GET", url, null, null, null, 200, 0));

                return retVal;
            }
            catch(WebException e)
            {
                switch (this.ValidateResponse(e.Response)) {
                    case ServiceClientErrorType.Valid:
                        return this.Get(url);
                    default:
                        throw new RestClientException<byte[]>(
                                            null,
                                            e,
                                            e.Status,
                                            e.Response);
                }
            }
            catch (Exception e)
            {
                s_tracer.TraceError("Error invoking HTTP: {0}", e.Message);
                this.Responded?.Invoke(this, new RestResponseEventArgs("GET", url, null, null, null, 500, 0));
                throw;
            }
        }

        /// <summary>
        /// Invokes the specified method against the URL provided
        /// </summary>
        /// <param name="method">Method.</param>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="contentType">Content type.</param>
        /// <param name="body">Body.</param>
        /// <typeparam name="TBody">The 1st type parameter.</typeparam>
        /// <typeparam name="TResult">The 2nd type parameter.</typeparam>
        /// <param name="url">URL.</param>
        public TResult Invoke<TBody, TResult>(string method, string url, string contentType, TBody body)
        {
            return this.Invoke<TBody, TResult>(method, url, contentType, body, null);
        }

        /// <summary>
        /// Invoke the specified method
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <param name="body"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public TResult Invoke<TBody, TResult>(string method, string url, string contentType, TBody body, params KeyValuePair<string, object>[] query)
        {
            NameValueCollection parameters = new NameValueCollection();

            try
            {
                if (query != null)
                {
                    parameters = new NameValueCollection(query);
                }

                var requestEventArgs = new RestRequestEventArgs(method, url, parameters, contentType, body);
                this.Requesting?.Invoke(this, requestEventArgs);
                if (requestEventArgs.Cancel)
                {
                    s_tracer.TraceVerbose("HTTP request cancelled");
                    return default(TResult);
                }

                // Invoke
                WebHeaderCollection responseHeaders = null;
                var retVal = this.InvokeInternal<TBody, TResult>(requestEventArgs.Method, requestEventArgs.Url, requestEventArgs.ContentType, requestEventArgs.AdditionalHeaders, out responseHeaders, body, requestEventArgs.Query);
                this.Responded?.Invoke(this, new RestResponseEventArgs(requestEventArgs.Method, requestEventArgs.Url, requestEventArgs.Query, requestEventArgs.ContentType, retVal, 200, 0));
                return retVal;
            }
            catch (Exception e)
            {
                s_tracer.TraceError("Error invoking HTTP: {0}", e.Message);
                this.Responded?.Invoke(this, new RestResponseEventArgs(method, url, parameters, contentType, null, 500, 0));
                throw;
            }
        }

        /// <summary>
        /// Invokes the specified method against the url provided
        /// </summary>
        /// <param name="method">Method.</param>
        /// <param name="url">URL.</param>
        /// <param name="contentType">Content type.</param>
        /// <param name="body">Body.</param>
        /// <param name="query">Query.</param>
        /// <typeparam name="TBody">The 1st type parameter.</typeparam>
        /// <typeparam name="TResult">The 2nd type parameter.</typeparam>
        protected abstract TResult InvokeInternal<TBody, TResult>(string method, string url, string contentType, WebHeaderCollection requestHeaders, out WebHeaderCollection responseHeaders, TBody body, NameValueCollection query);

        /// <summary>
        /// Executes a post against the url
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="contentType">Content type.</param>
        /// <param name="body">Body.</param>
        /// <typeparam name="TBody">The 1st type parameter.</typeparam>
        /// <typeparam name="TResult">The 2nd type parameter.</typeparam>
        public TResult Post<TBody, TResult>(string url, string contentType, TBody body)
        {
            return this.Invoke<TBody, TResult>("POST", url, contentType, body);
        }

        /// <summary>
        /// Deletes the specified object
        /// </summary>
        /// <param name="url">URL.</param>
        /// <typeparam name="TResult">The 1st type parameter.</typeparam>
        public TResult Delete<TResult>(string url)
        {
            return this.Invoke<Object, TResult>("DELETE", url, null, null);
        }

        /// <summary>
        /// Executes a PUT for the specified object
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="contentType">Content type.</param>
        /// <param name="body">Body.</param>
        /// <typeparam name="TBody">The 1st type parameter.</typeparam>
        /// <typeparam name="TResult">The 2nd type parameter.</typeparam>
        public TResult Put<TBody, TResult>(string url, string contentType, TBody body)
        {
            return this.Invoke<TBody, TResult>("PUT", url, contentType, body);
        }

        /// <summary>
        /// Executes an Options against the URL
        /// </summary>
        /// <param name="url">URL.</param>
        /// <typeparam name="TResult">The 1st type parameter.</typeparam>
        public TResult Options<TResult>(string url)
        {
            return this.Invoke<Object, TResult>("OPTIONS", url, null, null);
        }

        /// <summary>
        /// Gets or sets the credentials to be used for this client
        /// </summary>
        /// <value>The credentials.</value>
        public Credentials Credentials
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of acceptable response formats
        /// </summary>
        /// <value>The accept.</value>
        public string Accept
        {
            get;
            set;
        }

        /// <summary>
        /// Get the description of this service
        /// </summary>
        /// <value>The description.</value>
        public IRestClientDescription Description { get { return this.m_configuration; } set { this.m_configuration = value; } }

        #endregion IRestClient implementation

        /// <summary>
        /// Validate the response
        /// </summary>
        /// <returns><c>true</c>, if response was validated, <c>false</c> otherwise.</returns>
        /// <param name="response">Response.</param>
        protected virtual ServiceClientErrorType ValidateResponse(WebResponse response)
        {
            if (response is HttpWebResponse)
            {
                var httpResponse = response as HttpWebResponse;
                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        {
                            if (response.Headers["WWW-Authenticate"]?.StartsWith(this.Description.Binding.Security.Mode.ToString(), StringComparison.CurrentCultureIgnoreCase) == false)
                                return ServiceClientErrorType.AuthenticationSchemeMismatch;
                            else
                            {
                                // Validate the realm
                                string wwwAuth = response.Headers["WWW-Authenticate"];
                                int realmStart = wwwAuth.IndexOf("realm=\"");
                                if (realmStart < 0)
                                    return ServiceClientErrorType.SecurityError; // No realm
                                realmStart += 7;// skip realm
                                string realm = wwwAuth.Substring(realmStart, wwwAuth.IndexOf('"', realmStart) - realmStart);

                                if (!String.IsNullOrEmpty(this.Description.Binding.Security.AuthRealm) &&
                                    !this.Description.Binding.Security.AuthRealm.Equals(realm))
                                {
                                    s_tracer.TraceWarning("Warning: REALM mismatch, authentication may fail. Server reports {0} but client configured for {1}", realm, this.Description.Binding.Security.AuthRealm);
                                }

                                // Credential provider
                                if (this.Description.Binding.Security.CredentialProvider != null)
                                {
                                    this.Credentials = this.Description.Binding.Security.CredentialProvider.Authenticate(this);
                                    if (this.Credentials != null)
                                        return ServiceClientErrorType.Valid;
                                    else
                                        return ServiceClientErrorType.SecurityError;
                                }
                                else
                                    return ServiceClientErrorType.SecurityError;
                            }
                        }
                    case HttpStatusCode.ServiceUnavailable:
                        return ServiceClientErrorType.NotReady;
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.NotModified:
                    case HttpStatusCode.Created:
                    case HttpStatusCode.Redirect:
                    case HttpStatusCode.Moved:
                        return ServiceClientErrorType.Valid;
                    default:
                        return ServiceClientErrorType.GenericError;
                }
            }
            else
                return ServiceClientErrorType.GenericError;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="OpenIZ.Core.Http.RestClientBase"/>.
        /// The <see cref="Dispose"/> method leaves the <see cref="OpenIZ.Core.Http.RestClientBase"/> in an unusable
        /// state. After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="OpenIZ.Core.Http.RestClientBase"/> so the garbage collector can reclaim the memory that the
        /// <see cref="OpenIZ.Core.Http.RestClientBase"/> was occupying.</remarks>
        public void Dispose()
        {
        }

        /// <summary>
        /// Patches the specified <paramref name="patch"/>
        /// </summary>
        public String Patch<TPatch>(string url, string contentType, String ifMatch, TPatch patch)
        {
            try
            {

                var requestEventArgs = new RestRequestEventArgs("PATCH", url, null, contentType, patch);
                this.Requesting?.Invoke(this, requestEventArgs);
                if (requestEventArgs.Cancel)
                {
                    s_tracer.TraceVerbose("HTTP request cancelled");
                    return null;
                }

                WebHeaderCollection requestHeaders = requestEventArgs.AdditionalHeaders ?? new WebHeaderCollection(),
                    responseHeaders = null;
                requestHeaders[HttpRequestHeader.IfMatch] = ifMatch;

                // Invoke
                this.InvokeInternal<TPatch, Object>("PATCH", url, contentType, requestHeaders, out responseHeaders, patch, null);

                // Return the ETag of the 
                return responseHeaders["ETag"];
            }
            catch (Exception e)
            {
                s_tracer.TraceError("Error invoking HTTP: {0}", e.Message);
                this.Responded?.Invoke(this, new RestResponseEventArgs("PATCH", url, null, contentType, null, 500, 0));
                throw;
            }

        }

        /// <summary>
        /// Perform a head operation against the specified url
        /// </summary>
        public IDictionary<string, string> Head(string resourceName, params KeyValuePair<String, Object>[] query)
        {
            NameValueCollection parameters = new NameValueCollection();

            try
            {
                if (query != null)
                {
                    parameters = new NameValueCollection(query);
                }

                var requestEventArgs = new RestRequestEventArgs("HEAD", resourceName, parameters, null, null);
                this.Requesting?.Invoke(this, requestEventArgs);
                if (requestEventArgs.Cancel)
                {
                    s_tracer.TraceVerbose("HTTP request cancelled");
                    return null;
                }

                // Invoke
                var httpWebReq = this.CreateHttpRequest(resourceName, requestEventArgs.Query);
                httpWebReq.Method = "HEAD";

                // Get the responst
                Dictionary<String, String> retVal = new Dictionary<string, string>();
                Exception fault = null;
                var httpTask = httpWebReq.GetResponseAsync().ContinueWith(o =>
                {
                    if (o.IsFaulted)
                        fault = o.Exception.InnerExceptions.First();
                    else
                    {
                        this.Responding?.Invoke(this, new RestResponseEventArgs("HEAD", resourceName, parameters, null, null, 200, o.Result.ContentLength));
                        foreach (var itm in o.Result.Headers.AllKeys)
                            retVal.Add(itm, o.Result.Headers[itm]);
                    }
                }, TaskContinuationOptions.LongRunning);
                httpTask.Wait();
                if (fault != null)
                    throw fault;
                this.Responded?.Invoke(this, new RestResponseEventArgs("HEAD", resourceName, parameters, null, null, 200, 0));

                return retVal;
            }
            catch (Exception e)
            {
                s_tracer.TraceError("Error invoking HTTP: {0}", e.Message);
                this.Responded?.Invoke(this, new RestResponseEventArgs("HEAD", resourceName, parameters, null, null, 500, 0));
                throw;
            }
        }

        /// <summary>
        /// Fire responding event
        /// </summary>
        protected void FireResponding(RestResponseEventArgs args)
        {
            this.Responding?.Invoke(this, args);
        }

    }
    /// <summary>
    /// Service client error type
    /// </summary>
    public enum ServiceClientErrorType
    {
        Valid,
        GenericError,
        AuthenticationSchemeMismatch,
        SecurityError,
        RealmMismatch,
        NotReady
    }
}