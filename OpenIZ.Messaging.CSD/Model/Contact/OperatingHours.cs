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
 * User: Nityan
 * Date: 2017-4-2
 */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.CSD.Model.Contact
{
	/// <summary>
	/// Represents operating hours.
	/// </summary>
	[XmlType("operatingHours", Namespace = "urn:ihe:iti:csd:2013")]
	public class OperatingHours
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OperatingHours"/> class.
		/// </summary>
		public OperatingHours()
		{
			
		}

		/// <summary>
		/// Gets or sets a value indicating whether [open flag].
		/// </summary>
		/// <value><c>true</c> if [open flag]; otherwise, <c>false</c>.</value>
		[XmlElement("openFlag")]
		public bool OpenFlag { get; set; }

		/// <summary>
		/// Gets or sets the day of week.
		/// </summary>
		/// <value>The day of week.</value>
		[XmlElement("dayOfTheWeek", DataType = "integer")]
		public List<string> DayOfWeek { get; set; }

		/// <summary>
		/// Gets or sets the beginning hour.
		/// </summary>
		/// <value>The beginning hour.</value>
		[XmlElement("beginningHour", DataType = "time")]
		public DateTime BeginningHour { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [beginning hour specified].
		/// </summary>
		/// <value><c>true</c> if [beginning hour specified]; otherwise, <c>false</c>.</value>
		[XmlIgnore]
		public bool BeginningHourSpecified { get; set; }

		/// <summary>
		/// Gets or sets the ending hour.
		/// </summary>
		/// <value>The ending hour.</value>
		[XmlElement("endingHour", DataType = "time")]
		public DateTime EndingHour { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [ending hour specified].
		/// </summary>
		/// <value><c>true</c> if [ending hour specified]; otherwise, <c>false</c>.</value>
		[XmlIgnore]
		public bool EndingHourSpecified { get; set; }

		/// <summary>
		/// Gets or sets the begin effective date.
		/// </summary>
		/// <value>The begin effective date.</value>
		[XmlElement("beginEffectiveDate", DataType = "date")]
		public DateTime BeginEffectiveDate { get; set; }

		/// <summary>
		/// Gets or sets the end effective date.
		/// </summary>
		/// <value>The end effective date.</value>
		[XmlElement("endEffectiveDate", DataType = "date")]
		public DateTime EndEffectiveDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [end effective date specified].
		/// </summary>
		/// <value><c>true</c> if [end effective date specified]; otherwise, <c>false</c>.</value>
		[XmlIgnore]
		public bool EndEffectiveDateSpecified { get; set; }
	}
}
