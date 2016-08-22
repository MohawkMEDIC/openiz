using OpenIZ.Core.Model.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents a simple repository service for storing batches (bundles)
    /// </summary>
    public interface IBatchRepositoryService
    {
        
        /// <summary>
        /// Inserts all the data in the provided bundle in one transaction
        /// </summary>
        Bundle Insert(Bundle data);

        /// <summary>
        /// Updates all the data in the provided bundle in one transaction
        /// </summary>
        Bundle Update(Bundle data);

        /// <summary>
        /// Obsoletes all data in the provided bundle in one transaction
        /// </summary>
        Bundle Obsolete(Bundle obsolete);

        /// <summary>
        /// Validate & prepare bundle for insert
        /// </summary>
        Bundle Validate(Bundle bundle);
    }
}
