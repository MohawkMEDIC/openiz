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
 * User: Nityan
 * Date: 2016-8-30
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents a stock management repository service.
	/// </summary>
	public class LocalStockManagementRepositoryService : IStockManagementRepositoryService
	{
		/// <summary>
		/// Performs a stock adjustment for the specified facility and material.
		/// </summary>
		/// <param name="manufacturedMaterial">The manufactured material to be adjusted.</param>
		/// <param name="place">The facility for which the stock is to be adjusted.</param>
		/// <param name="quantity">The quantity to be adjusted.</param>
		/// <param name="reason">The reason for the stock to be adjusted.</param>
		public Act Adjust(ManufacturedMaterial manufacturedMaterial, Place place, int quantity, Concept reason)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the balance for the material.
		/// </summary>
		/// <param name="place">The facility for which to get the balance of stock.</param>
		/// <param name="manufacturedMaterial">The manufactured material for which to retrieve the balance.</param>
		public int GetBalance(Place place, ManufacturedMaterial manufacturedMaterial)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Find adjustments
        /// </summary>
        public IEnumerable<Act> FindAdjustments(Guid manufacturedMaterialKey, Guid placeKey, DateTimeOffset? startPeriod, DateTimeOffset? endPeriod)
        {
            IDataPersistenceService<Act> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Act>>();
            if (persistenceService == null)
                throw new InvalidOperationException();

            return persistenceService.Query(o => o.ClassConceptKey == ActClassKeys.AccountManagement && o.ActTime >= startPeriod.Value && o.ActTime <= endPeriod.Value &&
                o.Participations.Where(guard=>guard.ParticipationRoleKey == ActParticipationKey.Location).Any(p=>p.PlayerEntityKey == placeKey) &&
                o.Participations.Where(guard=>guard.ParticipationRoleKey == ActParticipationKey.Consumable).Any(p=>p.PlayerEntityKey == manufacturedMaterialKey), AuthenticationContext.Current.Principal);

        }
    }
}