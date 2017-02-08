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
 * Date: 2016-8-12
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Represents a IMSI handler for manufactured materials
	/// </summary>
	public class ManufacturedMaterialHandler : IResourceHandler
	{
		// Repository
		private IMaterialRepositoryService m_repository;

		public ManufacturedMaterialHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.m_repository = ApplicationContext.Current.GetService<IMaterialRepositoryService>();
		}

		/// <summary>
		/// Gets the resource name
		/// </summary>
		public string ResourceName
		{
			get
			{
				return "ManufacturedMaterial";
			}
		}

		/// <summary>
		/// Gets the type that this handler services
		/// </summary>
		public Type Type
		{
			get
			{
				return typeof(ManufacturedMaterial);
			}
		}

		/// <summary>
		/// Create the specified material
		/// </summary>
		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle) // Client submitted a bundle
				throw new InvalidOperationException("Bundle must have an entry point");
			else if (processData is ManufacturedMaterial)
			{
				var material = processData as ManufacturedMaterial;
				if (updateIfExists)
					return this.m_repository.Save(material);
				else
					return this.m_repository.Insert(material);
			}
			else
				throw new ArgumentException(nameof(data), "Invalid data type");
		}

		/// <summary>
		/// Gets the specified manufactured material
		/// </summary>
		/// <returns></returns>
		public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.m_repository.GetManufacturedMaterial(id, versionId);
		}

		/// <summary>
		/// Obsoletes the specified material
		/// </summary>
		public IdentifiedData Obsolete(Guid key)
		{
			return this.m_repository.ObsoleteManufacturedMaterial(key);
		}

		/// <summary>
		/// Query for the specified material
		/// </summary>
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
			return this.m_repository.Find(QueryExpressionParser.BuildLinqExpression<ManufacturedMaterial>(queryParameters, null, false));
		}

		/// <summary>
		/// Query for the specified material with restrictions
		/// </summary>
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
			return this.m_repository.Find(QueryExpressionParser.BuildLinqExpression<ManufacturedMaterial>(queryParameters), offset, count, out totalCount);
		}

		/// <summary>
		/// Update the specified material
		/// </summary>
		public IdentifiedData Update(IdentifiedData data)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			var bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var saveData = bundleData?.Entry ?? data;

			if (saveData is Bundle)
				throw new InvalidOperationException("Bundle must have an entry");
			else if (saveData is ManufacturedMaterial)
				return this.m_repository.Save(saveData as ManufacturedMaterial);
			else
				throw new ArgumentException(nameof(data), "Invalid storage type");
		}
	}
}