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
using OpenIZ.Core.Http.Description;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Net;

namespace OpenIZ.Core.Http
{
	/// <summary>
	/// Represents a RESTFul client which encapsulates some of the functions of the request
	/// </summary>
	public interface IRestClient : IDisposable, IReportProgressChanged
	{
		/// <summary>
		/// Gets or sets the credentials to be used for this client
		/// </summary>
		Credentials Credentials
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a list of acceptable response formats
		/// </summary>
		/// <value>The accept.</value>
		String Accept { get; set; }

		/// <summary>
		/// Gets the specified item
		/// </summary>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="queryString">Query string.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		TResult Get<TResult>(String url);

		/// <summary>
		/// Gets a inumerable result set of type T
		/// </summary>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="query">Query.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		TResult Get<TResult>(String url, params KeyValuePair<String, Object>[] query);

		/// <summary>
		/// Invokes the specified method against the URL provided
		/// </summary>
		/// <param name="method">Method.</param>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="contentType">Content type.</param>
		/// <param name="body">Body.</param>
		/// <typeparam name="TBody">The 1st type parameter.</typeparam>
		/// <typeparam name="TResult">The 2nd type parameter.</typeparam>
		TResult Invoke<TBody, TResult>(String method, String url, String contentType, TBody body);

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
		TResult Invoke<TBody, TResult>(String method, String url, String contentType, TBody body, params KeyValuePair<String, Object>[] query);

        /// <summary>
        /// Instructs the server to perform a PATCH operation
        /// </summary>
        /// <typeparam name="TPatch">The type of patch being applied</typeparam>
        /// <param name="url">The URL</param>
        /// <param name="contentType">The content type</param>
        /// <param name="ifMatch">Target version/etag to patch</param>
        String Patch<TPatch>(string url, string contentType, String ifMatch, TPatch patch);


        /// <summary>
        /// Executes a post against the url
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="contentType">Content type.</param>
        /// <param name="body">Body.</param>
        /// <typeparam name="TBody">The 1st type parameter.</typeparam>
        /// <typeparam name="TResult">The 2nd type parameter.</typeparam>
        TResult Post<TBody, TResult>(String url, String contentType, TBody body);

		/// <summary>
		/// Deletes the specified object
		/// </summary>
		/// <param name="url">URL.</param>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		TResult Delete<TResult>(String url);

		/// <summary>
		/// Executes a PUT for the specified object
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="contentType">Content type.</param>
		/// <param name="body">Body.</param>
		/// <typeparam name="TBody">The 1st type parameter.</typeparam>
		/// <typeparam name="TResult">The 2nd type parameter.</typeparam>
		TResult Put<TBody, TResult>(String url, String contentType, TBody body);

		/// <summary>
		/// Executes an Options against the URL
		/// </summary>
		/// <param name="url">URL.</param>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		TResult Options<TResult>(String url);

        /// <summary>
        /// Executes a HEAD operation against the URL
        /// </summary>
        /// <param name="url">URL.</param>
        /// <typeparam name="TResult">The 1st type parameter.</typeparam>
        IDictionary<String, String> Head(String resourceName, params KeyValuePair<String, Object>[] query);

        /// <summary>
        /// Perform a raw get
        /// </summary>
        byte[] Get(String url);

        /// <summary>
        /// Gets the service client description
        /// </summary>
        /// <value>The description.</value>
        IRestClientDescription Description { get; }

		/// <summary>
		/// Fired prior to rest client invoking a method
		/// </summary>
		event EventHandler<RestRequestEventArgs> Requesting;

		/// <summary>
		/// Fired after the request has been finished
		/// </summary>
		event EventHandler<RestResponseEventArgs> Responded;

        /// <summary>
        /// Fired on response
        /// </summary>
        event EventHandler<RestResponseEventArgs> Responding;

    }
}