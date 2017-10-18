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
 * Date: 2016-6-14
 */
using System;

namespace OpenIZ.Core.Model.Constants
{
	/// <summary>
	/// Status concepts represent the current status of entities, acts, and concepts.
	/// </summary>
	public static class StatusKeys
	{
		/// <summary>
		/// When an entity or act is active, it means the information or entity is currently correct and ongoing
		/// </summary>
		public static readonly Guid Active = Guid.Parse("C8064CBD-FA06-4530-B430-1A52F1530C27");

		/// <summary>
		/// Indicates that an act has been completed and now represents an act in the past
		/// </summary>
		public static readonly Guid Completed = Guid.Parse("AFC33800-8225-4061-B168-BACC09CDBAE3");

		/// <summary>
		/// Indicates that the data is new, and may require additional verification or actions
		/// </summary>
		public static readonly Guid New = Guid.Parse("C34FCBF1-E0FE-4989-90FD-0DC49E1B9685");

		/// <summary>
		/// Indicates that the entity or act never existed, and was entered in error
		/// </summary>
		public static readonly Guid Nullified = Guid.Parse("CD4AA3C4-02D5-4CC9-9088-EF8F31E321C5");

        /// <summary>
        /// Indicates that the act was cancelled before being completed
        /// </summary>
        public static readonly Guid Cancelled = Guid.Parse("3EFD3B6E-02D5-4CC9-9088-EF8F31E321C5");

		/// <summary>
		/// Indicates that the entity or act did exist at one point, however it no longer exists
		/// </summary>
		public static readonly Guid Obsolete = Guid.Parse("BDEF5F90-5497-4F26-956C-8F818CCE2BD2");
	}
}