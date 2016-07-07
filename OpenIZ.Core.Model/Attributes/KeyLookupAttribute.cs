using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Identifies to the persistence layer what property can be used for lookup when a key is not present
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class KeyLookupAttribute : Attribute
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        public KeyLookupAttribute(String uniqueProperty)
        {
            this.UniqueProperty = uniqueProperty;
        }

        /// <summary>
        /// Gets or sets whether the persistence engine should throw an exception when persisting duplicates
        /// </summary>
        public String UniqueProperty { get; set; }

    }
}
