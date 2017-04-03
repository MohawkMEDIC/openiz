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

using System.Xml.Serialization;

namespace OpenIZ.Messaging.CSD.Model.Location
{
	/// <summary>
	/// Represents a GeoCode location.
	/// </summary>
	[XmlType("geocode", Namespace = "urn:ihe:iti:csd:2013")]
	public class GeoCode
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GeoCode"/> class.
		/// </summary>
		public GeoCode()
		{
			
		}

		/// <summary>
		/// Gets or sets the latitude.
		/// </summary>
		/// <value>The latitude.</value>
		[XmlElement("latitude")]
		public decimal Latitude { get; set; }

		/// <summary>
		/// Gets or sets the longitude.
		/// </summary>
		/// <value>The longitude.</value>
		[XmlElement("longitude")]
		public decimal Longitude { get; set; }

		/// <summary>
		/// Gets or sets the altitude.
		/// </summary>
		/// <value>The altitude.</value>
		[XmlElement("altitude")]
		public decimal Altitude { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [altitude specified].
		/// </summary>
		/// <value><c>true</c> if [altitude specified]; otherwise, <c>false</c>.</value>
		[XmlIgnore]
		public bool AltitudeSpecified { get; set; }

		/// <summary>
		/// Gets or sets the coordinate system.
		/// </summary>
		/// <value>The coordinate system.</value>
		[XmlElement("coordinateSystem")]
		public string CoordinateSystem { get; set; }
	}
}
