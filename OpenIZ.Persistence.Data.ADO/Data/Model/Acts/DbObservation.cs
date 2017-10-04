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
using OpenIZ.Core.Model.Constants;
using OpenIZ.OrmLite.Attributes;
using OpenIZ.Persistence.Data.ADO.Data.Model.Concepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data.Model.Acts
{
 
    /// <summary>
    /// Stores data related to an observation act
    /// </summary>
    [Table("obs_tbl")]
    public class DbObservation : DbActSubTable
    {

        /// <summary>
        /// Parent key
        /// </summary>
        [JoinFilter(PropertyName = nameof(DbAct.ClassConceptKey), Value = ActClassKeyStrings.Observation)]
        public override Guid ParentKey
        {
            get
            {
                return base.ParentKey;
            }

            set
            {
                base.ParentKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the interpretation concept
        /// </summary>
        [Column("int_cd_id"), ForeignKey(typeof(DbConcept), nameof(DbConcept.Key))]
        public Guid InterpretationConceptKey { get; set; }

        /// <summary>
        /// Identifies the value type
        /// </summary>
        [Column("val_typ")]
        public String ValueType { get; set; }

    }

    /// <summary>
    /// Represents additional data related to a quantified observation
    /// </summary>
    [Table("qty_obs_tbl")]
    public class DbQuantityObservation : DbObsSubTable
    {
        
        /// <summary>
        /// Represents the unit of measure
        /// </summary>
        [Column("uom_cd_id"), ForeignKey(typeof(DbConcept), nameof(DbConcept.Key))]
        public Guid UnitOfMeasureKey { get; set; }

        /// <summary>
        /// Gets or sets the value of the measure
        /// </summary>
        [Column("qty"), NotNull]
        public Decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the value of the measure
        /// </summary>
        [Column("qty_prc")]
        public Decimal? Precision { get; set; }

    }

    /// <summary>
    /// Identifies the observation as a text obseration
    /// </summary>
    [Table("txt_obs_tbl")]
    public class DbTextObservation : DbObsSubTable
    {
        /// <summary>
        /// Gets the value of the observation as a string
        /// </summary>
        [Column("obs_val")]
        public String Value { get; set; }

    }

    /// <summary>
    /// Identifies data related to a coded observation
    /// </summary>
    [Table("cd_obs_tbl")]
    public class DbCodedObservation : DbObsSubTable
    {

        /// <summary>
        /// Gets or sets the concept representing the value of this
        /// </summary>
        [Column("val_cd_id"), ForeignKey(typeof(DbConcept), nameof(DbConcept.Key))] 
        public Guid? Value { get; set; }
        
    }
}
