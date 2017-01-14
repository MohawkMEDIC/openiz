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
 * Date: 2016-7-1
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.PSQL.Data.Model.Acts
{
    /// <summary>
    /// Represents storage class for a substance administration
    /// </summary>
    [TableName("substance_administration")]
    public class DbSubstanceAdministration : IDbVersionedAssociation
    {
        /// <summary>
        /// Gets or sets the route of administration
        /// </summary>
        [Column("routeConcept")]
        public Guid RouteConceptKey { get; set; }

        /// <summary>
        /// Gets or sets the dose unit
        /// </summary>
        [Column("doseUnit")]
        public Guid DoseUnitConceptKey { get; set; }

        /// <summary>
        /// Gets or sets the site
        /// </summary>
        [Column("site")]
        public Guid SiteConceptKey { get; set; }

        /// <summary>
        /// Gets or sets the dose quantity
        /// </summary>
        [Column("doseQuantity")]
        public Decimal DoseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the sequence number
        /// </summary>
        [Column("sequenceId")]
        public int SequenceId { get; set; }

    }
}
