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



namespace OpenIZ.Persistence.Data.PSQL.Data.Model.Extensibility
{
	/// <summary>
	/// Represents note storage
	/// </summary>
	public abstract class DbNote : IDbVersionedAssociation
	{

        /// <summary>
		/// Gets or sets the author identifier.
		/// </summary>
		/// <value>The author identifier.</value>
		[Column("author")]
		public Guid AuthorKey {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		[Column("text")]
		public String Text {
			get;
			set;
		}
	}

	/// <summary>
	/// Entity note.
	/// </summary>
	[TableName("entity_note")]
	public class DbEntityNote : DbNote
	{
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("ent_id")]
        public Guid EntityUuid
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Act note.
    /// </summary>
    [TableName("act_note")]
	public class DbActNote : DbNote
	{
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("act_id")]
        public Guid ActUuid
        {
            get;
            set;
        }

    }
}

