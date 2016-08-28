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
 * User: justi
 * Date: 2016-6-28
 */

using System;

namespace OpenIZ.Core.Model.Constants
{
	/// <summary>
	/// Telecommunications address use keys
	/// </summary>
	public static class TelecomAddressUseKeys
	{
		/// <summary>
		/// answering service
		/// </summary>
		public static readonly Guid AnsweringService = Guid.Parse("1ECD7B17-B5FF-4CAE-9C3B-C1258132D137");

		/// <summary>
		/// Emergency contact
		/// </summary>
		public static readonly Guid EmergencyContact = Guid.Parse("25985F42-476A-4455-A977-4E97A554D710");

		/// <summary>
		/// Mobile phone contact
		/// </summary>
		public static readonly Guid MobileContact = Guid.Parse("E161F90E-5939-430E-861A-F8E885CC353D");

		/// <summary>
		/// pager
		/// </summary>
		public static readonly Guid Pager = Guid.Parse("788000B4-E37A-4055-A2AA-C650089CE3B1");

		/// <summary>
		/// public (800 number example) contact
		/// </summary>
		public static readonly Guid Public = Guid.Parse("EC35EA7C-55D2-4619-A56B-F7A986412F7F");

		/// <summary>
		/// temporary contact
		/// </summary>
		public static readonly Guid TemporaryAddress = Guid.Parse("CEF6EA31-A097-4F59-8723-A38C727C6597");

		/// <summary>
		/// For use in the workplace
		/// </summary>
		public static readonly Guid WorkPlace = Guid.Parse("EAA6F08E-BB8E-4457-9DC0-3A1555FADF5C");
	}
}