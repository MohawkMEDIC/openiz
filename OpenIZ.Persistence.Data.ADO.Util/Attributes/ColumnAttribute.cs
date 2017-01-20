using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.OrmLite.Attributes
{
    /// <summary>
    /// Represents an attribute for marking columns
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// gets or sets the database name
        /// </summary>
        public ColumnAttribute(string name)
        {
            this.Name = name;
        }
        
        /// <summary>
        /// Gets or sets the name of the attribute
        /// </summary>
        public String Name { get; set; }
    }
}
