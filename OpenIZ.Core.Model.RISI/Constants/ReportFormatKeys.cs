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
 * Date: 2017-1-16
 */

using System;

namespace OpenIZ.Core.Model.RISI.Constants
{
	/// <summary>
	/// Represents a collection of report format keys.
	/// </summary>
	public static class ReportFormatKeys
	{
		/// <summary>
		/// Represents a CSV report format.
		/// </summary>
		public static readonly Guid Csv = Guid.Parse("BC0CEDFE-5F35-4A63-84B4-8C1998F130EA");

		/// <summary>
		/// Represents a DOCX report format.
		/// </summary>
		public static readonly Guid Docx = Guid.Parse("2240B6D1-C29D-4F9F-86A8-B1916294841B");

		/// <summary>
		/// Represents an HTML report format.
		/// </summary>
		public static readonly Guid Html = Guid.Parse("0FBB2A5E-3786-431B-9424-04DB6ADFED1B");

		/// <summary>
		/// Represents a JPrint report format.
		/// </summary>
		public static readonly Guid JPrint = Guid.Parse("30D73CF0-09D9-4527-B43D-3949093A098F");

		/// <summary>
		/// Represents an ODS report format.
		/// </summary>
		public static readonly Guid Ods = Guid.Parse("AF4F9771-2022-4C9C-8FB4-8BD7C1206829");

		/// <summary>
		/// Represents an ODT report format.
		/// </summary>
		public static readonly Guid Odt = Guid.Parse("E8E4E72E-842E-43A2-9FC8-5C9F5B80FCD3");

		/// <summary>
		/// Represents a PDF report format.
		/// </summary>
		public static readonly Guid Pdf = Guid.Parse("C6E350F6-E273-4815-AF46-300B38C2E77E");

		/// <summary>
		/// Represents an RTF report format.
		/// </summary>
		public static readonly Guid Rtf = Guid.Parse("36E0A49A-B408-4A99-903B-5D93F20597ED");

		/// <summary>
		/// Represents an XLS report format.
		/// </summary>
		public static readonly Guid Xls = Guid.Parse("90996710-8D38-4E5B-8A78-7561C5EC885E");

		/// <summary>
		/// Represents an XLSX report format.
		/// </summary>
		public static readonly Guid Xlsx = Guid.Parse("F51F86A5-634D-4671-B93F-0690A4799F52");

		/// <summary>
		/// Represents an XML report format.
		/// </summary>
		public static readonly Guid Xml = Guid.Parse("7871DAFE-64C7-452B-8C09-E3DBDCFE7AF3");
	}
}