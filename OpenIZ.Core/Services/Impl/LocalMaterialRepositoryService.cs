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
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Exceptions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Local material persistence service
	/// </summary>
	public class LocalMaterialRepositoryService : IMaterialRepositoryService
	{
		/// <summary>
		/// Find manufactured material
		/// </summary>
		public IEnumerable<ManufacturedMaterial> FindManufacturedMaterial(Expression<Func<ManufacturedMaterial, bool>> expression)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ManufacturedMaterial>)} not found");
			}

			return persistenceService.Query(expression, AuthenticationContext.Current.Principal);
		}

		/// <summary>
		/// Find manufactured material
		/// </summary>
		public IEnumerable<ManufacturedMaterial> FindManufacturedMaterial(Expression<Func<ManufacturedMaterial, bool>> expression, int offset, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ManufacturedMaterial>)} not found");
			}

			return persistenceService.Query(expression, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Finds the specified material
		/// </summary>
		public IEnumerable<Material> FindMaterial(Expression<Func<Material, bool>> expression)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Material>)} not found");
			}

			return persistenceService.Query(expression, AuthenticationContext.Current.Principal);
		}

		/// <summary>
		/// Find the specified material
		/// </summary>
		public IEnumerable<Material> FindMaterial(Expression<Func<Material, bool>> expression, int offset, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Material>)} not found");
			}

			return persistenceService.Query(expression, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Get manufactured material
		/// </summary>
		public ManufacturedMaterial GetManufacturedMaterial(Guid id, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ManufacturedMaterial>)} not found");
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
		}

		/// <summary>
		/// Gets the specified identified material
		/// </summary>
		public Material GetMaterial(Guid id, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Material>)} not found");
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
		}

		/// <summary>
		/// Insert manufactured material
		/// </summary>
		public ManufacturedMaterial Insert(ManufacturedMaterial material)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ManufacturedMaterial>)} not found");
			}

			return persistenceService.Insert(material, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Inserts the specified material
		/// </summary>
		public Material Insert(Material material)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Material>)} not found");
			}

			return persistenceService.Insert(material, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsolete the specified material
		/// </summary>
		public ManufacturedMaterial ObsoleteManufacturedMaterial(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ManufacturedMaterial>)} not found");
			}

			return persistenceService.Obsolete(new ManufacturedMaterial() { Key = key }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsoletes the speciied material
		/// </summary>
		public Material ObsoleteMaterial(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Material>)} not found");
			}

			return persistenceService.Obsolete(new Material() { Key = key }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Save the specified manufactured material
		/// </summary>
		public ManufacturedMaterial Save(ManufacturedMaterial material)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ManufacturedMaterial>)} not found");
			}

			try
			{
				return persistenceService.Update(material, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
			catch (DataPersistenceException)
			{
				return persistenceService.Insert(material, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
		}

		/// <summary>
		/// Save the specified material
		/// </summary>
		public Material Save(Material material)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Material>)} not found");
			}

			try
			{
				return persistenceService.Update(material, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
			catch (DataPersistenceException)
			{
				return persistenceService.Insert(material, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
		}
	}
}