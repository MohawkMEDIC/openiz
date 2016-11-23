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
 * Date: 2016-11-19
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Collection;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Represents a resource handler for concept reference terms.
	/// </summary>
	public class ConceptNameResourceHandler : IResourceHandler
	{
		/// <summary>
		/// The internal reference to the <see cref="IConceptRepositoryService"/> instance.
		/// </summary>
		private IConceptRepositoryService repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptNameResourceHandler"/> class.
        /// </summary>
        public ConceptNameResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => { this.repository = ApplicationContext.Current.GetService<IConceptRepositoryService>(); };
		}

		/// <summary>
		/// Gets the name of the resource which the resource handler supports.
		/// </summary>
		public string ResourceName => nameof(ConceptName);

		/// <summary>
		/// Gets the type which the resource handler supports.
		/// </summary>
		public Type Type => typeof(ConceptName);

		/// <summary>
		/// Creates a resource.
		/// </summary>
		/// <param name="data">The resource data to be created.</param>
		/// <param name="updateIfExists">Updates the resource if the resource exists.</param>
		/// <returns>Returns the created resource.</returns>
		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			var bundleData = data as Bundle;

			bundleData?.Reconstitute();

			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException($"Bundle must have entry of type {nameof(ConceptName)}");
			}
			else if (processData is ConceptName)
			{
				var conceptName = data as ConceptName;

				if (updateIfExists)
				{
					return this.repository.SaveConceptName(conceptName);
				}
				else
				{
					return this.repository.InsertConceptName(conceptName);
				}
			}
			else
			{
				throw new ArgumentException("Invalid persistence type");
			}
		}

		/// <summary>
		/// Gets a specific resource instance.
		/// </summary>
		/// <param name="id">The id of the resource.</param>
		/// <param name="versionId">The version id of the resource.</param>
		/// <returns>Returns the resource.</returns>
		public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.repository.GetConceptName(id);
		}

		/// <summary>
		/// Obsoletes a resource.
		/// </summary>
		/// <param name="key">The key of the resource to obsolete.</param>
		/// <returns>Returns the obsoleted resource.</returns>
		public IdentifiedData Obsolete(Guid key)
		{
			return this.repository.GetConceptName(key);
		}

		/// <summary>
		/// Queries for a resource.
		/// </summary>
		/// <param name="queryParameters">The query parameters of the resource.</param>
		/// <returns>Returns a collection of resources.</returns>
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
			return this.repository.FindConceptNames(QueryExpressionParser.BuildLinqExpression<ConceptName>(queryParameters));
		}

		/// <summary>
		/// Queries for a resource.
		/// </summary>
		/// <param name="queryParameters">The query parameters of the resource.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the results.</param>
		/// <returns>Returns a collection of resources.</returns>
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
			return this.repository.FindConceptNames(QueryExpressionParser.BuildLinqExpression<ConceptName>(queryParameters), offset, count, out totalCount);
		}

		/// <summary>
		/// Updates a resource.
		/// </summary>
		/// <param name="data">The resource data to be updated.</param>
		/// <returns>Returns the updated resource.</returns>
		public IdentifiedData Update(IdentifiedData data)
		{
			var bundleData = data as Bundle;

			bundleData?.Reconstitute();

			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException(string.Format("Bundle must have entry of type {0}", nameof(ConceptName)));
			}
			else if (processData is ConceptName)
			{
				var conceptNameData = data as ConceptName;

				return this.repository.SaveConceptName(conceptNameData);
			}
			else
			{
				throw new ArgumentException("Invalid persistence type");
			}
		}
	}
}
