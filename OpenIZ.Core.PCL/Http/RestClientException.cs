/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
using System;
using System.Net;

namespace OpenIZ.Core.Http
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
		public TResult Result
		{
			get;
			set;
		}

		/// <summary>
		/// Create the client exception
		/// </summary>
		public RestClientException(TResult result, Exception inner, WebExceptionStatus status, WebResponse response) : base("Request failed", inner, status, response)
		{
			this.Result = result;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return string.Format("[RestClientException: {0}, Result={1}]\r\n{2}", this.Message, Result, this.StackTrace);
		}
	}
}