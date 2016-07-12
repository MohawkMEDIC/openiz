using OpenIZ.Core.Http.Description;
using System;
using System.Collections.Generic;

namespace OpenIZ.Core.Http
{
	/// <summary>
	/// Represents a RESTFul client which encapsulates some of the functions of the request
	/// </summary>
	public interface IRestClient : IDisposable
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
	}
}