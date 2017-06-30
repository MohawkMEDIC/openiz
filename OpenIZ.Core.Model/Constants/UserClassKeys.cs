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
 * Date: 2016-8-2
 */
using System;

namespace OpenIZ.Core.Model.Constants
{
	/// <summary>
	/// Represents user classification keys
	/// </summary>
	public static class UserClassKeys
	{
		/// <summary>
		/// Represents a user which is an application
		/// </summary>
		public static readonly Guid ApplicationUser = Guid.Parse("E9CD4DAD-2759-4022-AB07-92FCFB236A98");

		/// <summary>
		/// Represents a user which is a human
		/// </summary>
		public static readonly Guid HumanUser = Guid.Parse("33932B42-6F4B-4659-8849-6ACA54139D8E");

		/// <summary>
		/// Represents a user which is a system user
		/// </summary>
		public static readonly Guid SystemUser = Guid.Parse("9F71BB34-9691-440F-8249-9C831EA16D58");
	}
}