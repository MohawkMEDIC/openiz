using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Interfaces
{
    /// <summary>
    /// Represents a class which can hold tags
    /// </summary>
    public interface ITaggable
    {

        /// <summary>
        /// Gets the tags associated with the object
        /// </summary>
        IEnumerable<ITag> Tags { get; }

    }

    /// <summary>
    /// Represents a tag
    /// </summary>
    public interface ITag : ISimpleAssociation
    {
        /// <summary>
        /// Gets the key for the tag
        /// </summary>
        String TagKey { get; }

        /// <summary>
        /// Gets the value for the tag
        /// </summary>
        String Value { get; }

    }
}
