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
 * Date: 2016-6-14
 */

using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using System;
using System.Collections.Generic;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Resource handler
	/// </summary>
	public interface IResourceHandler
	{
		/// <summary>
		/// Gets the deserialization type for JSON
		/// </summary>
		Type Type { get; }

		/// <summary>
		/// Get the resource name handled
		/// </summary>
		String ResourceName { get; }

		/// <summary>
		/// Get the specified resource instance
		/// </summary>
		IdentifiedData Get(Guid id, Guid versionId);

		/// <summary>
		/// Perform a query
		/// </summary>
		IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters);

		/// <summary>
		/// Perform a query with offset/count
		/// </summary>
		IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out Int32 totalCount);

		/// <summary>
		/// Creates the specified data in the persistence store
		/// </summary>
		IdentifiedData Create(IdentifiedData data, bool updateIfExists);

		/// <summary>
		/// Updates the specified object or updates an existing one if it doesn't exist
		/// </summary>
		IdentifiedData Update(IdentifiedData data);

		/// <summary>
		/// Obsoletes the specified data
		/// </summary>
		IdentifiedData Obsolete(Guid key);
	}
}