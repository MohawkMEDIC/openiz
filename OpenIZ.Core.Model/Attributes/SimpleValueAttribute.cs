using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Identifies the simple value
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SimpleValueAttribute : Attribute
    {

        /// <summary>
        /// Classifier attribute property
        /// </summary>
        /// <param name="valueProperty"></param>
        public SimpleValueAttribute(String valueProperty)
        {
            this.ValueProperty = valueProperty;
        }

        /// <summary>
        /// Gets or sets the classifier property
        /// </summary>
        public String ValueProperty { get; set; }
    }
}
