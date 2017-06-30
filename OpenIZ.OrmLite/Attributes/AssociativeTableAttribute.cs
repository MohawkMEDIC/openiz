using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.OrmLite.Attributes
{
    /// <summary>
    /// Indicates that another table is associated with the current table through a third
    /// table
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AssociativeTableAttribute : Attribute
    {

        /// <summary>
        /// Creates an associative table attribute
        /// </summary>
        public AssociativeTableAttribute(Type targetTable, Type associativeTable)
        {
            this.TargetTable = targetTable;
            this.AssociationTable = associativeTable;
        }

        /// <summary>
        /// Gets or sets the target table
        /// </summary>
        public Type TargetTable { get; set; }

        /// <summary>
        /// Gets or sets the table whicih associates the target table with the current table
        /// </summary>
        public Type AssociationTable { get; set; }

    }
}
