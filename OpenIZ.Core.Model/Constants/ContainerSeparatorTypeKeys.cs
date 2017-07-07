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
	/// Represents a collection of container separator type keys.
	/// A material in a blood collection container that facilitates the separation of of blood cells from serum or plasma
	/// </summary>
	public static class ContainerSeparatorTypeKeys
	{
		/// <summary>
		/// Represents a gelatinous type of separator material.
		/// </summary>
		public static readonly Guid Gel = Guid.Parse("EE450FF6-9BED-4C47-90D2-671AB3041756");

		/// <summary>
		/// Represents no separator material is present in the container.
		/// </summary>
		public static readonly Guid None = Guid.Parse("472524EB-C8D4-49F8-862A-EF4BD7CA0395");
	}
}
