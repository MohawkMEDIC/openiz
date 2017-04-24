using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Taggable persistence service
    /// </summary>
    public interface ITagPersistenceService
    {

        /// <summary>
        /// Save tag to source key
        /// </summary>
        void Save(Guid sourceKey, ITag tag);
    }
}
