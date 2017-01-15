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
using OpenIZ.Persistence.Data.ADO.Data.Attributes;
using System;



namespace OpenIZ.Persistence.Data.ADO.Data.Model.Security
{
	/// <summary>
	/// Security application data. Should only be one entry here as well
	/// </summary>
	[Table("sec_app_tbl")]
	public class DbSecurityApplication : DbBaseData
	{

        /// <summary>
        /// Gets or sets the application id
        /// </summary>
        [Column("app_id"), PrimaryKey]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the public identifier.
        /// </summary>
        /// <value>The public identifier.</value>
        [Column("app_pub_id")]
		public String PublicId {
			get;
			set;
		}

        /// <summary>
        /// Application authentication secret
        /// </summary>
        [Column("app_scrt")]
        public String Secret { get; set; }

        /// <summary>
        /// Replaces application identifier
        /// </summary>
        [Column("rplc_app_id")]
        public Guid? ReplacesApplicationKey { get; set; }
    }
}

