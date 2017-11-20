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
 * Date: 2017-4-4
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
	/// <summary>
	/// Represents a collection of container cap keys.
	/// </summary>
	public static class ContainerCapKeys
	{
		/// <summary>
		/// Represents a child proof cap.
		/// </summary>
		public static readonly Guid ChildProof = Guid.Parse("5A209555-FB3A-437D-B043-0190685A4284");

		/// <summary>
		/// Represents an easy open cap.
		/// </summary>
		public static readonly Guid EasyOpen = Guid.Parse("1E630924-CE10-4A8A-A91D-E319F517A6EC");

		/// <summary>
		/// Represents a film cap.
		/// </summary>
		public static readonly Guid Film = Guid.Parse("266A20FE-DFFB-4C8D-9EAD-FDC7CD86C552");

		/// <summary>
		/// Represents a foil cap.
		/// </summary>
		public static readonly Guid Foil = Guid.Parse("EE2B76DB-85E9-412F-90BB-47A94F8E4C30");

		/// <summary>
		/// Represents a medication cap.
		/// </summary>
		public static readonly Guid MedicationCap = Guid.Parse("6470A9BE-FCB4-463D-BB0C-894D0CDDD4C2");

		/// <summary>
		/// Represents a push cap.
		/// </summary>
		public static readonly Guid PushCap = Guid.Parse("47C40DF6-3F69-47CC-A84C-33037E4BFFD9");

		/// <summary>
		/// Represents a screw cap.
		/// </summary>
		public static readonly Guid ScrewCap = Guid.Parse("05695B7C-6BF7-4AF7-A0C9-A5C8B2E63E05");
	}
}