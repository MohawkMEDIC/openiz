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
 * Date: 2016-8-15
 */

using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents an abstract way for PCL classes to interact with persistence events that occur on the back end
	/// </summary>
	public interface IStockManagementService
	{
		/// <summary>
		/// Performs a stock adjustment for the specified facility and material
		/// </summary>
		Act Adjust(ManufacturedMaterial material, Place place, int quantity, Concept reason);

		/// <summary>
		/// Gets the balance for the material
		/// </summary>
		int GetBalance(Place place, ManufacturedMaterial material);
	}
}