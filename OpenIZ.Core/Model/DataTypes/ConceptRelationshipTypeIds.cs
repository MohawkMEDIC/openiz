using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Reference type identifiers
    /// </summary>
    public static class ConceptRelationshipTypeIds
    {
        /// <summary>
        /// Same as relationship
        /// </summary>
        public static readonly Guid SameAs = Guid.Parse("2c4dafc2-566a-41ae-9ebc-3097d7d22f4a");
        /// <summary>
        /// Negation of 
        /// </summary>
        public static readonly Guid NegationOf = Guid.Parse("ae8b4f2f-009f-4e0d-b35e-5a89555c5947");
        /// <summary>
        /// Member of
        /// </summary>
        public static readonly Guid MemberOf = Guid.Parse("a159d45b-3c34-4e1b-9b75-9193a7528ced");
        /// <summary>
        /// Inverse of
        /// </summary>
        public static readonly Guid InverseOf = Guid.Parse("ad27293d-433c-4b75-88d2-b5360cd95450");
    }
}
