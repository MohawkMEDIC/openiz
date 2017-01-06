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
using System.Collections.Generic;
using OpenIZ.Core.Model.RISI.Interfaces;

namespace OpenIZ.Core.Model.RISI.Impl
{
	/// <summary>
	/// Represents a default data type provider.
	/// </summary>
	internal sealed class DefaultDataTypeProvider : IDataTypeProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultDataTypeProvider"/> class.
		/// </summary>
		public DefaultDataTypeProvider()
		{
		}

		/// <summary>
		/// Gets the value for a given data type.
		/// </summary>
		/// <typeparam name="T">The type of data which is contained in the data type.</typeparam>
		/// <param name="id">The id of the data type.</param>
		/// <returns>Returns the value.</returns>
		public T GetValue<T>(Guid id)
		{
			return default(T);
		}

		/// <summary>
		/// Gets a list of values for a given data type.
		/// </summary>
		/// <typeparam name="T">The type of data which is contained in the data type.</typeparam>
		/// <param name="id">The id of the data type.</param>
		/// <returns>Returns a list of values.</returns>
		public IEnumerable<T> GetValues<T>(Guid id)
		{
			return default(IEnumerable<T>);
		}
	}
}