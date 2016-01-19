/**
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-19
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Data;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents versioned based data, that is base data which has versions
    /// </summary>
    public abstract class VersionedEntityData<THistoryModelType> : BaseEntityData
    {

        /// <summary>
        /// Creates a new versioned base data class
        /// </summary>
        public VersionedEntityData()
        {
        }

        /// <summary>
        /// Gets or sets the previous verion
        /// </summary>
        public abstract Guid? PreviousVersionKey { get; set; }

        /// <summary>
        /// Gets or sets the versions of this class in the past
        /// </summary>
        public abstract THistoryModelType PreviousVersion { get; set; }

        /// <summary>
        /// Gets or sets the key which represents the version of the entity
        /// </summary>
        public Guid VersionKey { get; set; }

        /// <summary>
        /// The sequence number of the version (for ordering)
        /// </summary>
        public Decimal VersionSequence { get; set; }

        /// <summary>
        /// Gets or sets the IIdentified data for this object
        /// </summary>
        public override Identifier<Guid> Id
        {
            get
            {
                var retVal = base.Id;
                retVal.VersionId = this.VersionKey;
                return retVal;
            }
            set
            {
                base.Id = value;
                this.VersionKey = value.VersionId;
            }
        }

        /// <summary>
        /// Represent the versioned data as a string
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} (K:{1}, V:{2})", this.GetType().Name, this.Key, this.VersionKey);
        }
    }

}
