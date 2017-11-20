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
 * Date: 2017-1-14
 */
using OpenIZ.OrmLite.Attributes;
using OpenIZ.Persistence.Data.ADO.Data.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data.Model.Entities
{
    /// <summary>
    /// User entity ORM
    /// </summary>
    [Table("usr_ent_tbl")]
    public class DbUserEntity : DbPersonSubTable
    {

        /// <summary>
        /// Gets or sets the security user which is associated with this entity
        /// </summary>
        [Column("sec_usr_id"), ForeignKey(typeof(DbSecurityUser), nameof(DbSecurityUser.Key))]
        public Guid SecurityUserKey { get; set; }

    }
}
