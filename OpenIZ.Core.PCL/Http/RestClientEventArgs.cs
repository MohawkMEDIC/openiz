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
using OpenIZ.Core.Model.Query;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

namespace OpenIZ.Core.Http
{
    /// <summary>
    /// Rest client request
    /// </summary>
    public class RestClientEventArgsBase : EventArgs
    {
        /// <summary>
        /// Rest client event args
        /// </summary>
        protected RestClientEventArgsBase(String method, String url, NameValueCollection query, String contentType, Object body)
        {
            this.Method = method;
            this.Url = url;
            this.Query = query;
            this.Body = body;
            this.ContentType = contentType;
        }

        /// <summary>
        /// Query passed to the request
        /// </summary>
        public NameValueCollection Query { get; private set; }

        /// <summary>
        /// Gets the method
        /// </summary>
        public String Method { get; private set; }

        /// <summary>
        /// Gets or sets the URL of the request
        /// </summary>
        public String Url { get; private set; }

        /// <summary>
        /// Gets or sets the body of the request / response
        /// </summary>
        public Object Body { get; private set; }

        /// <summary>
        /// Gets the content type
        /// </summary>
        public String ContentType { get; private set; }
    }

    /// <summary>
    /// Request event args
    /// </summary>
    public class RestRequestEventArgs : RestClientEventArgsBase
    {
        /// <summary>
        /// Creates the request event args with the specified values
        /// </summary>
        public RestRequestEventArgs(String method, String url, NameValueCollection query, String contentType, Object body) :
            base(method, url, query, contentType, body)
        {
            this.AdditionalHeaders = new WebHeaderCollection();
        }

        /// <summary>
        /// Gets or sets additional headers
        /// </summary>
        public WebHeaderCollection AdditionalHeaders { get; set; }

        /// <summary>
        /// Gets or sets an indicator whether this request can be cancelled
        /// </summary>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// Rest client event args
    /// </summary>
    public class RestResponseEventArgs : RestClientEventArgsBase
    {
        /// <summary>
        /// REST response client event args
        /// </summary>
        public RestResponseEventArgs(String method, String url, NameValueCollection query, String contentType, Object responseBody, int statusCode, long contentLength) :
            base(method, url, query, contentType, responseBody)
        {
            this.StatusCode = statusCode;
            this.ContentLength = contentLength;
        }

        /// <summary>
        /// Content length
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// Identifies the response code
        /// </summary>
        public int StatusCode { get; set; }
    }

}