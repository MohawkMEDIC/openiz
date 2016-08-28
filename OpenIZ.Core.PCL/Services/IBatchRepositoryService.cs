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
 * Date: 2016-8-22
 */

using OpenIZ.Core.Model.Collection;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a simple repository service for storing batches (bundles)
	/// </summary>
	public interface IBatchRepositoryService
	{
		/// <summary>
		/// Inserts all the data in the provided bundle in one transaction
		/// </summary>
		Bundle Insert(Bundle data);

		/// <summary>
		/// Obsoletes all data in the provided bundle in one transaction
		/// </summary>
		Bundle Obsolete(Bundle obsolete);

		/// <summary>
		/// Updates all the data in the provided bundle in one transaction
		/// </summary>
		Bundle Update(Bundle data);

		/// <summary>
		/// Validate & prepare bundle for insert
		/// </summary>
		Bundle Validate(Bundle bundle);
	}
}