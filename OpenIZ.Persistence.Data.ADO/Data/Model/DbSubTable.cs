using OpenIZ.Persistence.Data.ADO.Data.Attributes;
using OpenIZ.Persistence.Data.ADO.Data.Model.Acts;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data.Model
{
    /// <summary>
    /// Gets or sets the derived parent class
    /// </summary>
    public abstract class DbSubTable 
    {

        /// <summary>
        /// Parent key
        /// </summary>
        public abstract Guid ParentKey { get; set; }


    }

    /// <summary>
    /// Act based sub-table
    /// </summary>
    public abstract class DbActSubTable : DbSubTable
    {
        /// <summary>
        /// Gets or sets the parent key
        /// </summary>
        [Column("act_vrsn_id"), ForeignKey(typeof(DbActVersion), nameof(DbActVersion.Key)), PrimaryKey]
        public override Guid ParentKey { get; set; }
    }

    /// <summary>
    /// Entity based sub-table
    /// </summary>
    public abstract class DbEntitySubTable : DbSubTable
    {
        /// <summary>
        /// Gets or sets the parent key
        /// </summary>
        [Column("ent_vrsn_id"), ForeignKey(typeof(DbEntityVersion), nameof(DbEntityVersion.VersionKey)), PrimaryKey]
        public override Guid ParentKey { get; set; }
    }

    /// <summary>
    /// Represents a person based sub-table
    /// </summary>
    public abstract class DbPersonSubTable : DbEntitySubTable
    {
        /// <summary>
        /// Gets or sets the parent key
        /// </summary>
        [Column("ent_vrsn_id"), ForeignKey(typeof(DbPerson), nameof(DbPerson.ParentKey)), PrimaryKey]
        public override Guid ParentKey { get; set; }
    }
}
