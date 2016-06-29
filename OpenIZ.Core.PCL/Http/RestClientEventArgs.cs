using OpenIZ.Core.Model.Query;
using System;
using System.Collections.Generic;

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
        public NameValueCollection  Query { get; private set; }

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

        }

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
        public RestResponseEventArgs(String method, String url, NameValueCollection query, String contentType, Object responseBody, int statusCode) : 
            base(method, url, query, contentType, responseBody)
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Identifies the response code
        /// </summary>
        public int StatusCode { get; set; }

    }
}