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
	/// Reference type identifiers
	/// </summary>
	public static class ConceptRelationshipTypeKeys
	{
		/// <summary>
		/// The source concept has the inverse meaning of the target concept
		/// </summary>
		public static readonly Guid InverseOf = Guid.Parse("ad27293d-433c-4b75-88d2-b5360cd95450");

		/// <summary>
		/// The source concept is a member of the target concept
		/// </summary>
		public static readonly Guid MemberOf = Guid.Parse("a159d45b-3c34-4e1b-9b75-9193a7528ced");

		/// <summary>
		/// The source concept is a negation of the target concept
		/// </summary>
		public static readonly Guid NegationOf = Guid.Parse("ae8b4f2f-009f-4e0d-b35e-5a89555c5947");

		/// <summary>
		/// The source concept has the same meaning as the target concept
		/// </summary>
		public static readonly Guid SameAs = Guid.Parse("2c4dafc2-566a-41ae-9ebc-3097d7d22f4a");
	}
}