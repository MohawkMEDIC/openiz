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
	/// Represents a collection of container type keys.
	/// </summary>
	public static class ContainerTypeKeys
	{
		/// <summary>
		/// Represents a container type of box.
		/// </summary>
		public static readonly Guid Boxes = Guid.Parse("6CFDA504-ED76-4BC9-B77B-40D4ED09E93D");

		/// <summary>
		/// Represents a container type of vial.
		/// </summary>
		public static readonly Guid Vials = Guid.Parse("907DDBFB-91B3-439E-8DF2-67925DCF4625");
	}
}
