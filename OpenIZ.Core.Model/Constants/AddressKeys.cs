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
	/// Represents address component types
	/// </summary>
	public static class AddressComponentKeys
	{
		/// <summary>
		/// An additional locator (example: Beside the red barn).
		/// </summary>
		public static readonly Guid AdditionalLocator = Guid.Parse("D2312B8E-BDFB-4012-9397-F14336F8D206");
		/// <summary>
		/// An address line as would appear on an address (example: 123 Main Street West)
		/// </summary>
		public static readonly Guid AddressLine = Guid.Parse("4F342D28-8850-4DAF-8BCA-0B44A255F7ED");
		/// <summary>
		/// Identifies a particular building on a street (example: A23 Building)
		/// </summary>
		public static readonly Guid BuildingNumber = Guid.Parse("F3C86E99-8AFC-4947-9DD8-86412A34B1C7");
		/// <summary>
		/// Identifies a numeric identifier for a building (example: 123)
		/// </summary>
		public static readonly Guid BuildingNumberNumeric = Guid.Parse("3258B4D6-E4DC-43E6-9F29-FD8423A2AE12");
		/// <summary>
		/// Identifies a suffix to the building number (example: 123 *SECTOR 7*)
		/// </summary>
		public static readonly Guid BuildingNumberSuffix = Guid.Parse("B2DBF05C-584D-46DB-8CBF-026A6EA30D81");
		/// <summary>
		/// Identifies the person where deliveries should be care-of (example: c/o Bob Smith)
		/// </summary>
		public static readonly Guid CareOf = Guid.Parse("8C89A89E-08C5-4374-87F9-ADB3C9261DF6");
		/// <summary>
		/// The census tract which is used for political counting of the census
		/// </summary>
		public static readonly Guid CensusTract = Guid.Parse("4B3A347C-28FA-4560-A1A9-3795C9DB3D3B");
		/// <summary>
		/// The town or city (example: Toronto)
		/// </summary>
		public static readonly Guid City = Guid.Parse("05B85461-578B-4988-BCA6-E3E94BE9DB76");
		/// <summary>
		/// The country in which the address resides (example: Canada)
		/// </summary>
		public static readonly Guid Country = Guid.Parse("48B2FFB3-07DB-47BA-AD73-FC8FB8502471");
		/// <summary>
		/// The county or sub-division of a sub-national unit (example: Clark County)
		/// </summary>
		public static readonly Guid County = Guid.Parse("D9489D56-DDAC-4596-B5C6-8F41D73D8DC5");
		/// <summary>
		/// Represents a meaningless delimiter such as dash, or newline
		/// </summary>
		public static readonly Guid Delimiter = Guid.Parse("4C6B9519-A493-44A9-80E6-32D85109B04B");
		/// <summary>
		/// Represents an address line to be used for delivery rather than physical location (example: Loading Dock #4)
		/// </summary>
		public static readonly Guid DeliveryAddressLine = Guid.Parse("F6139B21-3A36-4A3F-B498-0C661F06DF59");
		/// <summary>
		/// Represents the area where the delivery should take place
		/// </summary>
		public static readonly Guid DeliveryInstallationArea = Guid.Parse("EC9D5AB8-3BE1-448F-9346-6A08253F9DEA");
		/// <summary>
		/// The delivery installation qualifier.
		/// </summary>
		public static readonly Guid DeliveryInstallationQualifier = Guid.Parse("78FB6EED-6549-4F22-AB3E-F3696DA050BC");
		/// <summary>
		/// The delivery installation type.
		/// </summary>
		public static readonly Guid DeliveryInstallationType = Guid.Parse("684FB800-145C-47C5-98C5-E7AA53802B69");
		/// <summary>
		/// The delivery mode.
		/// </summary>
		public static readonly Guid DeliveryMode = Guid.Parse("12608636-910D-4BAC-B849-7F999DE20332");
		/// <summary>
		/// The delivery mode identifier.
		/// </summary>
		public static readonly Guid DeliveryModeIdentifier = Guid.Parse("08BD6027-47EB-43DE-8454-59B7A5D00A3E");
		/// <summary>
		/// Represents a directory such as north, south, east, or west
		/// </summary>
		public static readonly Guid Direction = Guid.Parse("1F678716-AB8F-4856-9F76-D82FE3165C22");
		/// <summary>
		/// A codified adminsitrative unit used to locate the address (zip code or postal code)
		/// </summary>
		public static readonly Guid PostalCode = Guid.Parse("78A47122-F9BF-450F-A93F-90A103C5F1E8");
		/// <summary>
		/// Represents a PO box where delivery of mail should take place
		/// </summary>
		public static readonly Guid PostBox = Guid.Parse("2047F216-F41E-4CFB-A024-05D4D3DE52F5");
		/// <summary>
		/// Represents a precinct or sub-division of a city such as a burrogh
		/// </summary>
		public static readonly Guid Precinct = Guid.Parse("ACAFE0F2-E209-43BB-8633-3665FD7C90BA");
		/// <summary>
		/// Represents a state or province, or a sub-division of a national boundary
		/// </summary>
		public static readonly Guid State = Guid.Parse("8CF4B0B0-84E5-4122-85FE-6AFA8240C218");
		/// <summary>
		/// Represents a physical street delivery line (example: 123 Main Street West)
		/// </summary>
		public static readonly Guid StreetAddressLine = Guid.Parse("F69DCFA8-DF18-403B-9217-C59680BAD99E");
		/// <summary>
		/// Represents the name portion of a street address (example: Main St.)
		/// </summary>
		public static readonly Guid StreetName = Guid.Parse("0432D671-ABC3-4249-872C-AFD5274C2298");
		/// <summary>
		/// The street name base portion of a street address (Example: Main)
		/// </summary>
		public static readonly Guid StreetNameBase = Guid.Parse("37C7DBC8-4AC6-464A-AF65-D65FCBA60238");
		/// <summary>
		/// The street type (example: Street, Road, Hwy)
		/// </summary>
		public static readonly Guid StreetType = Guid.Parse("121953F6-0465-41DE-8F7A-B0E08204C771");
		/// <summary>
		/// Identifies the type of unit (example: Suite, Apartment, Unit)
		/// </summary>
		public static readonly Guid UnitDesignator = Guid.Parse("B18E71CB-203C-4640-83F0-CC86DEBBBBC0");
		/// <summary>
		/// The identifier of the unit (example: 820)
		/// </summary>
		public static readonly Guid UnitIdentifier = Guid.Parse("908C09DF-81FE-45AC-9233-0881A278A401");
	}

	/// <summary>
	/// Address use keys
	/// </summary>
	public static class AddressUseKeys
	{
		/// <summary>
		/// Represents an alphabetic address used for matching 
		/// </summary>
		public static readonly Guid Alphabetic = Guid.Parse("71D1C07C-6EE6-4240-8A95-19F96583512E");

		/// <summary>
		/// Represents a bad address, i.e. an address which is old or invalid.
		/// </summary>
		public static readonly Guid BadAddress = Guid.Parse("F3132FC0-AADD-40B7-B875-961C40695389");

		/// <summary>
		/// Represents a workplace address that reaches the person directly without intermediaries.
		/// </summary>
		public static readonly Guid Direct = Guid.Parse("D0DB6EDB-6CDC-4671-8BC2-00F1C808E188");

		/// <summary>
		/// The home address
		/// </summary>
		public static readonly Guid HomeAddress = Guid.Parse("493C3E9D-4F65-4E4D-9582-C9008F4F2EB4");

		/// <summary>
		/// Represents an address expressed in an ideographic manner (example: Kanji)
		/// </summary>
		public static readonly Guid Ideographic = Guid.Parse("09000479-4672-44F8-BB4A-72FB25F7356A");

		/// <summary>
		/// Represents an address expressed as a phonetic spelling of an ideographic address
		/// </summary>
		public static readonly Guid Phonetic = Guid.Parse("2B085D38-3308-4664-9F89-48D8EF4DABA7");

		/// <summary>
		/// The address is a physical place where visits should occur
		/// </summary>
		public static readonly Guid PhysicalVisit = Guid.Parse("5724A9B6-24B6-43B7-8075-7A0D61FCB814");

		/// <summary>
		/// The address is a postal address used for the delivery of mail and materials
		/// </summary>
		public static readonly Guid PostalAddress = Guid.Parse("7246E98D-20C6-4AE6-85AD-4AA09649FEB7");

		/// <summary>
		/// Represents a primary address to reach a contact after business hours.
		/// </summary>
		public static readonly Guid PrimaryHome = Guid.Parse("C4FAAFD8-FC90-4330-8B4B-E4E64C86B87B");

		/// <summary>
		/// Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.
		/// </summary>
		public static readonly Guid Public = Guid.Parse("EC35EA7C-55D2-4619-A56B-F7A986412F7F");

		/// <summary>
		/// Represents an address used for soundex matching purposes.
		/// </summary>
		public static readonly Guid Soundex = Guid.Parse("E5794E3B-3025-436F-9417-5886FEEAD55A");

		/// <summary>
		/// Represents a syllabic address.
		/// </summary>
		public static readonly Guid Syllabic = Guid.Parse("B4CA3BF0-A7FC-44F3-87D5-E126BEDA93FF");

		/// <summary>
		/// Represents a temporary address that may be good for visiting or mailing.
		/// </summary>
		public static readonly Guid TemporaryAddress = Guid.Parse("CEF6EA31-A097-4F59-8723-A38C727C6597");

		/// <summary>
		/// Represents a vacation home to reach a person while on vacation.
		/// </summary>
		public static readonly Guid VacationHome = Guid.Parse("5D69534C-4597-4D11-BB98-56A9918F5238");

		/// <summary>
		/// Represents an office address, should be used for business communications
		/// </summary>
		public static readonly Guid WorkPlace = Guid.Parse("EAA6F08E-BB8E-4457-9DC0-3A1555FADF5C");
	}
}