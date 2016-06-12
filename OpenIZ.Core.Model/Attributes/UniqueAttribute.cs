using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Identifies to the persistence layer that the storage should be unique
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : Attribute
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        public UniqueAttribute()
        {
            this.ErrorOnInsert = false;
        }

        /// <summary>
        /// Gets or sets whether the persistence engine should throw an exception when persisting duplicates
        /// </summary>
        public bool ErrorOnInsert { get; set; }

    }
}
