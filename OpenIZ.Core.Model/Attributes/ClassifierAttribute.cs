using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Classifier attribute used to mark a class' classifier
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassifierAttribute : Attribute
    {
        /// <summary>
        /// Classifier attribute property
        /// </summary>
        /// <param name="classProperty"></param>
        public ClassifierAttribute(String classProperty)
        {
            this.ClassifierProperty = classProperty;
        }

        /// <summary>
        /// Gets or sets the classifier property
        /// </summary>
        public String ClassifierProperty { get; set; }
    }
}
