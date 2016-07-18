using OpenIZ.Core.Model.EntityLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Model settings
    /// </summary>
    public static class ModelSettings
    {
        /// <summary>
        /// Threadstatic source provider so it can be overridden in serialization
        /// </summary>
        [ThreadStatic]
        public static IEntitySourceProvider SourceProvider;

        /// <summary>
        /// Deep load objects
        /// </summary>
        [ThreadStatic]
        public static bool DeepLoadObject = true;
    }
}
