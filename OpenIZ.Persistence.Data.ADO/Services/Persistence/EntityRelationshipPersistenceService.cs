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
 * User: khannan
 * Date: 2017-3-19
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Entities;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
	/// <summary>
	/// Represents an entity relationship persistence service.
	/// </summary>
	/// <seealso cref="OpenIZ.Persistence.Data.ADO.Services.Persistence.IdentifiedPersistenceService{OpenIZ.Core.Model.Entities.EntityRelationship, OpenIZ.Persistence.Data.ADO.Data.Model.Entities.DbEntityRelationship}" />
	/// <seealso cref="OpenIZ.Persistence.Data.ADO.Services.IAdoAssociativePersistenceService" />
	public class EntityRelationshipPersistenceService : IdentifiedPersistenceService<EntityRelationship, DbEntityRelationship>, IAdoAssociativePersistenceService
	{
		public IEnumerable GetFromSource(DataContext context, Guid id, decimal? versionSequenceId, IPrincipal principal)
		{
			int tr = 0;
			return this.QueryInternal(context, base.BuildSourceQuery<EntityRelationship>(id, versionSequenceId), 0, null, out tr, principal, false);
		}

		/// <summary>
		/// Performthe actual insert.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		public override EntityRelationship InsertInternal(DataContext context, EntityRelationship data, IPrincipal principal)
		{
			if (data.EffectiveVersionSequenceId == null)
			{
				var entityPersistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>();

				if (entityPersistenceService == null)
				{
					throw new InvalidOperationException($"Unable to locate { nameof(IDataPersistenceService<Entity>) }");
				}

				var entity = entityPersistenceService.Get(new Identifier<Guid>(data.SourceEntityKey.Value), principal, true);

				data.EffectiveVersionSequenceId = entity.VersionSequence;
			}

			return base.InsertInternal(context, data, principal);
		}
	}
}
