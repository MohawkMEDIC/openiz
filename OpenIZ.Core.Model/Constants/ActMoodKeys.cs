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
	/// Act Mood keys
	/// </summary>
	public static class ActMoodKeys
	{
		/// <summary>
		/// The ACT represents an appointment that was made to do something
		/// </summary>
		public static readonly Guid Appointment = Guid.Parse("C46EEE70-5612-473F-8D24-595EA15C9C39");

		/// <summary>
		/// The ACT represents a special type of request to create an appointment
		/// </summary>
		public static readonly Guid AppointmentRequest = Guid.Parse("0395F357-6821-4562-8192-49AC3C94F548");

		/// <summary>
		/// The ACT represents a definition of a type of act
		/// </summary>
		public static readonly Guid Definition = Guid.Parse("3B14A426-6337-4F2A-B83B-E6BE7DBCD5A5");

		/// <summary>
		/// The ACT represents something that has occurred
		/// </summary>
		public static readonly Guid Eventoccurrence = Guid.Parse("EC74541F-87C4-4327-A4B9-97F325501747");

		/// <summary>
		/// The ACT represents some sort of GOAL
		/// </summary>
		public static readonly Guid Goal = Guid.Parse("13925967-E748-4DD6-B562-1E1DA3DDFB06");

		/// <summary>
		/// The ACT represents an intent made by a human to do something
		/// </summary>
		public static readonly Guid Intent = Guid.Parse("099BCC5E-8E2F-4D50-B509-9F9D5BBEB58E");

		/// <summary>
		/// The ACT represents a promise to do something
		/// </summary>
		public static readonly Guid Promise = Guid.Parse("B389DEDF-BE61-456B-AA70-786E1A5A69E0");

		/// <summary>
		/// The ACT represents a proposal that a human should do something
		/// </summary>
		public static readonly Guid Propose = Guid.Parse("ACF7BAF2-221F-4BC2-8116-CEB5165BE079");

		/// <summary>
		/// The ACT represents a request to do something
		/// </summary>
		public static readonly Guid Request = Guid.Parse("E658CA72-3B6A-4099-AB6E-7CF6861A5B61");
	}
}