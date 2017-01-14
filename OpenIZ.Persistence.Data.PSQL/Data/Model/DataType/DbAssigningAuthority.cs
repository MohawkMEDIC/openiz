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

namespace OpenIZ.Persistence.Data.PSQL.Data.Model.DataType
{
    /// <summary>
    /// Represents an assigning authority
    /// </summary>
    [TableName("assigning_authority")]
    public class DbAssigningAuthority : DbBaseData
    {

        /// <summary>
        /// Gets or sets the name of the aa
        /// </summary>
        [Column("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the short HL7 code of the AA
        /// </summary>
        [Column("domainName"), Indexed, MaxLength(32)]
        public String DomainName { get; set; }

        /// <summary>
        /// Gets or sets the OID of the AA
        /// </summary>
        [Column("oid")]
        public String Oid { get; set; }

        /// <summary>
        /// Gets or sets the description of the AA
        /// </summary>
        [Column("description")]
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the URL of AA
        /// </summary>
        [Column("url")]
        public String Url { get; set; }

        /// <summary>
        /// Assigning device identifier
        /// </summary>
        [Column("assigningDevice")]
        public Guid AssigningDeviceKey { get; set; }

    }


    /// <summary>
    /// Identifier scope
    /// </summary>
    [TableName("assigning_authority_scope")]
    public class DbAuthorityScope : IDbVersionedAssociation
    {
        /// <summary>
        /// Gets or sets the scope of the auhority
        /// </summary>
        [Column("authority")]
        public Guid AssigningAuthorityKey { get; set; }

        /// <summary>
        /// Gets or sets the scope of the auhority
        /// </summary>
        [Column("concept")]
        public Guid ScopeConceptKey { get; set; }

    }
}
