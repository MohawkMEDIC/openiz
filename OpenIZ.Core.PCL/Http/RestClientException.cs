using System;
using System.Net;

namespace OpenIZ.Core.PCL.Http
{
	/// <summary>
	/// Rest client exception.
	/// </summary>
	public class RestClientException<TResult> : System.Net.WebException
	{
		/// <summary>
		/// The result
		/// </summary>
		/// <value>The result.</value>
		public TResult Result {
			get;
			set;
		}



		/// <summary>
		/// Create the client exception
		/// </summary>
		public RestClientException (TResult result, Exception inner, WebExceptionStatus status, WebResponse response) : base("Request failed", inner, status, response)
		{
			this.Result = result;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format ("[RestClientException: {0}, Result={1}]\r\n{2}", this.Message, Result, this.StackTrace);
		}
	}
}

