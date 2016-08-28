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
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Represents concept class resource handler.
	/// </summary>
	public class ConceptClassResourceHandler : IResourceHandler
	{
		private IConceptRepositoryService repository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptClassResourceHandler"/> class.
		/// </summary>
		public ConceptClassResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.repository = ApplicationContext.Current.GetService<IConceptRepositoryService>();
		}

		/// <summary>
		/// Gets the resource name.
		/// </summary>
		public string ResourceName
		{
			get
			{
				return "ConceptClass";
			}
		}

		/// <summary>
		/// Gets the resource type.
		/// </summary>
		public Type Type
		{
			get
			{
				return typeof(ConceptClass);
			}
		}

		/// <summary>
		/// Creates an organization.
		/// </summary>
		/// <param name="data">The organization to be created.</param>
		/// <param name="updateIfExists">Update the organization if it exists.</param>
		/// <returns>Returns the newly create organization.</returns>
		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException(string.Format("Bundle must have entry of type {0}", nameof(ConceptClass)));
			}
			else if (processData is ConceptClass)
			{
				var conceptClassData = data as ConceptClass;

				if (updateIfExists)
				{
					return this.repository.SaveConceptClass(conceptClassData);
				}
				else
				{
					return this.repository.InsertConceptClass(conceptClassData);
				}
			}
			else
			{
				throw new ArgumentException("Invalid persistence type");
			}
		}

		/// <summary>
		/// Gets an organization by id and version id.
		/// </summary>
		/// <param name="id">The id of the organization.</param>
		/// <param name="versionId">The version id of the organization.</param>
		/// <returns>Returns the organization.</returns>
		public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.repository.GetConceptClass(id);
		}

		/// <summary>
		/// Obsoletes an organization.
		/// </summary>
		/// <param name="key">The key of the organization to obsolete.</param>
		/// <returns>Returns the obsoleted organization.</returns>
		public IdentifiedData Obsolete(Guid key)
		{
			return this.repository.ObsoleteConceptClass(key);
		}

		/// <summary>
		/// Queries for an organization.
		/// </summary>
		/// <param name="queryParameters">The query parameters for which to use to query for the organization.</param>
		/// <returns>Returns a list of organizations.</returns>
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
			return this.repository.FindConceptClasses(QueryExpressionParser.BuildLinqExpression<ConceptClass>(queryParameters));
		}

		/// <summary>
		/// Queries for an organization.
		/// </summary>
		/// <param name="queryParameters">The query parameters for which to use to query for the organization.</param>
		/// <param name="offset">The query offset.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of organizations.</returns>
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
			return this.repository.FindConceptClasses(QueryExpressionParser.BuildLinqExpression<ConceptClass>(queryParameters), offset, count, out totalCount);
		}

		/// <summary>
		/// Updates an organization.
		/// </summary>
		/// <param name="data">The organization to be updated.</param>
		/// <returns>Returns the updated organization.</returns>
		public IdentifiedData Update(IdentifiedData data)
		{
			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException(string.Format("Bundle must have entry of type {0}", nameof(ConceptClass)));
			}
			else if (processData is ConceptClass)
			{
				var conceptClassData = data as ConceptClass;

				return this.repository.SaveConceptClass(conceptClassData);
			}
			else
			{
				throw new ArgumentException("Invalid persistence type");
			}
		}
	}
}
