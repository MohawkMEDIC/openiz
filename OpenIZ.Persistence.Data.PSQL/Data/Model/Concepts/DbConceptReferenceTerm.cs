using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.PSQL.Data.Model.Concepts
{
    /// <summary>
    /// Concept reference term link
    /// </summary>
    [TableName("cd_ref_term_assoc_tbl")]
    public class DbConceptReferenceTerm : DbConceptVersionedAssociation
    {
        /// <summary>
        /// Gets or sets the primary key
        /// </summary>
        [Column("cd_ref_term_id")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the target key
        /// </summary>
        [Column("ref_term_id")]
        public Guid TargetKey { get; set; }

        /// <summary>
        /// Gets or sets the relationship type id
        /// </summary>
        [Column("rel_typ_id")]
        public Guid RelationshipTypeKey { get; set; }
    }
}
