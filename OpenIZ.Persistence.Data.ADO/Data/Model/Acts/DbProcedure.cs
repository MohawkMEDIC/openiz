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
    /// Represents a procedure in the data model
    /// </summary>
    [Table("proc_tbl")]
    public class DbProcedure : DbActSubTable
    {

        /// <summary>
        /// Parent key
        /// </summary>
        [JoinFilter(PropertyName = nameof(DbAct.ClassConceptKey), Value = ActClassKeyStrings.Procedure)]
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
        /// Gets or sets the technique used 
        /// </summary>
        [Column("mth_cd_id"), ForeignKey(typeof(DbConcept), nameof(DbConcept.Key))]
        public Guid? MethodConceptKey { get; set; }

        /// <summary>
        /// Gets or sets the approach body site or system
        /// </summary>
        [Column("apr_ste_cd_id"), ForeignKey(typeof(DbConcept), nameof(DbConcept.Key))]
        public Guid? ApproachSiteConceptKey { get; set; }

        /// <summary>
        /// Gets or sets the target site code
        /// </summary>
        [Column("trg_ste_cd_id"), ForeignKey(typeof(DbConcept), nameof(DbConcept.Key))]
        public Guid? TargetSiteConceptKey { get; set; }
    }
}
