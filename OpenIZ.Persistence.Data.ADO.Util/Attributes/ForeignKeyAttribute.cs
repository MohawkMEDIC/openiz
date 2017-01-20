using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.OrmLite.Attributes
{
    /// <summary>
    /// Represents a foreign key
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : Attribute
    {
        /// <summary>
        /// Creates a new foreign key attribute
        /// </summary>
        public ForeignKeyAttribute(Type table, String column)
        {
            this.Table = table;
            this.Column = column;
        }

        /// <summary>
        /// Gets or sets the table to which the key applies
        /// </summary>
        public Type Table { get; set; }

        /// <summary>
        /// Gets or sets the column to which the key applies
        /// </summary>
        public String Column { get; set; }
    }
}
