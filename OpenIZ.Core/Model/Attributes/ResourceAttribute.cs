using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Model scope
    /// </summary>
    public enum ModelScope
    {
        /// <summary>
        /// The scope of the resource is clinical operations
        /// </summary>
        Concept = 0x01, 
        /// <summary>
        /// The scope of the resource is security
        /// </summary>
        Security = 0x02,
        /// <summary>
        /// The scope of the resource is protocol / workflow
        /// </summary>
        Protocol = 0x04,
        /// <summary>
        /// The scope of the resource is stock
        /// </summary>
        StockManagement = 0x08,
        /// <summary>
        /// The scope of the resource is reporting
        /// </summary>
        Reporting = 0x10,
        Clinical = 0x20,
        MetaData = 0x40
    }
    /// <summary>
    /// Resource attribute indicates that a class logically represents a resource
    /// which can be the entry point for REST operations, etc.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ResourceAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the model attribute
        /// </summary>
        public ResourceAttribute(ModelScope scope)
        {
            this.Scope = scope;
        }

        /// <summary>
        /// Model scope
        /// </summary>
        public ModelScope Scope { get; set; }
    }
}
