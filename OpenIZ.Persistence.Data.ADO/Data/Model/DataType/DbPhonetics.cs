using OpenIZ.Persistence.Data.ADO.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data.Model.DataType
{

    /// <summary>
    /// Phonetic value table
    /// </summary>
    [Table("phon_val_tbl")]
    public class DbPhoneticValue : DbIdentified
    {

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        [Column("val")]
        public String Value { get; set; }

        /// <summary>
        /// Gets or sets the phonetic code.
        /// </summary>
        /// <value>The phonetic code.</value>
        [Column("phon_cs")]
        public String PhoneticCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the phonetic algorithm identifier.
        /// </summary>
        /// <value>The phonetic algorithm identifier.</value>
        [Column("alg_id"), ForeignKey(typeof(DbPhoneticAlgorithm), nameof(DbPhoneticAlgorithm.Key))]
        public Guid PhoneticAlgorithmKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key of the phonetic code
        /// </summary>
        [Column("phon_val_id"), PrimaryKey]
        public override Guid Key { get; set; }
    }

    /// <summary>
    /// Represents a phonetic algorithm
    /// </summary>
    [Table("phon_alg_tbl")]
    public class DbPhoneticAlgorithm : DbNonVersionedBaseData
    {
        /// <summary>
        /// Gets or sets the algorithm key
        /// </summary>
        [Column("alg_id"), PrimaryKey]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the algorithm name
        /// </summary>
        [Column("alg_name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the handler class
        /// </summary>
        [Column("hdlr_cls")]
        public String HandlerClass { get; set; }
    }
}
