using System.Collections;

namespace OpenIZ.Core.Model.Collection
{
    /// <summary>
    /// Represents a lockable collection
    /// </summary>
    public interface ILockable
    {

        /// <summary>
        /// Get a locked version of the collection
        /// </summary>
        IEnumerable GetLocked(); 
    }
}