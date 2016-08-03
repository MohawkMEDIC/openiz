/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Address use keys
    /// </summary>
    public static class AddressUseKeys
    {
        public static readonly Guid Direct = Guid.Parse("D0DB6EDB-6CDC-4671-8BC2-00F1C808E188");
        public static readonly Guid Ideographic = Guid.Parse("09000479-4672-44F8-BB4A-72FB25F7356A");
        public static readonly Guid WorkPlace = Guid.Parse("EAA6F08E-BB8E-4457-9DC0-3A1555FADF5C");
        public static readonly Guid PostalAddress = Guid.Parse("7246E98D-20C6-4AE6-85AD-4AA09649FEB7");
        public static readonly Guid VacationHome = Guid.Parse("5D69534C-4597-4D11-BB98-56A9918F5238");
        public static readonly Guid Alphabetic = Guid.Parse("71D1C07C-6EE6-4240-8A95-19F96583512E");
        public static readonly Guid Soundex = Guid.Parse("E5794E3B-3025-436F-9417-5886FEEAD55A");
        public static readonly Guid PhysicalVisit = Guid.Parse("5724A9B6-24B6-43B7-8075-7A0D61FCB814");
        public static readonly Guid BadAddress = Guid.Parse("F3132FC0-AADD-40B7-B875-961C40695389");
        public static readonly Guid TemporaryAddress = Guid.Parse("CEF6EA31-A097-4F59-8723-A38C727C6597");
        public static readonly Guid Syllabic = Guid.Parse("B4CA3BF0-A7FC-44F3-87D5-E126BEDA93FF");
        public static readonly Guid Phonetic = Guid.Parse("2B085D38-3308-4664-9F89-48D8EF4DABA7");
        public static readonly Guid HomeAddress = Guid.Parse("493C3E9D-4F65-4E4D-9582-C9008F4F2EB4");
        public static readonly Guid PrimaryHome = Guid.Parse("C4FAAFD8-FC90-4330-8B4B-E4E64C86B87B");
        public static readonly Guid Public = Guid.Parse("EC35EA7C-55D2-4619-A56B-F7A986412F7F");
    }

    /// <summary>
    /// Represents address component types
    /// </summary>
    public static class AddressComponentKeys
    {
        public static readonly Guid BuildingNumberSuffix = Guid.Parse("B2DBF05C-584D-46DB-8CBF-026A6EA30D81");
        public static readonly Guid PostBox = Guid.Parse("2047F216-F41E-4CFB-A024-05D4D3DE52F5");
        public static readonly Guid UnitIdentifier = Guid.Parse("908C09DF-81FE-45AC-9233-0881A278A401");
        public static readonly Guid AddressLine = Guid.Parse("4F342D28-8850-4DAF-8BCA-0B44A255F7ED");
        public static readonly Guid DeliveryAddressLine = Guid.Parse("F6139B21-3A36-4A3F-B498-0C661F06DF59");
        public static readonly Guid Precinct = Guid.Parse("ACAFE0F2-E209-43BB-8633-3665FD7C90BA");
        public static readonly Guid CensusTract = Guid.Parse("4B3A347C-28FA-4560-A1A9-3795C9DB3D3B");
        public static readonly Guid DeliveryModeIdentifier = Guid.Parse("08BD6027-47EB-43DE-8454-59B7A5D00A3E");
        public static readonly Guid DeliveryInstallationArea = Guid.Parse("EC9D5AB8-3BE1-448F-9346-6A08253F9DEA");
        public static readonly Guid DeliveryMode = Guid.Parse("12608636-910D-4BAC-B849-7F999DE20332");
        public static readonly Guid BuildingNumber = Guid.Parse("F3C86E99-8AFC-4947-9DD8-86412A34B1C7");
        public static readonly Guid Delimiter = Guid.Parse("4C6B9519-A493-44A9-80E6-32D85109B04B");
        public static readonly Guid County = Guid.Parse("D9489D56-DDAC-4596-B5C6-8F41D73D8DC5");
        public static readonly Guid PostalCode = Guid.Parse("78A47122-F9BF-450F-A93F-90A103C5F1E8");
        public static readonly Guid CareOf = Guid.Parse("8C89A89E-08C5-4374-87F9-ADB3C9261DF6");
        public static readonly Guid StreetName = Guid.Parse("0432D671-ABC3-4249-872C-AFD5274C2298");
        public static readonly Guid StreetType = Guid.Parse("121953F6-0465-41DE-8F7A-B0E08204C771");
        public static readonly Guid StreetAddressLine = Guid.Parse("F69DCFA8-DF18-403B-9217-C59680BAD99E");
        public static readonly Guid UnitDesignator = Guid.Parse("B18E71CB-203C-4640-83F0-CC86DEBBBBC0");
        public static readonly Guid Country = Guid.Parse("48B2FFB3-07DB-47BA-AD73-FC8FB8502471");
        public static readonly Guid StreetNameBase = Guid.Parse("37C7DBC8-4AC6-464A-AF65-D65FCBA60238");
        public static readonly Guid Direction = Guid.Parse("1F678716-AB8F-4856-9F76-D82FE3165C22");
        public static readonly Guid City = Guid.Parse("05B85461-578B-4988-BCA6-E3E94BE9DB76");
        public static readonly Guid State = Guid.Parse("8CF4B0B0-84E5-4122-85FE-6AFA8240C218");
        public static readonly Guid DeliveryInstallationType = Guid.Parse("684FB800-145C-47C5-98C5-E7AA53802B69");
        public static readonly Guid AdditionalLocator = Guid.Parse("D2312B8E-BDFB-4012-9397-F14336F8D206");
        public static readonly Guid DeliveryInstallationQualifier = Guid.Parse("78FB6EED-6549-4F22-AB3E-F3696DA050BC");
        public static readonly Guid BuildingNumberNumeric = Guid.Parse("3258B4D6-E4DC-43E6-9F29-FD8423A2AE12");
    }
}
