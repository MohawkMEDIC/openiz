using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Binding attributes to suggest what values can be used in a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class BindingAttribute : Attribute
    {

        /// <summary>
        /// Binding attribute
        /// </summary>
        public BindingAttribute(Type binding)
        {
            this.Binding = binding;
        }

        /// <summary>
        /// Gets or sets the type binding
        /// </summary>
        public Type Binding { get; set; }

    }
}
