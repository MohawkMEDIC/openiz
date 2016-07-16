using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Identifies where tools can find the serialization information
    /// for an ignored property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SerializationReferenceAttribute : Attribute
    {
        /// <summary>
        /// The redirection attribute
        /// </summary>
        public SerializationReferenceAttribute(String redirectProperty)
        {
            this.RedirectProperty = redirectProperty;
        }

        /// <summary>
        /// Identifies where the serialization information can be found
        /// </summary>
        public String  RedirectProperty { get; set; }
    }
}
