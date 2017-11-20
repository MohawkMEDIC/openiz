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
 * Date: 2017-1-16
 */

using System;

namespace OpenIZ.Core.Model.RISI.Constants
{
	/// <summary>
	/// Represents a collection of data type keys.
	/// </summary>
	public static class ParameterTypeKeys
	{
		/// <summary>
		/// Represents a boolean value.
		/// </summary>
		public static readonly Guid Boolean = Guid.Parse("8C86A76C-EB72-4213-81F7-D56D7D606C2E");

		/// <summary>
		/// Represents a date time value.
		/// </summary>
		public static readonly Guid DateTime = Guid.Parse("44DD3308-A503-49F9-87F7-57FACE59CF70");

		/// <summary>
		/// Represents an enum value.
		/// </summary>
		public static readonly Guid Enum = Guid.Parse("B71859BC-F473-40CF-95DF-0F7C9DE5FCF8");

		/// <summary>
		/// Represents a float value.
		/// </summary>
		public static readonly Guid Float = Guid.Parse("52BAE88A-5239-4F83-A2C9-958CE07B0EBE");

		/// <summary>
		/// Represents a GUID/UUID value.
		/// </summary>
		public static readonly Guid Guid = Guid.Parse("6CDE9F0D-1DA4-462F-8C41-163969D4E575");

		/// <summary>
		/// Represents an integer value.
		/// </summary>
		public static readonly Guid Integer = Guid.Parse("295BF77E-BEBC-4DE0-A18A-F141DB6D987B");

		/// <summary>
		/// Represents an object value.
		/// </summary>
		public static readonly Guid Object = Guid.Parse("795BE824-A0E0-4C0C-8853-56FDC5673BC1");

		/// <summary>
		/// Represents a string value.
		/// </summary>
		public static readonly Guid String = Guid.Parse("60FC0E02-701F-48BD-81CD-8DE51B113B31");

		/// <summary>
		/// Represents an unsigned integer value.
		/// </summary>
		public static readonly Guid UnsignedInt = Guid.Parse("692C4A26-6010-4E62-9EAF-51F323517DA0");
	}
}