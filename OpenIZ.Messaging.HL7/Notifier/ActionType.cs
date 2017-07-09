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
 * Date: 2016-11-30
 */
namespace OpenIZ.Messaging.HL7.Notifier
{
	/// <summary>
	/// Represents a action type.
	/// </summary>
	public enum ActionType
	{
		/// <summary>
		/// Any action occurs. This is only used.
		/// </summary>
		Any = Create | Update | DuplicatesResolved,

		/// <summary>
		/// Indicates a creation action.
		/// </summary>
		Create = 0x1,

		/// <summary>
		/// Indicates a duplicates resolved action.
		/// </summary>
		DuplicatesResolved = 0x2,

		/// <summary>
		/// Indicates a reconciliation required action.
		/// </summary>
		ReconciliationRequired = 0x4,

		/// <summary>
		/// Indicates an update action.
		/// </summary>
		Update = 0x8
	}
}