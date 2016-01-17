using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Interfaces
{
    /// <summary>
    /// Represents a data model class which can be used as a securable
    /// </summary>
    public interface ISecurable
    {

        /// <summary>
        /// True if the object is masked
        /// </summary>
        void Mask();
    }
}
