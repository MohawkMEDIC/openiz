using OpenIZ.Core.Services;
using OpenIZ.Persistence.Data.ADO.Data;
using System;
using System.Collections;
using System.Security.Principal;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.OrmLite;

namespace OpenIZ.Persistence.Data.ADO.Services
{
    /// <summary>
    /// Represents an ADO based IDataPersistenceServie
    /// </summary>
    public interface IAdoPersistenceService : IDataPersistenceService
    {
        /// <summary>
        /// Inserts the specified object
        /// </summary>
        Object Insert(DataContext context, Object data, IPrincipal principal);

        /// <summary>
        /// Updates the specified data
        /// </summary>
        Object Update(DataContext context, Object data, IPrincipal principal);

        /// <summary>
        /// Obsoletes the specified data
        /// </summary>
        Object Obsolete(DataContext context, Object data, IPrincipal principal);

        /// <summary>
        /// Gets the specified data
        /// </summary>
        Object Get(DataContext context, Guid id, IPrincipal principal);

        /// <summary>
        /// Map to model instance
        /// </summary>
        Object ToModelInstance(object domainInstance, DataContext context, IPrincipal principal);
    }

    /// <summary>
    /// ADO associative persistence service
    /// </summary>
    public interface IAdoAssociativePersistenceService : IAdoPersistenceService
    {
        /// <summary>
        /// Get the set objects from the source
        /// </summary>
        IEnumerable GetFromSource(DataContext context, Guid id, decimal? versionSequenceId, IPrincipal principal);
    }
}