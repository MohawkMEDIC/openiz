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
 * Date: 2017-7-6
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OizDevTool
{
	/// <summary>
	/// Represents a CSD import utility.
	/// </summary>
	public partial class CsdImport
	{
		/// <summary>
		/// Maps the services.
		/// </summary>
		/// <param name="csdServices">The CSD services.</param>
		/// <returns>Returns a list of place services.</returns>
		private static IEnumerable<PlaceService> MapServices(IEnumerable<service> csdServices)
		{
			// TODO: implement
			return new List<PlaceService>();

			var placeServiceService = ApplicationContext.Current.GetService<IDataPersistenceService<PlaceService>>();
			var services = new List<PlaceService>();

			foreach (var csdService in csdServices)
			{
				var service = new PlaceService();

				Guid key;

				//service.Key = !TryMapKey(csdService.entityID, out key) ? Guid.NewGuid() : key;

				// map service concept
				if (csdService.codedType?.Any() == true)
				{
					// we don't support multiple service types for a place service at the moment, so we only take the first one
					service.ServiceConceptKey = MapCodedType(csdService.codedType[0].code, csdService.codedType[0].codingScheme)?.Key;
				}

				services.Add(service);
			}

			return services;
		}
	}
}