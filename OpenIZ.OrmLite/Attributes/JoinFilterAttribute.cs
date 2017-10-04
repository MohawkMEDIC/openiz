using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.OrmLite.Attributes
{
    /// <summary>
    /// Join filter attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class JoinFilterAttribute : Attribute
    {

        /// <summary>
        /// The field on which to filter
        /// </summary>
        public String PropertyName { get; set; }

        /// <summary>
        /// The constant to filter on
        /// </summary>
        public String Value { get; set; }
    }
}
