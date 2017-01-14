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



namespace OpenIZ.Persistence.Data.PSQL.Data.Model.Concepts
{
	/// <summary>
	/// Concept set
	/// </summary>
	[TableName("concept_set")]
	public class DbConceptSet : IDbVersionedAssociation
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Column("name")]
		public String Name {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the mnemonic.
		/// </summary>
		/// <value>The mnemonic.</value>
		[Column("mnemonic")]
		public String Mnemonic {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the oid of the concept set
        /// </summary>
        [Column("oid")]
        public String Oid { get; set; }

        /// <summary>
        /// Gets or sets the url of the concept set
        /// </summary>
        [Column("url")]
        public String Url { get; set; }

    }

	/// <summary>
	/// Concept set concept association.
	/// </summary>
	[TableName("concept_concept_set")]
	public class DbConceptSetConceptAssociation : IDbVersionedAssociation
	{

		/// <summary>
		/// Gets or sets the concept identifier.
		/// </summary>
		/// <value>The concept identifier.</value>
		[Column("concept_uuid"), NotNull, Indexed(Name = "concept_concept_set_concept_set", Unique = true)]
		public Guid ConceptKey {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the concept set identifier.
		/// </summary>
		/// <value>The concept set identifier.</value>
		[Column("concept_set_uuid"), Indexed(Name = "concept_concept_set_concept_set", Unique = true)]
		public Guid ConceptSetKey {
			get;
			set;
		}
	}
}

