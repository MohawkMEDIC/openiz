using OpenIZ.Persistence.Data.ADO.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data.Model.Acts
{
    /// <summary>
    /// Represents a link between act and protocol
    /// </summary>
    [Table("act_proto_assoc_tbl")]
    public class DbActProtocol : IDbAssociation
    {

        /// <summary>
        /// Gets or sets the protocol key
        /// </summary>
        [Column("proto_id"), ForeignKey(typeof(DbProtocol), nameof(DbProtocol.Key)), PrimaryKey]
        public Guid ProtocolKey { get; set; }

        /// <summary>
        /// Source key
        /// </summary>
        [Column("act_id"), ForeignKey(typeof(DbAct), nameof(DbAct.Key)), PrimaryKey]
        public Guid SourceKey { get; set; }

        /// <summary>
        /// Gets or sets the state
        /// </summary>
        [Column("stat_dat")]
        public byte[] State { get; set; }

        /// <summary>
        /// Gets or sets the complete flag
        /// </summary>
        [Column("is_compl")]
        public bool IsComplete { get; set; }

    }
}
