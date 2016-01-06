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
    public abstract class VersionedBaseData : BaseData
    {

        /// <summary>
        /// Creates a new versioned base data class
        /// </summary>
        public VersionedBaseData()
        {
            this.Versions = new List<VersionedBaseData>();
        }

        /// <summary>
        /// Gets or sets the versions of this class in the past
        /// </summary>
        public List<VersionedBaseData> Versions { get; set; }

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
    }
}
