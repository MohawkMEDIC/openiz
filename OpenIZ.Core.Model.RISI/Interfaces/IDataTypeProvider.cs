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
 * Date: 2017-1-5
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenIZ.Core.Model.RISI.Interfaces
{
	/// <summary>
	/// Represents a data type provider.
	/// </summary>
	public interface IDataTypeProvider
	{
		/// <summary>
		/// Gets the value for a given data type.
		/// </summary>
		/// <typeparam name="T">The type of data which is contained in the data type.</typeparam>
		/// <param name="id">The id of the data type.</param>
		/// <returns>Returns the value.</returns>
		T GetValue<T>(Guid id);

		/// <summary>
		/// Gets a list of values for a given data type.
		/// </summary>
		/// <typeparam name="T">The type of data which is contained in the data type.</typeparam>
		/// <param name="id">The id of the data type.</param>
		/// <returns>Returns a list of values.</returns>
		IEnumerable<T> GetValues<T>(Guid id);
	}
}