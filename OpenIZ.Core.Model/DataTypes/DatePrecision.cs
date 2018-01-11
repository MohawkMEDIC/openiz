/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-16
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.DataTypes
{
	/// <summary>
	/// Represents a date precision object
	/// </summary>
	[XmlType("DatePrecision",  Namespace = "http://openiz.org/model")]
    public enum DatePrecision : int
    {
		/// <summary>
		/// Represents full date precision.
		/// </summary>
		[XmlEnum("F")]
        Full = 0,
		/// <summary>
		/// Represents year date precision.
		/// </summary>
        [XmlEnum("Y")]
        Year = 1,

		/// <summary>
		/// Represents month date precision.
		/// </summary>
		[XmlEnum("m")]
        Month = 2,

		/// <summary>
		/// Represents day date precision.
		/// </summary>
		[XmlEnum("D")]
        Day = 3,

		/// <summary>
		/// Represents hour date precision.
		/// </summary>
		[XmlEnum("H")]
        Hour = 4,

		/// <summary>
		/// Represents minute date precision.
		/// </summary>
		[XmlEnum("M")]
        Minute = 5,

		/// <summary>
		/// Represents second date precision.
		/// </summary>
		[XmlEnum("S")]
        Second = 6
    }
}
