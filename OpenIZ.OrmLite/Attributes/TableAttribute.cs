using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.OrmLite.Attributes
{
    /// <summary>
    /// Represents a table mapping attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute: Attribute
    {
        /// <summary>
        /// Constructor with name
        /// </summary>
        public TableAttribute(String name)
        {
            this.Name = name;
        }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public String Name { get; set; }
    }
}
