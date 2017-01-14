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

namespace OpenIZ.Persistence.Data.PSQL.Data.Model.Entities
{
	/// <summary>
	/// Represents an entity in the database
	/// </summary>
	[TableName("ent_vrsn_tbl")]
	public class DbEntityVersion : DbVersionedData
	{
        /// <summary>
        /// Gets or sets the template
        /// </summary>
        [Column("template")]
        public Guid TemplateKey { get; set; }

        /// <summary>
        /// Gets or sets the class concept identifier.
        /// </summary>
        /// <value>The class concept identifier.</value>
        [Column("classConcept"), NotNull]
		public Guid ClassConceptKey {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the determiner concept identifier.
		/// </summary>
		/// <value>The determiner concept identifier.</value>
		[Column("determinerConcept")]
		public Guid DeterminerConceptKey {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the status concept identifier.
		/// </summary>
		/// <value>The status concept identifier.</value>
		[Column("statusConcept")]
		public Guid StatusConceptKey {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type concept identifier.
		/// </summary>
		/// <value>The type concept identifier.</value>
		[Column("typeConcept")]
		public Guid TypeConceptKey {
			get;
			set;
		}


	}
}

