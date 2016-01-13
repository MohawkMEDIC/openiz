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
    public abstract class VersionedEntityData : BaseEntityData
    {

        /// <summary>
        /// Creates a new versioned base data class
        /// </summary>
        public VersionedEntityData()
        {
            this.Versions = new List<VersionedEntityData>();
        }

        /// <summary>
        /// Gets or sets the versions of this class in the past
        /// </summary>
        public List<VersionedEntityData> Versions { get; set; }

        /// <summary>
        /// Gets or sets the key which represents the version of the entity
        /// </summary>
        public Guid VersionKey { get; set; }

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
