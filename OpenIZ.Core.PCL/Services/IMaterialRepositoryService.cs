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
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents the repository handler for materials
	/// </summary>
	public interface IMaterialRepositoryService
	{
		/// <summary>
		/// Finds the specified ManufacturedMaterial
		/// </summary>
		IEnumerable<ManufacturedMaterial> FindManufacturedMaterial(Expression<Func<ManufacturedMaterial, bool>> expression);

		/// <summary>
		/// Finds the specified ManufacturedMaterial with the specified restrictions
		/// </summary>
		IEnumerable<ManufacturedMaterial> FindManufacturedMaterial(Expression<Func<ManufacturedMaterial, bool>> expression, int offset, int? count, out int totalCount);

		/// <summary>
		/// Finds the specified material
		/// </summary>
		IEnumerable<Material> FindMaterial(Expression<Func<Material, bool>> expression);

		/// <summary>
		/// Finds the specified material with the specified restrictions
		/// </summary>
		IEnumerable<Material> FindMaterial(Expression<Func<Material, bool>> expression, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets the specified ManufacturedMaterial from the database
		/// </summary>
		ManufacturedMaterial GetManufacturedMaterial(Guid id, Guid versionId);

		/// <summary>
		/// Gets the specified material from the database
		/// </summary>
		Material GetMaterial(Guid id, Guid versionId);

		/// <summary>
		/// Inserts the ManufacturedMaterial in the persistence layer
		/// </summary>
		ManufacturedMaterial Insert(ManufacturedMaterial ManufacturedMaterial);

		/// <summary>
		/// Inserts the material in the persistence layer
		/// </summary>
		Material Insert(Material material);

		/// <summary>
		/// Obsoletes the specified ManufacturedMaterial
		/// </summary>
		ManufacturedMaterial ObsoleteManufacturedMaterial(Guid key);

		/// <summary>
		/// Obsoletes the specified material
		/// </summary>
		Material ObsoleteMaterial(Guid key);

		/// <summary>
		/// Saves the specified ManufacturedMaterial from data layer
		/// </summary>
		ManufacturedMaterial Save(ManufacturedMaterial ManufacturedMaterial);

		/// <summary>
		/// Saves the specified material from data layer
		/// </summary>
		Material Save(Material material);
	}
}