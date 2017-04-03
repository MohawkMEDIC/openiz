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
 * Date: 2017-1-15
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.RISI.Interfaces;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenIZ.Reporting.Jasper.Provider
{
	/// <summary>
	/// Represents a material value provider.
	/// </summary>
	public class MaterialValueProvider : IParameterValuesProvider
	{
		/// <summary>
		/// Gets a list of values.
		/// </summary>
		/// <typeparam name="T">The type of parameter for which to retrieve values.</typeparam>
		/// <returns>Returns a list of values.</returns>
		public IEnumerable<T> GetValues<T>() where T : IdentifiedData
		{
			return new List<Material>().Cast<T>();
			var results = new List<Material>();

			var materialPersistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();

			if (materialPersistenceService == null)
			{
				throw new InvalidOperationException($"Unable to locate { nameof(IDataPersistenceService<Material>) }");
			}

			var materials = materialPersistenceService.Query(m => m.ObsoletionTime == null && m.ClassConceptKey == EntityClassKeys.Material, AuthenticationContext.Current.Principal);

			results.AddRange(materials);

			return results.Cast<T>();
		}
	}
}