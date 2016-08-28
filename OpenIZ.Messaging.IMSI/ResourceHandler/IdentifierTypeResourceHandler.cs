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
 * User: khannan
 * Date: 2016-8-27
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Collection;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Represents an identifier type resource handler.
	/// </summary>
	public class IdentifierTypeResourceHandler : IResourceHandler
	{
		/// <summary>
		/// Gets the resource name.
		/// </summary>
		public string ResourceName
		{
			get
			{
				return "IdentifierType";
			}
		}

		/// <summary>
		/// Gets the resource type.
		/// </summary>
		public Type Type
		{
			get
			{
				return typeof(IdentifierType);
			}
		}

		/// <summary>
		/// Creates an identifier type.
		/// </summary>
		/// <param name="data">The identifier type to be created.</param>
		/// <param name="updateIfExists">Update the identifier type if it exists.</param>
		/// <returns>Returns the newly created identifier type.</returns>
		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets an identifier type by id and version id.
		/// </summary>
		/// <param name="id">The id of the identifier type.</param>
		/// <param name="versionId">The version id of the identifier type.</param>
		/// <returns></returns>
		public IdentifiedData Get(Guid id, Guid versionId)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Obsoletes an identifier type.
		/// </summary>
		/// <param name="key">The key of the identifier type to obsolete.</param>
		/// <returns>Returns the obsoleted identifier type.</returns>
		public IdentifiedData Obsolete(Guid key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Queries for an identifier type.
		/// </summary>
		/// <param name="queryParameters">The query parameters for which to use to query for the identifier type.</param>
		/// <returns>Returns a list of identifier types.</returns>
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Queries for an identifier type.
		/// </summary>
		/// <param name="queryParameters">The query parameters for which to use to query for the identifier type.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of identifier types.</returns>
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Updates an identifier type.
		/// </summary>
		/// <param name="data">The identifier type to be updated.</param>
		/// <returns>Returns the updated identifier type.</returns>
		public IdentifiedData Update(IdentifiedData data)
		{
			throw new NotImplementedException();
		}
	}
}
