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
 * Date: 2016-11-30
 */
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Patch;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a patch service which can calculate and apply patches
	/// </summary>
	public interface IPatchService
	{
		/// <summary>
		/// Performs a DIFF and creates the related patch which can be used to update <paramref name="existing"/>
		/// to <paramref name="updated"/>
		/// </summary>
		Patch Diff(IdentifiedData existing, IdentifiedData updated, params string[] ignoreProperties);

		/// <summary>
		/// Apples the specified <paramref name="patch"/> onto <paramref name="data"/> returning the updated object
		/// </summary>
		IdentifiedData Patch(Patch patch, IdentifiedData data, bool force = false);

		/// <summary>
		/// Tests that the patch can be applied on the specified object
		/// </summary>
		bool Test(Patch patch, IdentifiedData target);
	}
}