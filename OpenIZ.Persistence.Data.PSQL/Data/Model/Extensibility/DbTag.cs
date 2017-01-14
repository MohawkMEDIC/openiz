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
using PetaPoco;
using System;



namespace OpenIZ.Persistence.Data.PSQL.Data.Model.Extensibility
{
	/// <summary>
	/// Represents a simpe tag (version independent)
	/// </summary>
	public abstract class DbTag : DbAssociation, IDbBaseData
	{
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [Column("tag_id")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Created by 
        /// </summary>
        [Column("crt_usr_id")]
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        [Column("crt_utc")]
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Obsoleted by 
        /// </summary>
        [Column("obslt_usr_id")]
        public Guid? ObsoletedBy { get; set; }

        /// <summary>
        /// Gets or sets the obsoletion time
        /// </summary>
        [Column("obslt_utc")]
        public DateTimeOffset? ObsoletionTime { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [Column("tag_name")]
		public String TagKey {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[Column("tag_value")]
		public String Value {
			get;
			set;
		}
	}

	/// <summary>
	/// Represents a tag associated with an enttiy
	/// </summary>
	[TableName("ent_tag_tbl")]
	public class DbEntityTag : DbTag
	{

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [Column("ent_id")]
        public override Guid SourceKey
        {
            get;
            set;
        }
    }

	/// <summary>
	/// Represents a tag associated with an act
	/// </summary>
	[TableName("act_tag_tbl")]
	public class DbActTag : DbTag
	{
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [Column("act_id")]
        public override Guid SourceKey
        {
            get;
            set;
        }
    }

}

