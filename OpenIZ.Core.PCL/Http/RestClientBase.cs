using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Http.Description;
using OpenIZ.Core.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace OpenIZ.Core.Http
{
	/// <summary>
	/// Represents a simple rest client
	/// </summary>
	public abstract class RestClientBase : IRestClient
	{
		// Configuration
		private IRestClientDescription m_configuration;

		// Get tracer
		private Tracer m_tracer = Tracer.GetTracer(typeof(RestClientBase));

		/// <summary>
		/// Fired on request
		/// </summary>
		public event EventHandler<RestRequestEventArgs> Requesting;

		/// <summary>
		/// Fired on response
		/// </summary>
		public event EventHandler<RestResponseEventArgs> Responded;

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
		public static String CreateQueryString(params KeyValuePair<String, Object>[] query)
		{
			String queryString = String.Empty;
			foreach (var kv in query)
			{
				queryString += String.Format("{0}={1}", kv.Key, Uri.EscapeDataString(kv.Value.ToString()));
				if (!kv.Equals(query.Last()))
					queryString += "&";
			}
			return queryString;
		}

		/// <summary>
		/// Create the HTTP request
		/// </summary>
		/// <param name="url">URL.</param>
		protected virtual WebRequest CreateHttpRequest(String resourceName, params KeyValuePair<String, Object>[] query)
		{
			// URL is relative to base address

			Uri baseUrl = new Uri(this.Description.Endpoint[0].Address);
			UriBuilder uriBuilder = new UriBuilder();
			uriBuilder.Scheme = baseUrl.Scheme;
			uriBuilder.Host = baseUrl.Host;
			uriBuilder.Port = baseUrl.Port;
			uriBuilder.Path = baseUrl.AbsolutePath;
			if (!String.IsNullOrEmpty(resourceName))
				uriBuilder.Path += "/" + resourceName;

			// Add query string
			if (query != null)
				uriBuilder.Query = CreateQueryString(query);

			Uri uri = uriBuilder.Uri;

			// Log
			this.m_tracer.TraceVerbose("Constructing WebRequest to {0}", uriBuilder);

			// Add headers
			HttpWebRequest retVal = (HttpWebRequest)HttpWebRequest.Create(uri.ToString());
			if (this.Credentials != null)
			{
				foreach (var kv in this.Credentials.GetHttpHeaders())
				{
					this.m_tracer.TraceVerbose("Adding header {0}:{1}", kv.Key, kv.Value);
					retVal.Headers[kv.Key] = kv.Value;
				}
			}

			// Return type?
			if (!String.IsNullOrEmpty(this.Accept))
			{
				this.m_tracer.TraceVerbose("Accepts {0}", this.Accept);
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
					this.m_tracer.TraceVerbose("HTTP request cancelled");
					return default(TResult);
				}

				// Invoke
				var retVal = this.InvokeInternal<TBody, TResult>(method, url, contentType, body, query);

				this.Responded?.Invoke(this, new RestResponseEventArgs(method, url, parameters, contentType, retVal, 200));

				return retVal;
			}
			catch (Exception e)
			{
				this.m_tracer.TraceError("Error invoking HTTP: {0}", e);
				this.Responded?.Invoke(this, new RestResponseEventArgs(method, url, parameters, contentType, null, 500));
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
		protected abstract TResult InvokeInternal<TBody, TResult>(string method, string url, string contentType, TBody body, params KeyValuePair<string, object>[] query);

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
		public IRestClientDescription Description { get { return this.m_configuration; } }

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
								int realmStart = wwwAuth.IndexOf("realm=\"") + 7;
								if (realmStart < 0)
									return ServiceClientErrorType.SecurityError; // No realm
								string realm = wwwAuth.Substring(realmStart, wwwAuth.IndexOf('"', realmStart) - realmStart);

								if (!String.IsNullOrEmpty(this.Description.Binding.Security.AuthRealm) &&
									!this.Description.Binding.Security.AuthRealm.Equals(realm))
									return ServiceClientErrorType.RealmMismatch;

								// Credential provider
								if (this.Description.Binding.Security.CredentialProvider != null)
								{
									this.Credentials = this.Description.Binding.Security.CredentialProvider.Authenticate(this);
									return ServiceClientErrorType.Valid;
								}
								else
									return ServiceClientErrorType.SecurityError;
							}
						}
					default:
						return ServiceClientErrorType.Valid;
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
                    this.m_tracer.TraceVerbose("HTTP request cancelled");
                    return null;
                }

                // Invoke
                var httpWebReq = this.CreateHttpRequest(resourceName, query);
                httpWebReq.Method = "HEAD";

                // Get the responst
                Dictionary<String, String> retVal = new Dictionary<string, string>();
                var httpTask = httpWebReq.GetResponseAsync();
                httpTask.ContinueWith(o =>
                {
                    foreach (var itm in o.Result.Headers.AllKeys)
                        retVal.Add(itm, o.Result.Headers[itm]);
                });
                httpTask.Start(); 
                httpTask.Wait();

                this.Responded?.Invoke(this, new RestResponseEventArgs("HEAD", resourceName, parameters, null, null, 200));

                return retVal;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error invoking HTTP: {0}", e);
                this.Responded?.Invoke(this, new RestResponseEventArgs("HEAD", resourceName, parameters, null, null, 500));
                throw;
            }
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
		RealmMismatch
	}
}