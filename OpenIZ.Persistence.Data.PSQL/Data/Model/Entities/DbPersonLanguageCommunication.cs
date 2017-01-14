﻿/*
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
 * Date: 2016-6-28
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.PSQL.Data.Model.Entities
{
    /// <summary>
    /// Represents a person's languages of communication
    /// </summary>
    [TableName("person_language")]
    public class DbPersonLanguageCommunication : DbEntityLink
    {
        /// <summary>
        /// Gets or sets the language code of the communication language
        /// </summary>
        [Column("languageCode")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets wheter this person prefers to be contacted in this language
        /// </summary>
        [Column("isPreferred")]
        public bool IsPreferred { get; set; }


    }
}
