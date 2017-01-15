using OpenIZ.Persistence.Data.ADO.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data.Model.Acts
{
    /// <summary>
    /// Table which stores clinical protocols
    /// </summary>
    [Table("proto_tbl")]
    public class DbProtocol : DbBaseData
    {
        /// <summary>
        /// Gets or sets the protocol id
        /// </summary>
        [Column("proto_id"), PrimaryKey]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [Column("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the handler identifier
        /// </summary>
        [Column("hdlr_id"), ForeignKey(typeof(DbProtocolHandler), nameof(DbProtocolHandler.Key))]
        public Guid HandlerKey { get; set; }

        /// <summary>
        /// Gets or sets the OID
        /// </summary>
        [Column("oid")]
        public String Oid { get; set; }

        /// <summary>
        /// Definition of the protocol
        /// </summary>
        [Column("defn")]
        public byte[] Definition { get; set; }

        /// <summary>
        /// Gets or sets the replaces key if applicable
        /// </summary>
        [Column("rplc_proto_id")]
        public Guid? ReplacesProtocolKey { get; set; }
    }

    /// <summary>
    /// Represents a protocol handler
    /// </summary>
    [Table("proto_hdlr_tbl")]
    public class DbProtocolHandler : DbBaseData
    {
        /// <summary>
        /// Gets or sets the primary key
        /// </summary>
        [Column("hdlr_id")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the handler name
        /// </summary>
        [Column("hdlr_name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the handler class
        /// </summary>
        [Column("hdlr_cls")]
        public String TypeName { get; set; }

        /// <summary>
        /// Gets or sets whether the protocol is active
        /// </summary>
        [Column("is_active")]
        public bool IsActive { get; set; }

    }
}
