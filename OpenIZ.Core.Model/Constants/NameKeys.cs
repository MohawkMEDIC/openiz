/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-4-19
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{

    /// <summary>
    /// Name use keys
    /// </summary>
    public static class NameUseKeys
    {
        public static readonly Guid License = Guid.Parse("48075D19-7B29-4CA5-9C73-0CBD31248446");
        public static readonly Guid Alphabetic = Guid.Parse("71D1C07C-6EE6-4240-8A95-19F96583512E");
        public static readonly Guid Religious = Guid.Parse("15207687-5290-4672-A7DF-2880A23DCBB5");
        public static readonly Guid Artist = Guid.Parse("4A7BF199-F33B-42F9-8B99-32433EA67BD7");
        public static readonly Guid Phonetic = Guid.Parse("2B085D38-3308-4664-9F89-48D8EF4DABA7");
        public static readonly Guid Indigenous = Guid.Parse("A3FB2A05-5EBE-47AE-AFD0-4C1B22336090");
        public static readonly Guid Soundex = Guid.Parse("E5794E3B-3025-436F-9417-5886FEEAD55A");
        public static readonly Guid Assigned = Guid.Parse("A87A6D21-2CA6-4AEA-88F3-6135CCEB58D1");
        public static readonly Guid Search = Guid.Parse("87964BFF-E442-481D-9749-69B2A84A1FBE");
        public static readonly Guid Ideographic = Guid.Parse("09000479-4672-44F8-BB4A-72FB25F7356A");
        public static readonly Guid Pseudonym = Guid.Parse("C31564EF-CA8D-4528-85A8-88245FCEF344");
        public static readonly Guid MaidenName = Guid.Parse("0674C1C8-963A-4658-AFF9-8CDCD308FA68");
        public static readonly Guid Legal = Guid.Parse("EFFE122D-8D30-491D-805D-ADDCB4466C35");
        public static readonly Guid OfficialRecord = Guid.Parse("1EC9583A-B019-4BAA-B856-B99CAF368656");
        public static readonly Guid Syllabic = Guid.Parse("B4CA3BF0-A7FC-44F3-87D5-E126BEDA93FF");
        public static readonly Guid Anonymous = Guid.Parse("95E6843A-26FF-4046-B6F4-EB440D4B85F7");
    }

    /// <summary>
    /// Name component type keys
    /// </summary>
    public static class NameComponentKeys
    {
        public static readonly Guid Title = Guid.Parse("4386D92A-D81B-4033-B968-01E57E20D5E0");
        public static readonly Guid Family = Guid.Parse("29B98455-ED61-49F8-A161-2D73363E1DF0");
        public static readonly Guid Delimiter = Guid.Parse("4C6B9519-A493-44A9-80E6-32D85109B04B");
        public static readonly Guid Prefix = Guid.Parse("A787187B-6BE4-401E-8836-97FC000C5D16");
        public static readonly Guid Given = Guid.Parse("2F64BDE2-A696-4B0A-9690-B21EBD7E5092");
        public static readonly Guid Suffix = Guid.Parse("064523DF-BB03-4932-9323-CDF0CC9590BA");
    }
}
