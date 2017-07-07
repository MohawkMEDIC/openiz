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
using System;



namespace OpenIZ.Persistence.Data.ADO.Data.Model.Entities
{
	/// <summary>
	/// Represents a place in the local database
	/// </summary>
	[Table("plc_tbl")]
	public class DbPlace : DbEntitySubTable
    {
        /// <summary>
        /// Identifies whether the place is mobile
        /// </summary>
        [Column("mob_ind")]
        public bool IsMobile { get; set; }

        /// <summary>
        /// Identifies the known latitude of the place
        /// </summary>
        [Column("lat")]
        public float Lat { get; set; }

        /// <summary>
        /// Identifies the known longitude of the place
        /// </summary>
        [Column("lng")]
        public float Lng { get; set; }

    }
}

